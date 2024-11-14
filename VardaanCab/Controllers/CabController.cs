using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class CabController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult GetVendorList(string term)
        {
            var data = ent.Vendors.Where(a => a.IsActive && a.CompanyName.ToLower().Contains(term.ToLower())).ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatus(int id,int menuId=0)
        {
            string query = @"update cab set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("All",new { menuId=menuId});
        }

        public ActionResult ChangeAvailStatus (int id, int menuId = 0)
        {
            string query = @"update cab set IsAvailable= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("All",new { menuId=menuId});
        }

        public ActionResult All(string term = "", int page = 1,bool export=false, int menuId=0)
        {
            var model = new CabListVm();
            var data = (from c in ent.Cabs join vm in ent.VehicleModels
                        on c.VehicleModel_Id equals vm.Id into c_vm 
                        from vehiclModel in c_vm.DefaultIfEmpty()
                        join v in ent.Vendors on c.Vendor_Id equals v.Id into c_v
                        from cv in c_v.DefaultIfEmpty()
                        orderby c.Id descending
                        select new CabDTO
                        {
                            Id=c.Id,
                            CreateDate=c.CreateDate,
                            Company=c.Company,
                            CompanyName=c.Company,
                            ModelName= vehiclModel.ModelName,
                            FitnessVality=c.FitnessVality,
                            RCDoc=c.RCDoc,
                            RcIssueDate=c.RcIssueDate,
                            RcValidity=c.RcValidity,
                            RcNumber=c.RcNumber,
                            PermitValidity=c.PermitValidity,
                            IsActive=c.IsActive,
                            FitnessDoc=c.FitnessDoc,
                            InsuranceDoc=c.InsuranceDoc,
                            InsurranceValidity=c.InsurranceValidity,
                            PolutionDoc=c.PolutionDoc,
                            PolutionValidity=c.PolutionValidity,
                            VehicleModel_Id=c.VehicleModel_Id,
                            VendorName= cv.CompanyName,
                            VehicleNumber=c.VehicleNumber,
                            Vendor_Id=c.Vendor_Id,
                            PermitDoc=c.PermitDoc,
                            PermitNo=c.PermitNo,
                            IsAvailable=c.IsAvailable,
                            FuelEfficiency=c.FuelEfficiency
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.Company.ToLower().Contains(term) || a.VehicleNumber.ToLower().Contains(term) || a.ModelName.ToLower().Contains(term) ).ToList();
                page = 1;
            }

            if (export)
            {
               
                var ed = (from d in data
                          select new  CabExcelModel
                          {
                              CreateDate = d.CreateDate.Date,
                              ModelName = d.ModelName,
                              FitnessVality = d.FitnessVality.Date,
                              RcIssueDate = d.RcIssueDate,
                              RcValidity = d.RcValidity,
                              RcNumber = d.RcNumber,
                              PermitValidity = d.PermitValidity.Date,
                              InsurranceValidity = d.InsurranceValidity.Date,
                              PolutionValidity = d.PolutionValidity.Date,
                              VendorName = d.VendorName,
                              VehicleNumber = d.VehicleNumber,
                              PermitNo = d.PermitNo,
                              FuelEfficiency = d.FuelEfficiency
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =cabList.xls");
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
            int pageSize = 500;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Cabs = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new CabDTO();
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CabDTO model)
        {
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");


            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if(ent.Cabs.Any(a=>a.VehicleNumber.ToLower().Equals(model.VehicleNumber.ToUpper())))
                {
                    TempData["msg"] = "Vehicle number already exist in our database";
                    return View(model);
                }

                if (model.FitnessDocFile != null)
                {
                    if (model.FitnessDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "fitness doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.FitnessDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as fitness doc";
                        return View(model);
                    }
                    model.FitnessDoc = img;
                }

                if (model.InsuranceDocFile != null)
                {
                    if (model.InsuranceDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Insurrance doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.InsuranceDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Insurrance doc";
                        return View(model);
                    }
                    model.InsuranceDoc = img;
                }

                if (model.PolutionDocFile != null)
                {
                    if (model.PolutionDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Polution Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolutionDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Polution Doc";
                        return View(model);
                    }
                    model.PolutionDoc = img;
                }

                if (model.RCDocFile != null)
                {
                    if (model.RCDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "RC Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.RCDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as RC Doc";
                        return View(model);
                    }
                    model.RCDoc = img;
                }

                if (model.PermitDocFile != null)
                {
                    if (model.PermitDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Permit doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PermitDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as permit doc";
                        return View(model);
                    }
                    model.PermitDoc = img;
                }
                model.VehicleNumber=model.VehicleNumber.ToUpper();
                var data = Mapper.Map<Cab>(model);
                data.CreateDate = DateTime.Now;
                data.IsActive = true;
                data.IsAvailable = true;
                ent.Cabs.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error,please contact to admin department.";
            }

            return RedirectToAction("Add", new {menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.Cabs.Find(id);
            var model = Mapper.Map<CabDTO>(data);
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",data.VehicleModel_Id);
            var vendor = ent.Vendors.Find(data.Vendor_Id);
            if(vendor !=null)
            {
                model.CompanyName = vendor.CompanyName;
            }
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CabDTO model)
        {
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);

            try
            {
               // var cabData = ent.Cabs.AsNoTracking().FirstOrDefault(a=>a.Id==model.Id);
                if (!string.IsNullOrEmpty(model.FitnessDoc))
                    ModelState.Remove("FitnessDocFile");

                if(!string.IsNullOrEmpty(model.PolutionDoc))
                    ModelState.Remove("PolutionDocFile");

                if(!string.IsNullOrEmpty(model.InsuranceDoc))
                    ModelState.Remove("InsuranceDocFile");

                if(!string.IsNullOrEmpty(model.RCDoc))
                    ModelState.Remove("RCDocFile");

                if (!string.IsNullOrEmpty(model.PermitDoc))
                    ModelState.Remove("PermitDocFile");

                if (!ModelState.IsValid)
                    return View(model);

                if (ent.Cabs.Any(a =>a.Id!=model.Id && a.VehicleNumber.ToLower().Equals(model.VehicleNumber.ToLower())))
                {
                    TempData["msg"] = "Vehicle number already exist in our database";
                    return View(model);
                }
                if (model.FitnessDocFile != null)
                {
                    if (model.FitnessDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "fitness doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.FitnessDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as fitness doc";
                        return View(model);
                    }
                    model.FitnessDoc = img;
                }

                if (model.InsuranceDocFile != null)
                {
                    if (model.InsuranceDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Insurrance doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.InsuranceDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Insurrance doc";
                        return View(model);
                    }
                    model.InsuranceDoc = img;
                }

                if (model.PolutionDocFile != null)
                {
                    if (model.PolutionDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Polution Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolutionDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Polution Doc";
                        return View(model);
                    }
                    model.PolutionDoc = img;
                }

                if (model.RCDocFile != null)
                {
                    if (model.RCDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "RC Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.RCDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as RC Doc";
                        return View(model);
                    }
                    model.RCDoc = img;
                }
                if (model.PermitDocFile != null)
                {
                    if (model.PermitDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Permit doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PermitDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as permit doc";
                        return View(model);
                    }
                    model.PermitDoc = img;
                }
                var data = Mapper.Map<Cab>(model);
                ent.Entry(data).State=System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id,menuId=model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = ent.Cabs.Find(id);
                ent.Cabs.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }
    }
}