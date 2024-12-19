using AutoMapper;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data;
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
                driver.IsOnline = false;
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

        public ActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DriverName");
            dt.Columns.Add("DlNumber");
            dt.Columns.Add("DlImage");
            dt.Columns.Add("AadharNo");
            dt.Columns.Add("AadharImage");
            dt.Columns.Add("DriverImage");
            dt.Columns.Add("MobileNumber");
            dt.Columns.Add("DriverAddress");
            dt.Columns.Add("PoliceVerificationNo");
            dt.Columns.Add("LicenceExpDate");
            dt.Columns.Add("PolVerifImg");
            dt.Columns.Add("FatherName");
            dt.Columns.Add("AlternateNo1");
            dt.Columns.Add("AlternateNo2");
            dt.Columns.Add("LocalAddress");
            dt.Columns.Add("PermanentAddress");
            dt.Columns.Add("PanImage");
            dt.Columns.Add("PanNumber");
            dt.Columns.Add("Email");

            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
    {
        { "DriverName", "Driver Name" },
        { "DlNumber", "Dl Number" },
        { "DlImage", "Dl Image" },
        { "AadharNo", "Aadhar No" },
        { "AadharImage", "Aadhar Image" },
        { "DriverImage", "Driver Image" },
        { "MobileNumber", "Mobile Number" },
        { "DriverAddress", "Driver Address" },
        { "PoliceVerificationNo", "Police Verification No" },
        { "LicenceExpDate", "Licence Expire Date" },
        { "PolVerifImg", "Pol Verif Img" },
        { "FatherName", "Father Name" },
        { "AlternateNo1", "Alternate No 1" },
        { "AlternateNo2", "Alternate No 2" },
        { "LocalAddress", "Local Address" },
        { "PermanentAddress", "Permanent Address" },
        { "PanImage", "Pan Image" },
        { "PanNumber", "Pan Number" },
        { "Email", "Email" }
    };

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Driver");

                int colIndex = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    string oldColumnName = column.ColumnName;
                    worksheet.Cell(1, colIndex).Value = columnMappings.ContainsKey(oldColumnName)
                        ? columnMappings[oldColumnName]
                        : oldColumnName;

                    worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.Yellow;
                    colIndex++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DriverData.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ImportDriverData(HttpPostedFileBase file)
        {
            try
            {
                string randomPassword = _random.GenerateRandomPassword();

                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<Driver> drivers = new List<Driver>();

                        foreach (var row in rows)
                        {
                            string ff = row.Cell(10).GetValue<string>();

                            Driver driver = new Driver
                            {
                                DriverName = row.Cell(1).GetValue<string>() ?? string.Empty,
                                DlNumber = row.Cell(2).GetValue<string>() ?? string.Empty,
                                DlImage = row.Cell(3).GetValue<string>() ?? string.Empty,
                                AadharNo = row.Cell(4).GetValue<string>() ?? string.Empty,
                                AadharImage = row.Cell(5).GetValue<string>() ?? string.Empty,
                                DriverImage = row.Cell(6).GetValue<string>() ?? string.Empty,
                                MobileNumber = row.Cell(7).GetValue<string>() ?? string.Empty,
                                DriverAddress = row.Cell(8).GetValue<string>() ?? string.Empty,
                                PoliceVerificationNo = row.Cell(9).GetValue<string>() ?? string.Empty,
                                LicenceExpDate = row.Cell(10).GetValue<DateTime>(),
                                PolVerifImg = row.Cell(11).GetValue<string>() ?? string.Empty,
                                FatherName = row.Cell(12).GetValue<string>() ?? string.Empty,
                                AlternateNo1 = row.Cell(13).GetValue<string>() ?? string.Empty,
                                AlternateNo2 = row.Cell(14).GetValue<string>() ?? string.Empty,
                                LocalAddress = row.Cell(15).GetValue<string>() ?? string.Empty,
                                PermanentAddress = row.Cell(16).GetValue<string>() ?? string.Empty,
                                PanNumber = row.Cell(17).GetValue<string>() ?? string.Empty,
                                PanImage = row.Cell(18).GetValue<string>() ?? string.Empty,
                                Password = randomPassword,
                                Email = row.Cell(19).GetValue<string>() ?? string.Empty,
                                IsFirst = false,
                                IsOnline = false,
                                IsActive = true,
                                CreateDate = DateTime.Now,
                                IsOutsider = false,
                                IsAvailable = false
                            };

                            drivers.Add(driver);
                        }

                        if (drivers.Any())
                        {
                            ent.Drivers.AddRange(drivers);
                            ent.SaveChanges();
                        }

                        TempData["msg"] = "Data imported successfully!";
                        return RedirectToAction("All");
                    }
                }

                TempData["errormsg"] = "Please select an Excel file to import.";
                return RedirectToAction("All");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                TempData["errormsg"] = "Validation failed for one or more entities. Please check the logs for more details.";
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("All");
            }
        }


    }
}