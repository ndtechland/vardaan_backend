using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Model;
using OfficeOpenXml.FormulaParsing.Exceptions;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Vardaan.Services.IContract;
using Vardaan.Services.IContractApi;
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
            model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
            model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
            model.ShiftTypes = new SelectList(ent.TripMasters.Where(x => x.Id == 1).ToList(), "Id", "TripName");
            model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
            model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
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
                if (empinfo == null)
                {
                    
                        TempData["errormsg"] = $"Failed. Please register as an employee first with employee id {model.EmployeeId}.";

                        return RedirectToAction("CreateRequest", new { menuId = model.MenuId });
                    
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
            var employee = ent.Employees.FirstOrDefault(e => e.Employee_Id == id && e.IsActive == true);

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

                return Json(employeeData, JsonRequestBehavior.AllowGet);
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

            
            int userId = int.Parse(User.Identity.Name);
            string companyName = "";
            List<Customer> companyList = new List<Customer>();

            if (Session["IsAuth"] != null && Convert.ToBoolean(Session["IsAuth"]) == false)
            {
                companyList = ent.Customers.Where(x => x.IsActive == true).ToList();
                companyName = companyList.FirstOrDefault()?.CompanyName ?? "Unknown Company";


                dt.Rows.Add("9898989898", "Dummy pvt ltd", "2025-01-14", "2025-02-14", "BOTH", "NORMAL", "08:00", "07:00");
                dt.Rows.Add("9898989898", "Dummy pvt ltd", "2025-03-14", "2025-04-14", "PICKUP", "NORMAL", "09:00", "");
                dt.Rows.Add("9898989898", "Dummy pvt ltd", "2025 -05-14", "2025-06-14", "DROP", "NORMAL", "", "07:00");
            }
            else
            {
                var empinfo = ent.Employees.FirstOrDefault(e => e.Id == userId);

                if (empinfo != null)
                {
                    companyList = ent.Customers.Where(x => x.IsActive == true && x.Id == empinfo.Company_Id).ToList();
                    companyName = companyList.FirstOrDefault()?.CompanyName ?? "Unknown Company";

                    
                    dt.Rows.Add("9898989898", companyName, "2025-01-14", "2025-02-14", "BOTH", "NORMAL", "08:00", "07:00");
                    dt.Rows.Add("9898989898", companyName, "2025-03-14", "PICKUP", "NORMAL", "09:00", "");
                    dt.Rows.Add("9898989898", companyName, "2025-05-14", "2025-06-14", "DROP", "NORMAL", "", "07:00");
                }
                else
                {
                    companyList = new List<Customer>();
                }
            }

            // Optionally handle cases where the DataTable is empty for additional logic.

            //dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "2024-12-01", "2024-12-03", "BOTH", "NORMAL", "08:00", "19:30");
            //dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "2024-12-02", "2", "PICKUP", "NORMAL", "09:00", "19:30");
            //dt.Rows.Add("9898989898", "Test Vardaan car rental pvt ltd", "", "2024-12-05", "DROP", "NORMAL", "10:00", "19:30");

            // Create Excel workbook using ClosedXML
            using (var workbook = new XLWorkbook())
            {
                // Add DataTable as a worksheet
                var worksheet = workbook.Worksheets.Add("Employee Request Data");
                worksheet.Cell(1, 1).InsertTable(dt);

                // Add hidden sheets for dropdown data
                //var hiddenSheet = workbook.Worksheets.Add("CompanyList");
                //var hiddenTripTypeListSheet = workbook.Worksheets.Add("TripTypeList");
                //var hiddenShiftTypeSheet = workbook.Worksheets.Add("ShiftTypeList");
                //var hiddenPickuptimeSheet = workbook.Worksheets.Add("PickuptimeList");
                //var hiddenDroptimeSheet = workbook.Worksheets.Add("DroptimeList");

                //// Fetch lists from the database
                //var companyList = ent.Customers.Where(x => x.IsActive == true).ToList();
                //var TriptypeList = ent.TripTypes.Where(x => x.TripMasterId == 1).ToList();
                //var ShiftTypeList = ent.TripMasters.Where(x => x.Id == 1).ToList();
                //var PickupTimeList = ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList();
                //var DropTimeList = ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList();

                //// Populate hidden sheets and create dropdown ranges
                //int hiddenRow = 1;
                //foreach (var company in companyList.OrderByDescending(x => x.Id))
                //{
                //    hiddenSheet.Cell(hiddenRow++, 1).Value = company.CompanyName;
                //}
                //var companyRange = hiddenSheet.Range($"A1:A{companyList.Count}");

                //hiddenRow = 1;
                //foreach (var triptype in TriptypeList.OrderByDescending(x => x.Id))
                //{
                //    hiddenTripTypeListSheet.Cell(hiddenRow++, 1).Value = triptype.TripTypeName;
                //}
                //var TripTypeRange = hiddenTripTypeListSheet.Range($"A1:A{TriptypeList.Count}");

                //hiddenRow = 1;
                //foreach (var shifttype in ShiftTypeList.OrderByDescending(x => x.Id))
                //{
                //    hiddenShiftTypeSheet.Cell(hiddenRow++, 1).Value = shifttype.TripName;
                //}
                //var ShiftTypeRange = hiddenShiftTypeSheet.Range($"A1:A{ShiftTypeList.Count}");

                //hiddenRow = 1;
                //foreach (var pickuptime in PickupTimeList.OrderByDescending(x => x.Id))
                //{
                //    hiddenPickuptimeSheet.Cell(hiddenRow++, 1).Value = pickuptime.ShiftTime;
                //}
                //var PickuptimeRange = hiddenPickuptimeSheet.Range($"A1:A{PickupTimeList.Count}");

                //hiddenRow = 1;
                //foreach (var droptime in DropTimeList.OrderByDescending(x => x.Id))
                //{
                //    hiddenDroptimeSheet.Cell(hiddenRow++, 1).Value = droptime.ShiftTime;
                //}
                //var DroptimeRange = hiddenDroptimeSheet.Range($"A1:A{DropTimeList.Count}");

                //// Apply dropdown validations for all rows with dummy data
                //int rowCount = dt.Rows.Count + 1; // Including header row
                //for (int row = 2; row <= rowCount; row++)
                //{
                //    // Company Dropdown
                //    var companyValidation = worksheet.Cell(row, 2).DataValidation;
                //    companyValidation.List(companyRange);
                //    companyValidation.IgnoreBlanks = true;
                //    companyValidation.InCellDropdown = true;

                //    // TripType Dropdown
                //    var tripTypeValidation = worksheet.Cell(row, 5).DataValidation;
                //    tripTypeValidation.List(TripTypeRange);
                //    tripTypeValidation.IgnoreBlanks = true;
                //    tripTypeValidation.InCellDropdown = true;

                //    // ShiftType Dropdown
                //    var shiftTypeValidation = worksheet.Cell(row, 6).DataValidation;
                //    shiftTypeValidation.List(ShiftTypeRange);
                //    shiftTypeValidation.IgnoreBlanks = true;
                //    shiftTypeValidation.InCellDropdown = true;

                //    // PickupTime Dropdown
                //    var pickupTimeValidation = worksheet.Cell(row, 7).DataValidation;
                //    pickupTimeValidation.List(PickuptimeRange);
                //    pickupTimeValidation.IgnoreBlanks = true;
                //    pickupTimeValidation.InCellDropdown = true;

                //    // DropTime Dropdown
                //    var dropTimeValidation = worksheet.Cell(row, 8).DataValidation;
                //    dropTimeValidation.List(DroptimeRange);
                //    dropTimeValidation.IgnoreBlanks = true;
                //    dropTimeValidation.InCellDropdown = true;
                //}

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
                                //string PickupShiftTimeName = row.Cell(7).GetValue<string>();
                                string PickupShiftTimeName = row.Cell(7).GetValue<string>() ?? null;

                                // Parse the string into a DateTime object
                                DateTime dateTimeValue;
                                if (DateTime.TryParse(PickupShiftTimeName, out dateTimeValue))
                                {
                                    // Extract the time in the desired format (e.g., "4:00")
                                    
                                     PickupShiftTimeName = dateTimeValue.ToString("HH:mm");

                                }
                                string DropShiftTimeName = row.Cell(8).GetValue<string>() ?? null;
                                if (DateTime.TryParse(DropShiftTimeName, out dateTimeValue))
                                {
                                    // Extract the time in the desired format (e.g., "4:00")
                                    DropShiftTimeName = dateTimeValue.ToString("HH:mm");

                                }
                                var startdatevalue = row.Cell(3).GetValue<string>();
                                DateTime? startRequestDate = string.IsNullOrEmpty(startdatevalue) ? (DateTime?)null : DateTime.Parse(startdatevalue);
                                var enddatevalue = row.Cell(4).GetValue<string>();
                                DateTime? endRequestDate = string.IsNullOrEmpty(enddatevalue) ? (DateTime?)null : DateTime.Parse(enddatevalue);
                                var employeeId = row.Cell(1).GetValue<string>() ?? string.Empty;

                                if (TripTypeName.ToLower() == "pickup" || TripTypeName.ToLower() == "both")
                                {
                                    if (!string.IsNullOrEmpty(PickupShiftTimeName))
                                    {
                                        var shifttimeinfo = ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList();

                                        bool isValidShiftTime = shifttimeinfo.Any(x => x.ShiftTime.Equals(PickupShiftTimeName, StringComparison.OrdinalIgnoreCase));

                                        if (!isValidShiftTime)
                                        {
                                            excelErrorModels.Add(new ExcelErrorModel
                                            {
                                                ErrorType = "Pickup Shift Time",
                                                AffectedRow = count,
                                                Description = $"The pickup shift time {PickupShiftTimeName} does not exist in the master table. Please provide a valid shift time."
                                            });
                                        }
                                    }
                                    else
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Pickup Shift Time",
                                            AffectedRow = count, 
                                            Description = "Pickup shift time cannot be empty. Please provide a valid shift time."
                                        });
                                    }
                                }
                                if (TripTypeName.ToLower() == "drop" || TripTypeName.ToLower() == "both")
                                {
                                    if (!string.IsNullOrEmpty(DropShiftTimeName))
                                    {
                                        var shifttimeinfo = ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList();

                                        bool isValidShiftTime = shifttimeinfo.Any(x => x.ShiftTime.Equals(DropShiftTimeName, StringComparison.OrdinalIgnoreCase));

                                        if (!isValidShiftTime)
                                        {
                                            excelErrorModels.Add(new ExcelErrorModel
                                            {
                                                ErrorType = "Drop Shift Time",
                                                AffectedRow = count,
                                                Description = $"The Drop shift time {DropShiftTimeName} does not exist in the master table. Please provide a valid shift time."
                                            });
                                        }
                                    }
                                    else
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Drop Shift Time",
                                            AffectedRow = count,
                                            Description = "Drop shift time cannot be empty. Please provide a valid shift time."
                                        });
                                    }
                                }

                                var validTripTypes = new List<string> { "both", "pickup", "drop" };

                                // Validate registration type
                                if (!validTripTypes.Contains(TripTypeName.ToLower()))
                                {
                                    var validTypesHint = string.Join(", ", validTripTypes);

                                    excelErrorModels.Add(new ExcelErrorModel
                                    {
                                        ErrorType = "Trip Type",
                                        AffectedRow = count,
                                        Description = $"Kindly check the employee Trip Type: {TripTypeName}. " +
                                                      $"Valid types are: {validTypesHint}."
                                    });
                                }
                                var validShiftType = "normal";

                                // Validate shift type
                                if (!validShiftType.Contains(validShiftType.ToLower()))
                                {
                                    var validTypesHint = string.Join(", ", validShiftType);

                                    excelErrorModels.Add(new ExcelErrorModel
                                    {
                                        ErrorType = "Shift Type",
                                        AffectedRow = count,
                                        Description = $"Kindly check the Shift Type: {validShiftType}. " +
                                                      $"Valid types are: {validTypesHint}."
                                    });
                                }


                                if (!string.IsNullOrEmpty(employeeId))
                                {
                                    var empinfo = ent.Employees.FirstOrDefault(e => e.IsActive == true && e.Employee_Id == employeeId);

                                    
                                    if (empinfo == null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Employee Id",
                                            AffectedRow = count, 
                                            Description = $"Please register as an employee first with Employee ID {employeeId}."
                                        });
                                    }
                                }
                                else
                                {
                                    excelErrorModels.Add(new ExcelErrorModel
                                    {
                                        ErrorType = "Employee Id",
                                        AffectedRow = count, 
                                        Description = "Employee ID cannot be empty. Please provide a valid Employee ID."
                                    });
                                }


                                if (startRequestDate == null || endRequestDate == null)
                                {
                                    excelErrorModels.Add(new ExcelErrorModel
                                    {
                                        ErrorType = TripTypeName,
                                        AffectedRow = count,
                                        Description = $"For {TripTypeName} trip type, both Start and End dates are mandatory."
                                    });
                                }
                                if (startRequestDate < DateTime.Now.Date || endRequestDate < DateTime.Now.Date) // Use .Date to compare dates only
                                {
                                    if (startRequestDate < DateTime.Now.Date && endRequestDate < DateTime.Now.Date)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Dates",
                                            AffectedRow = count,
                                            Description = $"Start Date and End Date must not be earlier than today."
                                        });
                                    }
                                    else if (startRequestDate < DateTime.Now.Date)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Start Date",
                                            AffectedRow = count,
                                            Description = $"Start Date must not be earlier than today."
                                        });
                                    }
                                    else if (endRequestDate < DateTime.Now.Date)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "End Date",
                                            AffectedRow = count,
                                            Description = $"End Date must not be earlier than today."
                                        });
                                    }
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

                                if (TripTypeName.ToLower() == "pickup" || TripTypeName.ToLower() == "drop")
                                {
                                    if (!string.IsNullOrEmpty(PickupShiftTimeName) && !string.IsNullOrEmpty(DropShiftTimeName))
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = TripTypeName,
                                            AffectedRow = count,
                                            Description = $"For {TripTypeName} trip type, only one of the shift time {TripTypeName} should be filled."
                                        });
                                    }
                                    if (string.IsNullOrEmpty(PickupShiftTimeName) && string.IsNullOrEmpty(DropShiftTimeName))
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = TripTypeName,
                                            AffectedRow = count,
                                            Description = $"For {TripTypeName} trip type, at least one of the shift time ({TripTypeName}) is mandatory."
                                        });
                                    }
                                }

                                if(!string.IsNullOrEmpty(CompanyName))
                                {
                                    var cominfo = ent.Customers.Where(c => c.CompanyName.ToLower() == CompanyName.ToLower()).FirstOrDefault();
                                    if(cominfo==null)
                                    {
                                        excelErrorModels.Add(new ExcelErrorModel
                                        {
                                            ErrorType = "Company Name",
                                            AffectedRow = count,
                                            Description = $"Company {CompanyName} not exist in the database."
                                        });
                                    }
                                }
                                else
                                {
                                    excelErrorModels.Add(new ExcelErrorModel
                                    {
                                        ErrorType = "Company Name",
                                        AffectedRow = count,
                                        Description = $"Company cannot be empty."
                                    });
                                }

                                if (excelErrorModels.Any(e => e.AffectedRow == count)) continue;
                                EmployeeRequest employeereq = new EmployeeRequest
                                {
                                    EmployeeId = employeeId,
                                    CompanyId = string.IsNullOrEmpty(CompanyName) ? 0 : ent.Customers.FirstOrDefault(x => x.CompanyName.ToLower() == CompanyName.ToLower())?.Id ?? 0,
                                    StartRequestDate = startRequestDate,
                                    EndRequestDate = endRequestDate,
                                    TripType = string.IsNullOrEmpty(TripTypeName) ? 0 : ent.TripTypes.FirstOrDefault(x => x.TripTypeName.ToLower() == TripTypeName.ToLower())?.Id ?? 0,
                                    ShiftType = string.IsNullOrEmpty(ShiftTypeName) ? 0 : ent.TripMasters.FirstOrDefault(x => x.TripName.ToLower() == ShiftTypeName.ToLower())?.Id ?? 0,
                                    PickupShiftTimeId = string.IsNullOrEmpty(PickupShiftTimeName) ? 0 : ent.ShiftMasters.Where(x=>x.TripTypeId==1).FirstOrDefault(x => x.ShiftTime.ToLower() == PickupShiftTimeName.ToLower())?.Id ?? 0,
                                    DropShiftTimeId = string.IsNullOrEmpty(DropShiftTimeName) ? 0 : ent.ShiftMasters.Where(x => x.TripTypeId == 2).FirstOrDefault(x => x.ShiftTime.ToLower() == DropShiftTimeName.ToLower())?.Id ?? 0,
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
                        if (excelErrorModels.Any())
                        {
                            TempData["HasErrors"] = true;
                            TempData["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                            return RedirectToAction("CreateRequest");
                        }

                        if (emprequest.Any())
                        {
                            ent.EmployeeRequests.AddRange(emprequest);
                            ent.SaveChanges();
                        }

                        TempData["Message"] = "Data imported successfully!";
                        return RedirectToAction("CreateRequest");
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
        [HttpGet]
        public ActionResult RoutingList()
        {
            try
            {
                //List<EmployeeGroup> employeelist = new List<EmployeeGroup>();
                Dictionary<string, List<EmployeeGroup>> dict = new Dictionary<string, List<EmployeeGroup>>();

                 var employeelistJson = Session["AllRoutes"].ToString();
                 dict = new JavaScriptSerializer().Deserialize<Dictionary<string, List<EmployeeGroup>>>(employeelistJson);
                return View(dict);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult RoutingList(List<EmployeeGroup> employeelist)
        {
            try
            {
                
                return View();
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
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
                ViewBag.Heading = "Create Routing";
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

                if (model.StartDate != null && model.EndDate != null)
                {
                    var EmployeeReqList = (from empr in ent.EmployeeRequests
                                           join emp in ent.Employees on empr.EmployeeId equals emp.Employee_Id
                                           where empr.StartRequestDate <= model.StartDate && empr.EndRequestDate >= model.EndDate
                                           select new
                                           {
                                               Employee = emp
                                           }).ToList();
                    var emplist = EmployeeReqList.Select(e => e.Employee).ToList();
                    var employeelist = TransformData(emplist);
                    double threshold = 0.1; // Approx. 100 meters in degrees
                    int groupCounter = 0;

                    foreach (var emp in employeelist)
                    {
                        if (emp.Group == null) // Only group ungrouped employees
                        {
                            // Check for matches
                            bool hasMatch = false;

                            foreach (var other in employeelist)
                            {
                                if (other.Group == null && CalculateDistance(emp.Latitude, emp.Longitude, other.Latitude, other.Longitude) <= threshold)
                                {
                                    hasMatch = true;
                                    if (emp.Group == null)
                                    {
                                        groupCounter++;
                                        emp.Group = groupCounter.ToString(); // Assign a numbered group
                                    }
                                    other.Group = emp.Group; // Assign the same group to the matching employee
                                }
                            }

                            // If no match is found, assign a GUID
                            if (!hasMatch && emp.Group == null)
                            {
                                emp.Group = Guid.NewGuid().ToString();
                            }
                        }
                    }
                    Dictionary<string, List<EmployeeGroup>> dict = new Dictionary<string, List<EmployeeGroup>>();


                    var groupedData = employeelist.GroupBy(x => x.Group);

                    foreach (var group in groupedData)
                    {
                        dict[group.Key] = group.ToList();
                    }

                    var JsonAllRoutes = new JavaScriptSerializer().Serialize(dict);

                    Session["AllRoutes"] = JsonAllRoutes;

                    return RedirectToAction("RoutingList","ETS");
                }



                #region no use
                //if (EmployeeReqList != null && EmployeeReqList.Any())
                //    {
                // List<dynamic> AllRoutes = new List<dynamic>();
                //        int cabCounter = 1001; // Initialize counter for unique cab numbers

                // Define the proximity radius for 100 meters
                //       double proximityRadiusLat = 100 / 111000.0; // ~0.0009 degrees for latitude

                //foreach (var request in EmployeeReqList)
                //{
                //    var Emp = ent.Employees.FirstOrDefault(x => x.Employee_Id == request.EmployeeId);
                //    var EmpCompany = ent.Customers.FirstOrDefault(x => x.Id == model.Company_Id);

                //    if (Emp != null && EmpCompany != null)
                //    {
                //        // Employee latitude and longitude
                //        double empLat = (double)Emp.Latitude;
                //        double empLong = (double)Emp.Longitude;

                //        // Adjust longitude proximity radius for employee's latitude
                //        double proximityRadiusLong = 100 / (111000.0 * Math.Cos(DegreesToRadians(empLat)));

                //        // Get matching routes
                //        var MatchingRoutes = (from cz in ent.CompanyZones
                //                              join chz in ent.CompanyZoneHomeRoutes on cz.Id equals chz.CompanyZoneId
                //                              where cz.CompanyId == EmpCompany.Id &&
                //                                    cz.Id == Emp.PrimaryFacilityZone
                //                              select new
                //                              {
                //                                  RouteId = chz.Id.ToString(),
                //                                  RouteName = chz.HomeRouteName,
                //                                  Latitude = chz.Latitude,
                //                                  Longitude = chz.Longitude
                //                              }).ToList();

                //        bool isAssigned = false;

                //        if (MatchingRoutes.Any())
                //        {
                //            foreach (var route in MatchingRoutes)
                //            {
                //                double routeLat = (double)route.Latitude;
                //                double routeLong = (double)route.Longitude;

                //                // Check if the route is within the 100-meter proximity
                //                if (Math.Abs(routeLat - empLat) <= proximityRadiusLat &&
                //                    Math.Abs(routeLong - empLong) <= proximityRadiusLong)
                //                {
                //                    // Find or create a group for this route
                //                    var CurrentGroup = AllRoutes
                //                        .FirstOrDefault(r => r.RouteId == route.RouteId)?.Employees as List<dynamic>;

                //                    if (CurrentGroup == null)
                //                    {
                //                        CurrentGroup = new List<dynamic>();
                //                        AllRoutes.Add(new
                //                        {
                //                            RouteId = route.RouteId,
                //                            RouteName = route.RouteName,
                //                            Employees = CurrentGroup
                //                        });
                //                    }

                //                    CurrentGroup.Add(new
                //                    {
                //                        EmployeeId = Emp.Employee_Id,
                //                        EmployeeName = $"{Emp.Employee_First_Name} {Emp.Employee_Middle_Name} {Emp.Employee_Last_Name}",
                //                        Latitude = empLat,
                //                        Longitude = empLong
                //                    });

                //                    // Assign cab type based on group size
                //                    string cabNumber = $"Dummy10GB{cabCounter++}";
                //                    if (CurrentGroup.Count <= 4)
                //                    {
                //                        CurrentGroup.ForEach(e => e.CabType = "Cab-4");
                //                        CurrentGroup.ForEach(e => e.CabNumber = cabNumber);
                //                    }
                //                    else if (CurrentGroup.Count <= 6)
                //                    {
                //                        CurrentGroup.ForEach(e => e.CabType = "Cab-6");
                //                        CurrentGroup.ForEach(e => e.CabNumber = cabNumber);
                //                    }

                //                    isAssigned = true;
                //                    break;
                //                }
                //            }
                //        }

                //        // If no route matches, assign a new cab (4-seater) for the employee
                //        if (!isAssigned)
                //        {
                //            string cabNumber = $"Dummy10GB{cabCounter++}";
                //            AllRoutes.Add(new
                //            {
                //                RouteId = Guid.NewGuid().ToString(), // Use a unique identifier for this route
                //                RouteName = $"Individual Route for {Emp.Employee_Id}",
                //                Employees = new List<dynamic>
                //{
                //    new
                //    {
                //        EmployeeId = Emp.Employee_Id,
                //        EmployeeName = $"{Emp.Employee_First_Name} {Emp.Employee_Middle_Name} {Emp.Employee_Last_Name}",
                //        Latitude = empLat,
                //        Longitude = empLong,
                //        CabType = "Cab-4",
                //        CabNumber = cabNumber
                //    }
                //}
                //            });
                //        }
                //    }
                //}

                // Serialize all routes to JSON for further processing or saving
                //var JsonAllRoutes = new JavaScriptSerializer().Serialize(AllRoutes);

                // AllRoutes now contains the cab assignments.
                // You can save these or process them further as needed.
                //}
                //}


                #endregion


                ViewBag.BtnTXT = "Create Routing";
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error + " + ex.Message);
            }
        }
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of Earth in km
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private List<EmployeeGroup> TransformData(List<VardaanCab.DataAccessLayer.DataLayer.Employee> originalData)
        {
            return originalData.Select(c => new EmployeeGroup
            {
                Id = c.Id,
                Employee_Id = c.Employee_Id,
                Gender = c.Gender,
                CompanyName = ent.Customers.Where(x =>x.Id == c.Company_Id).FirstOrDefault().OrgName,
                Latitude = (double)c.Latitude,  
                Longitude = (double)c.Longitude,
                PickupandDropAddress = c.EmployeeGeoCode,
                Name = $"{c.Employee_First_Name} {c.Employee_Middle_Name} {c.Employee_Last_Name}",
                ZoneWise = ent.CompanyZones.Where(x =>x.Id == c.PrimaryFacilityZone).FirstOrDefault().CompanyZone1,
                ZoneHomeWise = ent.CompanyZoneHomeRoutes.Where(x => x.Id == c.HomeRouteName).FirstOrDefault().HomeRouteName,
                DestinationAreaWise = ent.EmployeeDestinationAreas.Where(x => x.Id == c.EmployeeDestinationArea).FirstOrDefault().DestinationAreaName  
            }).ToList();
        }
        static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Radius of the Earth in kilometers
            const double R = 6371;

            // Convert degrees to radians
            lat1 = DegreesToRadians(lat1);
            lon1 = DegreesToRadians(lon1);
            lat2 = DegreesToRadians(lat2);
            lon2 = DegreesToRadians(lon2);

            // Haversine formula
            double dlat = lat2 - lat1;
            double dlon = lon2 - lon1;
            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dlon / 2) * Math.Sin(dlon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in kilometers
            double distance = R * c;

            // Convert distance to meters
            return distance * 1000;
        }
        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}