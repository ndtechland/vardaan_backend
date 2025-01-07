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
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Exceptions;
using System.Text.RegularExpressions;

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
            dt.Columns.Add("AadharNo");
            dt.Columns.Add("MobileNumber");
            dt.Columns.Add("DriverAddress");
            dt.Columns.Add("PoliceVerificationNo");
            dt.Columns.Add("LicenceExpDate");
            dt.Columns.Add("FatherName");
            dt.Columns.Add("AlternateNo1");
            dt.Columns.Add("AlternateNo2");
            dt.Columns.Add("LocalAddress");
            dt.Columns.Add("PermanentAddress");
            dt.Columns.Add("PanNumber");
            dt.Columns.Add("Email");

            // Add dummy data
            dt.Rows.Add("ABC", "DL12345", "123456789012", "9876543210", "123 Main Street", "PV123", "2025-12-31",
                        "xyz", "9876543211", "9876543212", "456 Local St", "789 Permanent St", "PAN123456", "abcd@example.com");
            dt.Rows.Add("XYZ", "DL67890", "987654321098", "8765432109", "456 Elm Street", "PV456", "2026-05-15",
                        "abc", "8765432108", "8765432107", "789 Local St", "123 Permanent St", "PAN987654", "xyz12@example.com");

            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
    {
        { "DriverName", "Driver Name" },
        { "DlNumber", "DL Number" },
        { "AadharNo", "Aadhar No" },
        { "MobileNumber", "Mobile Number" },
        { "DriverAddress", "Driver Address" },
        { "PoliceVerificationNo", "Police Verification No" },
        { "LicenceExpDate", "Licence Expire Date" },
        { "FatherName", "Father Name" },
        { "AlternateNo1", "Alternate No 1" },
        { "AlternateNo2", "Alternate No 2" },
        { "LocalAddress", "Local Address" },
        { "PermanentAddress", "Permanent Address" },
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

                int rowIndex = 2;
                foreach (DataRow row in dt.Rows)
                {
                    colIndex = 1;
                    foreach (var item in row.ItemArray)
                    {
                        worksheet.Cell(rowIndex, colIndex).Value = item?.ToString() ?? string.Empty;
                        colIndex++;
                    }
                    rowIndex++;
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
                if (file == null || file.ContentLength <= 0)
                {
                    TempData["errormsg"] = "Please select an Excel file to import.";
                    return RedirectToAction("All");
                }

                using (var workbook = new XLWorkbook(file.InputStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1); // Skip header row
                    List<Driver> drivers = new List<Driver>();
                    List<ExcelErrorModel> excelErrorModels = new List<ExcelErrorModel>();

                    int count = 0;
                    foreach (var row in rows)
                    {
                        count++;
                        string randomPassword = _random.GenerateRandomPassword();

                        // Mobile Number Validation
                        string mobno = row.Cell(4).GetValue<string>()?.Trim() ?? string.Empty;
                        if (!mobno.All(char.IsDigit))
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Mobile Number",
                                AffectedRow = count,
                                Description = $"Mobile Number {mobno} contains invalid characters. Only digits are allowed."
                            });
                        }
                        else if (mobno.Length != 10)
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Mobile Number",
                                AffectedRow = count,
                                Description = $"Mobile Number {mobno} must be exactly 10 digits."
                            });
                        }
                        else if (ent.Drivers.Where(d => d.IsActive).Any(e => e.MobileNumber == mobno))
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Mobile Number",
                                AffectedRow = count,
                                Description = $"Mobile Number {mobno} already exists in the system."
                            });
                        }

                        // Aadhar Number Validation
                        string aadharno = row.Cell(3).GetValue<string>()?.Trim() ?? string.Empty;
                        if (!aadharno.All(char.IsDigit))
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Aadhar Number",
                                AffectedRow = count,
                                Description = $"Aadhar Number {aadharno} contains invalid characters. Only digits are allowed."
                            });
                        }
                        else if (aadharno.Length != 12)
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Aadhar Number",
                                AffectedRow = count,
                                Description = $"Aadhar Number {aadharno} must be exactly 12 digits."
                            });
                        }

                        // Alternate Numbers Validation
                        string alternateno1 = row.Cell(9).GetValue<string>()?.Trim() ?? string.Empty;
                        string alternateno2 = row.Cell(10).GetValue<string>()?.Trim() ?? string.Empty;

                        ValidateAlternateNumber(alternateno1, "Alternate Number 1", count, excelErrorModels);
                        ValidateAlternateNumber(alternateno2, "Alternate Number 2", count, excelErrorModels);

                        // Email Validation
                        string email = row.Cell(14).GetValue<string>()?.Trim() ?? string.Empty;
                        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            excelErrorModels.Add(new ExcelErrorModel
                            {
                                ErrorType = "Email Address",
                                AffectedRow = count,
                                Description = $"Email address {email} is invalid. Please provide a valid email."
                            });
                        }

                        // If errors exist for the current row, skip adding it to the drivers list
                        if (excelErrorModels.Any(e => e.AffectedRow == count)) continue;

                        // Add driver to list
                        drivers.Add(new Driver
                        {
                            DriverName = row.Cell(1).GetValue<string>() ?? string.Empty,
                            DlNumber = row.Cell(2).GetValue<string>() ?? string.Empty,
                            AadharNo = aadharno,
                            MobileNumber = mobno,
                            DriverAddress = row.Cell(5).GetValue<string>() ?? string.Empty,
                            PoliceVerificationNo = row.Cell(6).GetValue<string>() ?? string.Empty,
                            LicenceExpDate = row.Cell(7).GetValue<DateTime>(),
                            FatherName = row.Cell(8).GetValue<string>() ?? string.Empty,
                            AlternateNo1 = alternateno1,
                            AlternateNo2 = alternateno2,
                            LocalAddress = row.Cell(11).GetValue<string>() ?? string.Empty,
                            PermanentAddress = row.Cell(12).GetValue<string>() ?? string.Empty,
                            PanNumber = row.Cell(13).GetValue<string>() ?? string.Empty,
                            Password = randomPassword,
                            Email = email,
                            IsFirst = false,
                            IsOnline = false,
                            IsActive = true,
                            CreateDate = DateTime.Now,
                            IsOutsider = false,
                            IsAvailable = false
                        });
                    }

                    // If errors exist, return them to the view
                    if (excelErrorModels.Any())
                    {
                        TempData["HasErrors"] = true;
                        TempData["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                        return RedirectToAction("Add");
                    }

                    // Save drivers to database
                    if (drivers.Any())
                    {
                        ent.Drivers.AddRange(drivers);
                        ent.SaveChanges();
                    }

                    TempData["msg"] = "Data imported successfully!";
                    return RedirectToAction("All");
                }
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

        /// <summary>
        /// Validates an alternate number for a specific driver.
        /// </summary>
        private void ValidateAlternateNumber(string alternateno, string errorType, int row, List<ExcelErrorModel> errors)
        {
            if (!string.IsNullOrEmpty(alternateno) && !alternateno.All(char.IsDigit))
            {
                errors.Add(new ExcelErrorModel
                {
                    ErrorType = errorType,
                    AffectedRow = row,
                    Description = $"{errorType} {alternateno} contains invalid characters. Only digits are allowed."
                });
            }
            else if (!string.IsNullOrEmpty(alternateno) && alternateno.Length != 10)
            {
                errors.Add(new ExcelErrorModel
                {
                    ErrorType = errorType,
                    AffectedRow = row,
                    Description = $"{errorType} {alternateno} must be exactly 10 digits."
                });
            }
        }




    }
}