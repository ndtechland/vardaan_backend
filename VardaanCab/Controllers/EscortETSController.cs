using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class EscortETSController : Controller
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
        private readonly IEscort _escort;
        public EscortETSController(IEscort escort)
        {
            _escort = escort;
        }
        // GET: EscortETS
        public ActionResult Escort(int menuId = 0, int id = 0)
        {
            var model = new EscortDTO();
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x=>x.Id).ToList(), "Id", "OrgName");
            model.Vendors = new SelectList(ent.Vendors.Where(x => x.IsActive == true).ToList(), "Id", "VendorName");
            
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.Escorts.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.EscortFatheName = data.EscortFatheName;
                model.CompanyId = data.CompanyId;
                model.EscortName = data.EscortName;
                model.EscortMobileNumber = data.EscortMobileNumber;
                model.EscortAadhaarNumber = data.EscortAadhaarNumber;
                model.VendorId = data.VendorId;
                model.DOB = data.DOB;
                model.EscortEmployeeId = data.EscortEmployeeId;
                model.Pincode = data.Pincode;
                model.PermanentAddress = data.PermanentAddress;
                model.EscortAddress = data.EscortAddress;
                ViewBag.Heading = "Update Escort";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.EscortFatheName = "";
                model.CompanyId = 0;
                model.EscortName = "";
                model.EscortMobileNumber = "";
                model.EscortAadhaarNumber = "";
                model.VendorId = 0;
                model.DOB = null;
                model.EscortEmployeeId = "";
                model.Pincode = "";
                model.PermanentAddress = "";
                model.EscortAddress = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Escort";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Escort(EscortDTO model)
        {
            try
            {
                var empinfo = ent.Employees.Where(e => e.IsActive == true && e.Employee_Id == model.EscortEmployeeId).FirstOrDefault();
                if(empinfo==null)
                {
                    TempData["errormsg"] = $"Failed. Please register as an employee first with employee id {model.EscortEmployeeId}.";
                    return RedirectToAction("Escort", new { menuId = model.MenuId });

                }
                else
                {
                    bool isCreated = await _escort.AddUpdateEscort(model);

                    if (isCreated)
                    {
                        TempData["msg"] = model.Id > 0
                            ? "Record has been updated successfully."
                            : "Record has been added successfully.";
                        return RedirectToAction("Escort", new { menuId = model.MenuId });

                    }
                    else
                    {
                        TempData["errormsg"] = "Failed.";
                        return RedirectToAction("Escort", new { menuId = model.MenuId });

                    }

                }

            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Server error: " + ex.Message;
                return View(model);
            }
        }

        public async Task<ActionResult> EscortsList()
        {
            try
            {
                var model = new EscortDTO();

                model.EscortList = await _escort.GetEscorts();

                return View(model);
            }
            catch (Exception)
            {
                TempData["Errormsg"] = "An unexpected error occurred. Please try again later.";
                return View(new EscortDTO());
            }
        }

        public async Task<ActionResult> DeleteEscort(int id)
        {
            try
            {
               bool isDeleted=await _escort.DeleteEscort(id);
                if (isDeleted)
                {
                    TempData["dltmsg"] = "Deleted successfully.";
                }
                else
                {
                    TempData["dltmsg"] = "Data not fount";
                }
                
                return RedirectToAction("EscortsList");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExportToExcel()
        {
            string qry = @"SELECT 
    er.Id,
    c.CompanyName,
    er.EscortName,
    er.EscortFatheName,
er.EscortMobileNumber,
    v.VendorName,
    er.DOB,
    er.EscortEmployeeId,
    er.Pincode,
    er.PermanentAddress,
    er.EscortAddress,
    er.CreatedDate
FROM Escort er
JOIN Employee e ON er.EscortEmployeeId = e.Employee_Id
JOIN Customer c ON er.CompanyId = c.Id
JOIN Vendor v ON er.VendorId = v.Id
ORDER BY er.Id DESC;";
            var data = ent.Database.SqlQuery<Escorts>(qry).ToList();
            var excelData = ExportVisitsToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "escorts.xlsx");
        }
        public byte[] ExportVisitsToExcel(IEnumerable<Escorts> Escorts)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Escorts");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Escort Name";
                worksheet.Cell(currentRow, 2).Value = "Escort Id";
                worksheet.Cell(currentRow, 3).Value = "Escort Gender";
                worksheet.Cell(currentRow, 4).Value = "Escort Address";
                worksheet.Cell(currentRow, 5).Value = "Permanent Address";
                worksheet.Cell(currentRow, 6).Value = "Escort Mobile Number";
                worksheet.Cell(currentRow, 7).Value = "Date Of Birth";
                worksheet.Cell(currentRow, 8).Value = "Escort Vendor Name";
                worksheet.Cell(currentRow, 9).Value = "Escort Batch Number";
                worksheet.Cell(currentRow, 10).Value = "Escort Father Name";
                worksheet.Cell(currentRow, 11).Value = "Escort Pin code";
                worksheet.Cell(currentRow, 12).Value = "Remarks";
                worksheet.Cell(currentRow, 13).Value = "Facility Name";
                currentRow++;
                foreach (var item in Escorts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.EscortName;
                    worksheet.Cell(currentRow, 2).Value = item.Id;
                    worksheet.Cell(currentRow, 3).Value = item.Gender;
                    worksheet.Cell(currentRow, 4).Value = item.EscortAddress;
                    worksheet.Cell(currentRow, 5).Value = item.PermanentAddress;
                    worksheet.Cell(currentRow, 6).Value = item.EscortMobileNumber;
                    worksheet.Cell(currentRow, 7).Value = item.DOB;
                    worksheet.Cell(currentRow, 8).Value = item.VendorName;
                    worksheet.Cell(currentRow, 9).Value = "";
                    worksheet.Cell(currentRow, 10).Value = item.EscortFatheName;
                    worksheet.Cell(currentRow, 11).Value = item.Pincode;
                    worksheet.Cell(currentRow, 12).Value = "";
                    worksheet.Cell(currentRow, 13).Value = item.CompanyName;
                    currentRow++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public ActionResult ExportToExcelForImport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CompanyId");
            dt.Columns.Add("EscortName");
            dt.Columns.Add("EscortFatheName");
            dt.Columns.Add("EscortMobileNumber");
            dt.Columns.Add("EscortAadhaarNumber");
            dt.Columns.Add("VendorId");
            dt.Columns.Add("DOB");
            dt.Columns.Add("EscortEmployeeId");
            dt.Columns.Add("Pincode");            
            dt.Columns.Add("PermanentAddress");
            dt.Columns.Add("EscortAddress");

            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
            {
            { "CompanyId", "Company" },
            { "EscortName", "Escort Name" },
            { "EscortFatheName", "Escort Father Name" },
            { "EscortMobileNumber", "Escort Mobile Number" },
            { "EscortAadhaarNumber", "Escort Aadhaar Number" },
            { "VendorId", "Vendor" },
            { "DOB", "DOB" },
            { "EscortEmployeeId", "Escort EmployeeId" },
            { "Pincode", "Pincode" },            
            { "PermanentAddress", "Permanent Address" },
            { "EscortAddress", "Escort Address" }
            };

            // Export to Excel
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Escort");


                int colIndex = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    string oldColumnName = column.ColumnName;
                    if (columnMappings.ContainsKey(oldColumnName))
                    {
                        worksheet.Cell(1, colIndex).Value = columnMappings[oldColumnName];
                    }
                    else
                    {
                        worksheet.Cell(1, colIndex).Value = oldColumnName;
                    }
                    worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.Yellow;
                    colIndex++;
                }

                // Create a hidden sheet to store company names for dropdown
                var hiddenSheet = workbook.Worksheets.Add("CompanyList");
                var hiddenVendorSheet = workbook.Worksheets.Add("VendorList");


                // Retrieve active customers for the dropdown list
                var companyList = ent.Customers.Where(x=>x.IsActive).ToList();
                var vendorList = ent.Vendors.Where(x => x.IsActive).ToList();

                // Populate hidden sheet with company names
                int hiddenRow = 1;
                foreach (var company in companyList.OrderByDescending(x => x.Id))
                {
                    hiddenSheet.Cell(hiddenRow++, 1).Value = company.OrgName;
                }
                hiddenRow = 1;
                foreach (var ven in vendorList.OrderByDescending(x => x.Id))
                {
                    hiddenVendorSheet.Cell(hiddenRow++, 1).Value = ven.VendorName;
                }
                // Define the dropdown list range
                var companyRange = hiddenSheet.Range($"A1:A{companyList.Count}");
                var VendorRange = hiddenVendorSheet.Range($"A1:A{vendorList.Count}");


                //Apply dropdown list validation to cell A2(under "Company ID")
                var validationOne = worksheet.Cell(2, 1).DataValidation;
                validationOne.List(companyRange); // Dropdown from hidden sheet
                validationOne.IgnoreBlanks = true;
                validationOne.InCellDropdown = true;

                //Vendor
                var validationVendorOne = worksheet.Cell(2, 6).DataValidation;
                validationVendorOne.List(VendorRange); // Dropdown from hidden sheet
                validationVendorOne.IgnoreBlanks = true;
                validationVendorOne.InCellDropdown = true;

                // Save and return Excel file as download
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EscortData.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ImportEscortData(HttpPostedFileBase file)
        {
            try
            {

                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<Escort> escorts = new List<Escort>();

                        foreach (var row in rows)
                        {
                            string comapny = row.Cell(1).GetValue<string>();
                            string Vendors = row.Cell(6).GetValue<string>();

                            Escort es = new Escort
                            {
                                CompanyId = string.IsNullOrEmpty(comapny) ? 0 :
                                    ent.Customers.Where(x => x.OrgName.ToLower() == comapny.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                EscortName = row.Cell(2).GetValue<string>() ?? string.Empty,
                                EscortFatheName = row.Cell(3).GetValue<string>() ?? string.Empty,
                                EscortMobileNumber = row.Cell(4).GetValue<string>() ?? string.Empty,
                                EscortAadhaarNumber = row.Cell(5).GetValue<string>() ?? string.Empty,
                                VendorId = string.IsNullOrEmpty(Vendors) ? 0 :
                                    ent.Vendors.Where(x => x.VendorName.ToLower() == Vendors.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,
                                DOB = row.Cell(7).GetValue<DateTime>(),
                                EscortEmployeeId = row.Cell(8).GetValue<string>() ?? string.Empty,
                                Pincode = row.Cell(9).GetValue<string>() ?? string.Empty,
                                PermanentAddress = row.Cell(10).GetValue<string>() ?? string.Empty,
                                EscortAddress = row.Cell(11).GetValue<string>() ?? string.Empty,                             
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                            };

                            escorts.Add(es);
                        }

                        if (escorts.Any())
                        {
                            ent.Escorts.AddRange(escorts);
                            ent.SaveChanges();
                        }
                        TempData["dltmsg"] = "Data imported successfully!";
                        return RedirectToAction("EscortsList");
                    }
                }

                TempData["msg"] = "Please select an Excel file to import.";
                return RedirectToAction("Escort");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        // Log or output the validation errors
                        Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                ViewBag.Message = "Validation failed for one or more entities. Please check the logs for more details.";
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception or handle other errors
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }
    }
}