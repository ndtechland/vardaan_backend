using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly CommonOperations _random = new CommonOperations();
        public ActionResult All(string term = "", int page = 1, bool export = false, int menuId=0)
        {
            var model = new DriverListVm();
            var data = ent.Drivers.OrderByDescending(a=>a.Id).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.DriverName.ToLower().Contains(term) ||(a.MobileNumber!=null && a.MobileNumber.Contains(term))).ToList();
                page = 1;
            }

                if (export)
                {
                    var ed = (from d in data
                              select new DriverExcelModel
                              {
                                  DriverName=d.DriverName,
                                  MobileNumber=d.MobileNumber,
                                  AlternateNo1=d.AlternateNo1,
                                  AlternateNo2=d.AlternateNo2,
                                  RegistrationDate=d.CreateDate.Date,
                                  LocalAddress=d.LocalAddress,
                                  PermanentAddress=d.PermanentAddress,
                                  AadharNo=d.AadharNo,
                                  LicenceNumber=d.DlNumber,
                                  LicenceExpDate=d.LicenceExpDate.Date,
                                  PanNumber=d.PanNumber,
                              }).ToList(); ;
                    var grid = new GridView();
                    grid.DataSource = ed;
                    grid.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename =driverList.xls");
                    Response.ContentType = "application/vnd.ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            
            // pagination
            int total = data.Count;
            int pageSize = 1000;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Drivers = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            ViewBag.menuId = menuId;
            return View();
        }

        [HttpPost]
        public ActionResult Add(DriverDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if(ent.Drivers.Any(a=>a.MobileNumber.ToLower().Equals(model.MobileNumber.ToLower())))
                {
                    TempData["msg"] = "Driver Mobile Number is already exist in our database";
                    return View(model);
                }

                if (model.DriverImageFile!=null)
                {
                    if(model.DriverImageFile.ContentLength>1*1024*1024)
                    {
                        TempData["msg"] = "Driver image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.DriverImageFile, "Images");
                    if(img=="not allowed")
                    {
                        TempData["msg"] = "only .jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.DriverImage = img;
                }

                if (model.AadharImageFile != null)
                {
                    if (model.AadharImageFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Aadhar image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.AadharImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.AadharImage = img;
                }

                if (model.DlImageFile != null)
                {
                    if (model.DlImageFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "DL image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.DlImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.DlImage = img;
                }

                if (model.PolVerifImgFile != null)
                {
                    if (model.PolVerifImgFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Police verif. image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolVerifImgFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Police verif. image";
                        return View(model);
                    }
                    model.PolVerifImg = img;
                }

                if (model.PanFile != null)
                {
                    if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Pan image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PanFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Pan image";
                        return View(model);
                    }
                    model.PanImage = img;
                }
                var rendompass = _random.GenerateRandomPassword();
                var driver = Mapper.Map<Driver>(model);
                driver.CreateDate = DateTime.Now;
                driver.IsActive = true;
                driver.Email = model.Email;
                driver.Password = rendompass;
                driver.IsAvailable = true;
                driver.IsFirst = true;
                ent.Drivers.Add(driver);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.Drivers.Find(id);
            var model = Mapper.Map<DriverDTO>(data);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DriverDTO model)
        {
            try
            {
                ModelState.Remove("DlImageFile");
                ModelState.Remove("AadharImageFile");
                ModelState.Remove("DriverImageFile");
                ModelState.Remove("PolVerifImgFile");
                ModelState.Remove("PanFile"); 
                if (!ModelState.IsValid)
                    return View(model);

                if (ent.Drivers.Any(a =>a.Id!=model.Id && a.DriverName.ToLower().Equals(model.DriverName.ToLower())))
                {
                    TempData["msg"] = "Driver name already exist in our database";
                    return View(model);
                }

                if (model.DriverImageFile != null)
                {
                    if (model.DriverImageFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Driver image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.DriverImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.DriverImage = img;
                }

                if (model.AadharImageFile != null)
                {
                    if (model.AadharImageFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Aadhar image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.AadharImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.AadharImage = img;
                }

                if (model.DlImageFile != null)
                {
                    if (model.DlImageFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "DL image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.DlImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Driver's image";
                        return View(model);
                    }
                    model.DlImage = img;
                }

                if (model.PolVerifImgFile != null)
                {
                    if (model.PolVerifImgFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Police verif. image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolVerifImgFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Police verif. image";
                        return View(model);
                    }
                    model.PolVerifImg = img;
                }
                if (model.PanFile != null)
                {
                    if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Pan image size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PanFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Pan image";
                        return View(model);
                    }
                    model.PanImage = img;
                }
                var driver = Mapper.Map<Driver>(model);
                driver.CreateDate = DateTime.Now;
                driver.Email = model.Email;
                ent.Entry(driver).State=System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Records have updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit",new {id=model.Id,menuId=model.MenuId});
        }

        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        var data = ent.Drivers.Find(id);
        //        ent.Drivers.Remove(data);
        //        ent.SaveChanges();
        //        return Content("ok");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("Server error");
        //    }
        //}

        public ActionResult ChangeDriverStatus(int id,int menuId=0)
        {
            string query = @"update driver set isactive= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
            }
            catch(Exception ex)
            {
            }
            return RedirectToAction("All",new { menuId=menuId });
        }

    }
}