using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Exceptions;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
using System.Web.UI.WebControls;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class ETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
        private readonly IETS _ets;
        public ETSController(IETS ets)
        {
            _ets = ets;
        }
        // GET: ETS
        public ActionResult CreateRequest(int menuId = 0, int id = 0)
        {
            var model = new CreateRequestDTO();
            model.Companies = new SelectList(ent.Customers.Where(c=>c.IsActive==true).OrderByDescending(c=>c.Id).ToList(), "Id", "OrgName");
            model.TripTypes = new SelectList(ent.TripTypes.Where(x=>x.TripMasterId==1).ToList(), "Id", "TripTypeName");
            model.ShiftTypes = new SelectList(ent.TripMasters.Where(x=>x.Id==1).ToList(), "Id", "TripName");
            model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x=>x.TripTypeId==1).ToList(), "Id", "ShiftTime");
            model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x=>x.TripTypeId==2).ToList(), "Id", "ShiftTime");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.EmployeeRequests.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.RequestType = data.RequestType;
                model.CompanyId = data.CompanyId;
                model.EmployeeId = data.EmployeeId;
                model.Unit = data.Unit;
                model.Department = data.Department;
                model.CostCentre = data.CostCentre;
                model.ExpenseCode = data.ExpenseCode;
                model.RequestorEmpId = data.RequestorEmpId;
                model.RequestorName = data.RequestorName;
                model.RequestorContacts = data.RequestorContacts;
                model.BookingReceivedDate = data.BookingReceivedDate;
                model.RequestTripType = data.RequestTripType;
                model.DestinationRequestMethod = data.DestinationRequestMethod;
                model.LocationType = data.LocationType;
                model.StartRequestDate = data.StartRequestDate;
                model.EndRequestDate = data.EndRequestDate;               
                model.TripType = data.TripType;
                model.ShiftType = data.ShiftType;
                model.SMSTriggeredLocation = data.SMSTriggeredLocation;
                model.PickupShiftTimeId = data.PickupShiftTimeId;
                model.DropShiftTimeId = data.DropShiftTimeId;
                ViewBag.Heading = "Update Employee Request";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.RequestType = "";
                model.EmployeeId = "";
                model.Unit = "";
                model.Department = "";
                model.CostCentre = "";
                model.ExpenseCode = "";
                model.RequestorEmpId = "";
                model.RequestorName = "";
                model.RequestorContacts = "";
                model.BookingReceivedDate = null;
                model.RequestTripType = null;
                model.DestinationRequestMethod = null;
                model.LocationType = null;
                model.StartRequestDate = null;
                model.EndRequestDate = null;
                model.TripType = null;
                model.ShiftType = null;
                model.SMSTriggeredLocation = null;
                model.PickupShiftTimeId = 0;
                model.DropShiftTimeId = 0;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create New Request";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateRequest(CreateRequestDTO model)
        {
            try
            {
                var empinfo = ent.Employees.Where(e => e.IsActive == true && e.Employee_Id == model.EmployeeId).FirstOrDefault();
                if (model.EmployeeId != null)
                {
                    if (empinfo == null)
                    {
                        TempData["errormsg"] = $"Failed. Please register as an employee first with employee id {model.EmployeeId}.";

                        return RedirectToAction("CreateRequest", new { menuId = model.MenuId });
                    }
                }
                else
                {
                    bool isCreated = await _ets.AddUpdateRequest(model);
                    if (isCreated)
                    {
                        TempData["msg"] = model.Id > 0
                            ? "Record has been updated successfully."
                            : "Record has been added successfully.";
                        return RedirectToAction("CreateRequest", new { menuId = model.MenuId });

                    }
                    else
                    {
                        TempData["errormsg"] = "Failed.";
                        

                    }
                }
                return RedirectToAction("CreateRequest", new { menuId = model.MenuId });


            }
            catch (Exception)
            {
                TempData["msg"] = "Server error";
                return RedirectToAction("CreateRequest", new { menuId = model.MenuId });
            }
            
        }
        public JsonResult GetEmployeeDetails(string id)
        {
            var employee = ent.Employees.FirstOrDefault(e => e.Employee_Id == id && e.IsActive==true);

            if (employee != null)
            {
                var employeeData = new
                {
                    firstName = employee.Employee_First_Name,
                    lastName = employee.Employee_Last_Name, 
                    gender = employee.Gender,
                    email = employee.Email,
                    guestContactNumber = employee.MobileNumber
                };

                return Json(employeeData,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }
        }

        public async Task<ActionResult> EmployeeRequestList(int menuId = 0)
        {
            try
            {
                var model = new CreateRequestDTO();

                ViewBag.menuId = menuId;

                model.EmployeeRequestList = await _ets.GetEmployeerequests();

                if (model.EmployeeRequestList == null)
                {
                    ModelState.AddModelError("", "No employee requests found.");
                }

                return View(model);
            }
            catch (Exception ex)
            {

                TempData["Errormsg"] = "Server Error: " + ex.Message;
                return RedirectToAction("EmployeeRequestList", new { menuId = menuId });
            }
        }


        public async Task<ActionResult> DeleteEmployeeRequest(int id)
        {
            try
            {
                bool isDeleted = await _ets.DeleteRequest(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("EmployeeRequestList");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExportToExcel()
        {
            // Create DataTable and add columns
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeId");
            dt.Columns.Add("Company"); // customer
            dt.Columns.Add("RequestStartDate");
            dt.Columns.Add("RequestEndDate");
            dt.Columns.Add("TripType"); // triptype
            dt.Columns.Add("ShiftType"); // tripmaster
            dt.Columns.Add("PickUpTime"); // shiftmaster id(1)
            dt.Columns.Add("DropTime"); // shiftmaster id(2)

            // Add dummy rows
            dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "2024-12-01", "2024-12-03", "BOTH", "NORMAL", "08:00", "19:30");
            dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "2024-12-02", "2", "PICKUP", "NORMAL", "09:00", "19:30");
            dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "", "2024-12-05", "DROP", "NORMAL", "10:00", "19:30");

            // Create Excel workbook using ClosedXML
            using (var workbook = new XLWorkbook())
            {
                // Add DataTable as a worksheet
                var worksheet = workbook.Worksheets.Add("Employee Request Data");
                worksheet.Cell(1, 1).InsertTable(dt);

                // Add hidden sheets for dropdown data
                var hiddenSheet = workbook.Worksheets.Add("CompanyList");
                var hiddenTripTypeListSheet = workbook.Worksheets.Add("TripTypeList");
                var hiddenShiftTypeSheet = workbook.Worksheets.Add("ShiftTypeList");
                var hiddenPickuptimeSheet = workbook.Worksheets.Add("PickuptimeList");
                var hiddenDroptimeSheet = workbook.Worksheets.Add("DroptimeList");

                // Fetch lists from the database
                var companyList = ent.Customers.Where(x => x.IsActive == true).ToList();
                var TriptypeList = ent.TripTypes.Where(x => x.TripMasterId == 1).ToList();
                var ShiftTypeList = ent.TripMasters.Where(x => x.Id == 1).ToList();
                var PickupTimeList = ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList();
                var DropTimeList = ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList();

                // Populate hidden sheets and create dropdown ranges
                int hiddenRow = 1;
                foreach (var company in companyList.OrderByDescending(x => x.Id))
                {
                    hiddenSheet.Cell(hiddenRow++, 1).Value = company.CompanyName;
                }
                var companyRange = hiddenSheet.Range($"A1:A{companyList.Count}");

                hiddenRow = 1;
                foreach (var triptype in TriptypeList.OrderByDescending(x => x.Id))
                {
                    hiddenTripTypeListSheet.Cell(hiddenRow++, 1).Value = triptype.TripTypeName;
                }
                var TripTypeRange = hiddenTripTypeListSheet.Range($"A1:A{TriptypeList.Count}");

                hiddenRow = 1;
                foreach (var shifttype in ShiftTypeList.OrderByDescending(x => x.Id))
                {
                    hiddenShiftTypeSheet.Cell(hiddenRow++, 1).Value = shifttype.TripName;
                }
                var ShiftTypeRange = hiddenShiftTypeSheet.Range($"A1:A{ShiftTypeList.Count}");

                hiddenRow = 1;
                foreach (var pickuptime in PickupTimeList.OrderByDescending(x => x.Id))
                {
                    hiddenPickuptimeSheet.Cell(hiddenRow++, 1).Value = pickuptime.ShiftTime;
                }
                var PickuptimeRange = hiddenPickuptimeSheet.Range($"A1:A{PickupTimeList.Count}");

                hiddenRow = 1;
                foreach (var droptime in DropTimeList.OrderByDescending(x => x.Id))
                {
                    hiddenDroptimeSheet.Cell(hiddenRow++, 1).Value = droptime.ShiftTime;
                }
                var DroptimeRange = hiddenDroptimeSheet.Range($"A1:A{DropTimeList.Count}");

                // Apply dropdown validations for all rows with dummy data
                int rowCount = dt.Rows.Count + 1; // Including header row
                for (int row = 2; row <= rowCount; row++)
                {
                    // Company Dropdown
                    var companyValidation = worksheet.Cell(row, 2).DataValidation;
                    companyValidation.List(companyRange);
                    companyValidation.IgnoreBlanks = true;
                    companyValidation.InCellDropdown = true;

                    // TripType Dropdown
                    var tripTypeValidation = worksheet.Cell(row, 5).DataValidation;
                    tripTypeValidation.List(TripTypeRange);
                    tripTypeValidation.IgnoreBlanks = true;
                    tripTypeValidation.InCellDropdown = true;

                    // ShiftType Dropdown
                    var shiftTypeValidation = worksheet.Cell(row, 6).DataValidation;
                    shiftTypeValidation.List(ShiftTypeRange);
                    shiftTypeValidation.IgnoreBlanks = true;
                    shiftTypeValidation.InCellDropdown = true;

                    // PickupTime Dropdown
                    var pickupTimeValidation = worksheet.Cell(row, 7).DataValidation;
                    pickupTimeValidation.List(PickuptimeRange);
                    pickupTimeValidation.IgnoreBlanks = true;
                    pickupTimeValidation.InCellDropdown = true;

                    // DropTime Dropdown
                    var dropTimeValidation = worksheet.Cell(row, 8).DataValidation;
                    dropTimeValidation.List(DroptimeRange);
                    dropTimeValidation.IgnoreBlanks = true;
                    dropTimeValidation.InCellDropdown = true;
                }

                // Set response for Excel download
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeRequestData.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ImportEmployeeRequestData(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<EmployeeRequest> emprequest = new List<EmployeeRequest>();
                        List<ExcelErrorModel> excelErrorModels = new List<ExcelErrorModel>();
                        var count = 0;

                        foreach (var row in rows)
                        {
                            count++;
                            try
                            {
                                string CompanyName = row.Cell(2).GetValue<string>();
                                string TripTypeName = row.Cell(5).GetValue<string>();
                                string ShiftTypeName = row.Cell(6).GetValue<string>();
                                string PickupShiftTimeName = row.Cell(7).GetValue<string>();
                                string DropShiftTimeName = row.Cell(8).GetValue<string>();
                                var startdatevalue = row.Cell(3).GetValue<string>();
                                DateTime? startRequestDate = string.IsNullOrEmpty(startdatevalue) ? (DateTime?)null : DateTime.Parse(startdatevalue);
                                var enddatevalue = row.Cell(4).GetValue<string>();
                                DateTime? endRequestDate = string.IsNullOrEmpty(enddatevalue) ? (DateTime?)null : DateTime.Parse(enddatevalue);
                                var employeeId = row.Cell(1).GetValue<string>() ?? string.Empty;

                               
                                if (!string.IsNullOrEmpty(employeeId))
                                {
                                    var empinfo = ent.Employees.FirstOrDefault(e => e.IsActive == true && e.Employee_Id == employeeId);

                                    if (empinfo == null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Employee Id",
                                            AffectedRow = count,
                                            Description = $"Please register as an employee first with employee id {employeeId}."
                                        });
                                    }
                                }

                                if (TripTypeName == "BOTH")
                                {
                                    if (startRequestDate == null || endRequestDate == null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = TripTypeName,
                                            AffectedRow = count,
                                            Description = $"For {TripTypeName} trip type, both Start and End dates are mandatory."
                                        });
                                    }

                                    
                                    if (endRequestDate < startRequestDate)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Request Dates",
                                            AffectedRow = count,
                                            Description = "End Request Date cannot be earlier than Start Request Date."
                                        });
                                    }
                                }

                                

                                if (TripTypeName == "PICKUP" || TripTypeName == "DROP")
                                {
                                    if (startRequestDate != null && endRequestDate != null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = TripTypeName,
                                            AffectedRow = count,
                                            Description = $"For {TripTypeName} trip type, only one of the dates (Start or End) should be filled."
                                        });
                                    }
                                    if (startRequestDate == null && endRequestDate == null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = TripTypeName,
                                            AffectedRow = count,
                                            Description = $"For {TripTypeName} trip type, at least one of the dates (Start or End) is mandatory."
                                        });
                                    }
                                }

                                
                                EmployeeRequest employeereq = new EmployeeRequest
                                {
                                    EmployeeId = employeeId,
                                    CompanyId = string.IsNullOrEmpty(CompanyName) ? 0 : ent.Customers.FirstOrDefault(x => x.CompanyName.ToLower() == CompanyName.ToLower())?.Id ?? 0,
                                    StartRequestDate = startRequestDate,
                                    EndRequestDate = endRequestDate,
                                    TripType = string.IsNullOrEmpty(TripTypeName) ? 0 : ent.TripTypes.FirstOrDefault(x => x.TripTypeName.ToLower() == TripTypeName.ToLower())?.Id ?? 0,
                                    ShiftType = string.IsNullOrEmpty(ShiftTypeName) ? 0 : ent.TripMasters.FirstOrDefault(x => x.TripName.ToLower() == ShiftTypeName.ToLower())?.Id ?? 0,
                                    PickupShiftTimeId = row.Cell(7).GetValue<int>(),
                                    DropShiftTimeId = row.Cell(8).GetValue<int>(),
                                    RequestType = "EMPLOYEE",
                                    CreatedDate = DateTime.Now
                                };

                                emprequest.Add(employeereq);
                            }
                            catch (Exception ex)
                            {
                                // Log individual row errors
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "General",
                                    AffectedRow = count,
                                    Description = $"An error occurred while processing row {count}: {ex.Message}"
                                });
                            }
                        }

                       
                        if (emprequest.Any())
                        {
                            ent.EmployeeRequests.AddRange(emprequest);
                            ent.SaveChanges();
                        }

                       
                        if (excelErrorModels.Any())
                        {
                            TempData["HasErrors"] = true;
                            TempData["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                            return RedirectToAction("CreateRequest");
                        }

                        TempData["dltmsg"] = "Data imported successfully!";
                        return RedirectToAction("EmployeeRequestList");
                    }
                }

                ViewBag.Message = "Please select an Excel file to import.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }


        public ActionResult Routing()
        {
            try
            {
                var model = new RoutingDTO();
                model.Customers = new SelectList(ent.Customers.Where(a => a.IsActive).OrderByDescending(a => a.Id).ToList(), "Id", "OrgName");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
                model.ShiftTypes = new SelectList(ent.TripMasters.Where(x => x.Id == 1).ToList(), "Id", "TripName");
                model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
                model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
                model.Zones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
                ViewBag.BtnTXT = "Create Routing";
                ViewBag.Heading = "Add Shift Time";
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error + " + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Routing(RoutingDTO model)
        {
            try
            {
                model.Customers = new SelectList(ent.Customers.Where(a => a.IsActive).OrderByDescending(a => a.Id).ToList(), "Id", "OrgName");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
                model.ShiftTypes = new SelectList(ent.TripMasters.Where(x => x.Id == 1).ToList(), "Id", "TripName");
                model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
                model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
                model.Zones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
                
                if(model.StartDate != null && model.EndDate != null)
                {
                    var EmployeeReqList = ent.EmployeeRequests.Where(x => x.StartRequestDate <= model.StartDate && x.EndRequestDate <= model.EndDate).ToList();   
                    if(EmployeeReqList != null)
                    {
                        foreach(var item in EmployeeReqList)
                        {
                           // ent.CompanyZoneHomeRoutes.Where()
                           
                        }
                    }
                }
                ViewBag.BtnTXT = "Create Routing";
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error + " + ex.Message);
            }
        }
    }
}