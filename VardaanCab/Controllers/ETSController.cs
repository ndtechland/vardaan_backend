using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employee Request Data");
                worksheet.Cell(1, 1).InsertTable(dt);
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

                                if (!string.IsNullOrEmpty(CompanyName))
                                {
                                    var cominfo = ent.Customers.Where(c => c.CompanyName.ToLower() == CompanyName.ToLower()).FirstOrDefault();
                                    if (cominfo == null)
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
                                    PickupShiftTimeId = string.IsNullOrEmpty(PickupShiftTimeName) ? 0 : ent.ShiftMasters.Where(x => x.TripTypeId == 1).FirstOrDefault(x => x.ShiftTime.ToLower() == PickupShiftTimeName.ToLower())?.Id ?? 0,
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
                //ViewBag.DriverItems = ent.Drivers.Where(x => x.IsActive == true).Select(x => new SelectListItem
                //{
                //    Text = x.DriverName,
                //    Value = x.Id.ToString()
                //}).ToList();
                ViewBag.DriverItems = (from d in ent.Drivers
                                       join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                                       where dl.IsActive == true
                                       orderby dl.Id descending
                                       select new SelectListItem
                                       {
                                           Text = d.DriverName,
                                           Value = d.Id.ToString()
                                       }
                                       ).ToList();
                ViewBag.VehicleNumItems = (from c in ent.Cabs
                                           join dl in ent.DriverLoginHistories on c.VehicleNumber equals dl.VehicleNumber
                                           where dl.IsActive == true && c.IsLogin == true
                                           orderby dl.Id descending
                                           select new SelectListItem
                                           {
                                               Text = c.VehicleNumber,
                                               Value = c.Id.ToString()
                                           }
                                       ).ToList();
                ViewBag.DeviceIdItems = (from di in ent.DriverDeviceIds
                                         join dl in ent.DriverLoginHistories on di.Driver_Id equals dl.DriverId
                                         where dl.IsActive == true
                                         orderby dl.Id descending
                                         select new SelectListItem
                                         {
                                             Text = di.Id.ToString(),
                                             Value = di.Id.ToString()
                                         }
                                       ).ToList();
                ViewBag.EscortItems = ent.Escorts.Where(x => x.IsActive == true).Select(x => new SelectListItem
                {
                    Text = x.EscortName,
                    Value = x.Id.ToString()
                }).ToList();
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
                model.Customers = new SelectList(ent.Customers.Where(a => a.IsActive).OrderByDescending(a => a.Id)
        .Select(a => new
        {
            Id = a.Id,
            DisplayName = a.CompanyName + " (" + a.OrgName + ")"
        })
        .ToList(),
    "Id",
    "DisplayName");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
                model.ShiftTypes = new SelectList(ent.TripMasters.Where(x => x.Id == 1).ToList(), "Id", "TripName");
                model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
                model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
                model.Zones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
                model.vehicleCapacity = new SelectList(ent.VehicleCapacities.ToList(), "Id", "Capacity");
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
                InitializeDropdowns(model);
                if (model.StartDate != null && model.EndDate != null)
                {
                    var employeeRequestList = GetEmployeeRequests(model);
                    var employeeRequestList1 = GetEmployeeRequestsUsingADO(model);
                    if (employeeRequestList.Count > 0)
                    {
                        string combinedString = model.PickupShiftid != null ? string.Join(",", model.PickupShiftid) : string.Empty;
                        string DropcombinedString = model.DropShiftid != null ? string.Join(",", model.DropShiftid) : string.Empty;
                        string VehicleTypecombinedString = model.Vehicle_Type != null ? string.Join(",", model.Vehicle_Type) : string.Empty;
                        var data = new Routing()
                        {
                            RouteStartDate = model.StartDate,
                            RouteEndDate = model.EndDate,
                            Company_Id = model.Company_Id,
                            TripType = model.Trip_Type,
                            PickupShiftTime = combinedString ?? null,
                            DropShiftTime = DropcombinedString ?? null,
                            AdhocShiftTime = model.Adhoc_Shift_Time,
                            VehicleType = VehicleTypecombinedString ?? null,
                            Routingtype = model.Routing_Type,
                            RoutingOption = model.Routing_Options
                        };
                        ent.Routings.Add(data);
                        ent.SaveChanges();
                        List<EmployeeGroup> employeeGroups = GroupEmployeesByZoneAndArea(employeeRequestList);
                        Thread thread = new Thread(() => BackgroudJob(employeeGroups, model, employeeRequestList1));
                        thread.IsBackground = true;
                        thread.Start();
                        var groupedData = employeeGroups.GroupBy(x => x.Group);
                        var routeDictionary = groupedData.ToDictionary(group => group.Key, group => group.ToList());
                        var jsonRoutes = new JavaScriptSerializer().Serialize(routeDictionary);
                        Session["AllRoutes"] = jsonRoutes;

                        return RedirectToAction("RoutingList", "ETS");
                    }
                }

                ViewBag.BtnTXT = "Create Routing";
                TempData["errormsg"] = "Data not found between these dates...!";
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error: " + ex.Message);
            }
        }

        public void BackgroudJob(List<EmployeeGroup> employeeGroups, RoutingDTO model, List<EmployeeRequest> employeeRequestList1)
        {
            try
            {
                foreach (var item in employeeGroups)
                {
                    AllRoute allRoute = new AllRoute()
                    {
                        Routing_Id = ent.Routings.OrderByDescending(x => x.ID).FirstOrDefault().ID,
                        RouteId = Convert.ToInt64(item.Group),
                        Employee_Id = item.Employee_Id,
                        RouteNameId = ent.CompanyZones.Where(x => x.CompanyZone1 == item.ZoneWise).FirstOrDefault().Id,
                        AvailableSeats = item.missingEmployees,
                        CabNumber = item.CabNumber
                    };
                    ent.AllRoutes.Add(allRoute);
                    ent.SaveChanges();
                    var Triptypename = ent.TripTypes.Where(x => x.Id == model.Trip_Type).First().TripTypeName;
                    ///Store  two time entries for both trip type and then Pickup and Drop single time 
                    if (Triptypename?.ToLower() == "both")
                    {
                        var cab = ent.Cabs.FirstOrDefault(x => x.VehicleNumber == item.CabNumber);
                        var employeeRequest = employeeRequestList1.FirstOrDefault(x => x.EmployeeId == item.Employee_Id);
                        var employee = ent.Employees.FirstOrDefault(x => x.Employee_Id == item.Employee_Id);
                        var customer = ent.Customers.FirstOrDefault(x => x.Id == model.Company_Id);

                        if (employee == null || customer == null)
                        {
                            // Log missing data and return to avoid inserting incomplete records
                            return;
                        }

                        for (DateTime date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
                        {
                            var picdata = new PickupAndDropLocationData()
                            {
                                AllRoute_Id = allRoute?.Id ?? 0,
                                Employee_Id = item.Employee_Id,
                                RouteDate = date,
                                CabId = cab == null ? 0 : cab?.Id, // Avoid null reference
                                TripTypeid = ent.TripTypes.Where(x => x.TripTypeName.ToUpper() == "PICKUP" && x.TripMasterId == 1).First().Id,
                                PickupShiftId = employeeRequest?.PickupShiftTimeId ?? 0,
                                DropShiftId = employeeRequest?.DropShiftTimeId ?? 0,
                                CompanyId = model.Company_Id,
                                PickupLocation = employee?.EmployeeCurrentAddress ?? "Unknown",
                                DropLocation = customer?.GeoLocation ?? "Unknown",
                                DriverId = 0,
                                CreatedDate = DateTime.Now,
                                IsActive = true
                            };

                            var dropdata2 = new PickupAndDropLocationData()
                            {
                                AllRoute_Id = allRoute?.Id ?? 0,
                                Employee_Id = item.Employee_Id,
                                RouteDate = date,
                                CabId = cab == null ? 0 : cab?.Id,
                                TripTypeid = ent.TripTypes.Where(x => x.TripTypeName.ToUpper() == "DROP" && x.TripMasterId == 1).First().Id,
                                PickupShiftId = employeeRequest?.PickupShiftTimeId ?? 0,
                                DropShiftId = employeeRequest?.DropShiftTimeId ?? 0,
                                CompanyId = model.Company_Id,
                                PickupLocation = customer?.GeoLocation ?? "Unknown", // Swapped for drop trip
                                DropLocation = employee?.EmployeeCurrentAddress ?? "Unknown",
                                DriverId = 0,
                                CreatedDate = DateTime.Now,
                                IsActive = true
                            };

                            ent.PickupAndDropLocationDatas.Add(picdata);
                            ent.PickupAndDropLocationDatas.Add(dropdata2);
                            ent.SaveChanges();
                        }

                        //ent.SaveChanges(); // Save once per iteration (better performance)
                    }
                    else
                    {
                        for (DateTime date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
                        {
                            PickupAndDropLocationData picdropdata = new PickupAndDropLocationData()
                            {
                                AllRoute_Id = allRoute.Id,
                                RouteDate = date,
                                Employee_Id = item.Employee_Id,
                                CabId = ent.Cabs.Where(x => x.VehicleNumber == item.CabNumber).FirstOrDefault()?.Id ?? 0, // Handle null cases
                                TripTypeid = model.Trip_Type,
                                PickupShiftId = employeeRequestList1.Where(x => x.EmployeeId == item.Employee_Id).FirstOrDefault()?.PickupShiftTimeId ?? 0,
                                DropShiftId = employeeRequestList1.Where(x => x.EmployeeId == item.Employee_Id).FirstOrDefault()?.DropShiftTimeId ?? 0,
                                CompanyId = model.Company_Id,
                                PickupLocation = Triptypename == "pickup"
                                    ? ent.Employees.Where(x => x.Employee_Id == item.Employee_Id).FirstOrDefault()?.EmployeeCurrentAddress
                                    : ent.Customers.Where(x => x.Id == model.Company_Id).FirstOrDefault()?.GeoLocation,
                                DropLocation = Triptypename == "pickup"
                                    ? ent.Customers.Where(x => x.Id == model.Company_Id).FirstOrDefault()?.GeoLocation
                                    : ent.Employees.Where(x => x.Employee_Id == item.Employee_Id).FirstOrDefault()?.EmployeeCurrentAddress,
                                DriverId = 0,
                                CreatedDate = DateTime.Now,
                                IsActive = true
                            };

                            ent.PickupAndDropLocationDatas.Add(picdropdata);
                            ent.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }

        public List<EmployeeRequest> GetEmployeeRequestsUsingADO(RoutingDTO model)
        {
            var employeeRequests = new List<EmployeeRequest>();

            // Extracting the correct SQL connection string
            var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
            var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();

                // Start Transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Base SQL Query with parameterized IN clauses
                        string sqlQuery = @"
                SELECT Id, EmployeeId, CompanyId, TripType, PickupShiftTimeId, DropShiftTimeId, StartRequestDate, EndRequestDate, IsRouting
                FROM EmployeeRequest 
                WHERE 
                    IsRouting = 0
                    AND CompanyId = @CompanyId
                    AND TripType = @TripType
                    AND (@PickupShiftIds IS NULL OR PickupShiftTimeId IN (SELECT value FROM SplitString(@PickupShiftIds, ',')))
                    AND (@DropShiftIds IS NULL OR DropShiftTimeId IN (SELECT value FROM SplitString(@DropShiftIds, ',')))
                    AND StartRequestDate <= @StartDate
                    AND EndRequestDate >= @EndDate";

                        using (var command = new SqlCommand(sqlQuery, connection, transaction))
                        {
                            // Adding parameters safely
                            command.Parameters.AddWithValue("@CompanyId", model.Company_Id);
                            command.Parameters.AddWithValue("@TripType", model.Trip_Type);
                            command.Parameters.AddWithValue("@StartDate", model.StartDate);
                            command.Parameters.AddWithValue("@EndDate", model.EndDate);

                            // Handling NULL values for shift IDs
                            command.Parameters.AddWithValue("@PickupShiftIds",
                                (model.PickupShiftid != null && model.PickupShiftid.Any())
                                ? string.Join(",", model.PickupShiftid)
                                : (object)DBNull.Value);

                            command.Parameters.AddWithValue("@DropShiftIds",
                                (model.DropShiftid != null && model.DropShiftid.Any())
                                ? string.Join(",", model.DropShiftid)
                                : (object)DBNull.Value);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    employeeRequests.Add(new EmployeeRequest
                                    {
                                        Id = reader.GetInt32(0),
                                        EmployeeId = reader.GetString(1),
                                        CompanyId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                        TripType = reader.GetInt32(3),
                                        PickupShiftTimeId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                        DropShiftTimeId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                        StartRequestDate = reader.GetDateTime(6),
                                        EndRequestDate = reader.GetDateTime(7),
                                        IsRouting = reader.GetBoolean(8)
                                    });
                                }
                            }
                        }

                        // Bulk update IsRouting field if any records exist
                        if (employeeRequests.Any())
                        {
                            string sqlUpdateQuery = @"
                    UPDATE EmployeeRequest 
                    SET IsRouting = 1
                    WHERE Id IN (SELECT value FROM SplitString(@Ids, ','))";

                            using (var updateCommand = new SqlCommand(sqlUpdateQuery, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@Ids",
                                    string.Join(",", employeeRequests.Select(e => e.Id)));

                                int rowsAffected = updateCommand.ExecuteNonQuery();
                                Console.WriteLine($"{rowsAffected} records updated successfully.");
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error updating IsRouting: " + ex.Message);
                    }
                }
            }

            return employeeRequests;
        }


        private void InitializeDropdowns(RoutingDTO model)
        {
            model.Customers = new SelectList(ent.Customers.Where(a => a.IsActive).OrderByDescending(a => a.Id)
        .Select(a => new
        {
            Id = a.Id,
            DisplayName = a.CompanyName + " (" + a.OrgName + ")"
        }).ToList(), "Id", "DisplayName");
            model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
            model.ShiftTypes = new SelectList(ent.TripMasters.Where(x => x.Id == 1).ToList(), "Id", "TripName");
            model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
            model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
            model.Zones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
            model.vehicleCapacity = new SelectList(ent.VehicleCapacities.ToList(), "Id", "Capacity");
        }
        public List<EmployeeGroup> GetEmployeeRequests(RoutingDTO model)
        {
            var employees = new List<Employee>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    sqlConnection.Open(); // Synchronous open

                    using (var command = new SqlCommand("GetEmployeeRequestsForRouting", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = model.Company_Id });
                        command.Parameters.Add(new SqlParameter("@TripType", SqlDbType.Int) { Value = model.Trip_Type });
                        command.Parameters.Add(new SqlParameter("@PickupShiftIds", SqlDbType.VarChar)
                        {
                            Value = (model.PickupShiftid != null && model.PickupShiftid.Any())
        ? string.Join(",", model.PickupShiftid)
        : (object)DBNull.Value
                        });

                        command.Parameters.Add(new SqlParameter("@DropShiftIds", SqlDbType.VarChar)
                        {
                            Value = (model.DropShiftid != null && model.DropShiftid.Any())
                                ? string.Join(",", model.DropShiftid)
                                : (object)DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = model.StartDate });
                        command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = model.EndDate });

                        using (var reader = command.ExecuteReader()) // Synchronous execution
                        {
                            while (reader.Read()) // Synchronous reading
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    Company_Id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                    Company_location = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Employee_Id = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Employee_First_Name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Employee_Middle_Name = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Employee_Last_Name = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    MobileNumber = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    Email = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    StateId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                    CityId = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                    Pincode = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                                    EmployeeCurrentAddress = reader.IsDBNull(12) ? null : reader.GetString(12),
                                    LoginUserName = reader.IsDBNull(13) ? null : reader.GetString(13),
                                    WeekOff = reader.IsDBNull(14) ? null : reader.GetString(14),
                                    EmployeeGeoCode = reader.IsDBNull(15) ? null : reader.GetString(15),
                                    EmployeeBusinessUnit = reader.IsDBNull(16) ? null : reader.GetString(16),
                                    EmployeeDepartment = reader.IsDBNull(17) ? null : reader.GetString(17),
                                    EmployeeProjectName = reader.IsDBNull(18) ? null : reader.GetString(18),
                                    ReportingManager = reader.IsDBNull(19) ? null : reader.GetString(19),
                                    PrimaryFacilityZone = reader.IsDBNull(20) ? 0 : reader.GetInt32(20),
                                    HomeRouteName = reader.IsDBNull(21) ? 0 : reader.GetInt32(21),
                                    EmployeeDestinationArea = reader.IsDBNull(22) ? 0 : reader.GetInt32(22),
                                    EmployeeRegistrationType = reader.IsDBNull(23) ? 0 : reader.GetInt32(23),
                                    IsActive = reader.IsDBNull(24) ? true : reader.GetBoolean(24),
                                    CreatedDate = reader.IsDBNull(25) ? DateTime.Now : reader.GetDateTime(25),
                                    Password = reader.IsDBNull(26) ? null : reader.GetString(26),
                                    IsFirst = reader.IsDBNull(27) ? true : reader.GetBoolean(27),
                                    OTP = reader.IsDBNull(28) ? 0 : reader.GetInt32(28),
                                    Gender = reader.IsDBNull(29) ? null : reader.GetString(29),
                                    AlternateNumber = reader.IsDBNull(30) ? null : reader.GetString(30),
                                    DeviceId = reader.IsDBNull(31) ? null : reader.GetString(31),
                                    Latitude = reader.IsDBNull(32) ? 0.00 : reader.GetDouble(32),
                                    Longitude = reader.IsDBNull(33) ? 0.00 : reader.GetDouble(33),
                                    ReportingManagerEmployeeId = reader.IsDBNull(34) ? null : reader.GetString(34),
                                    CurrentLat = reader.IsDBNull(35) ? 0.00 : reader.GetDouble(35),
                                    CurrentLong = reader.IsDBNull(36) ? 0.00 : reader.GetDouble(36)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching employees.", ex);
            }

            return TransformData(employees);
        }


        private List<EmployeeGroup> GroupEmployeesByZoneAndArea(List<EmployeeGroup> employees)
        {
            int groupCounter = 0;
            int cabCounter = 1001;
            int maxGroupSize = 6;
            var groupedEmployees = new List<EmployeeGroup>();

            var groupedByZone = employees.GroupBy(emp => emp.ZoneWise);

            foreach (var group in groupedByZone)
            {
                var groupQueue = group.ToList(); // Employees in this zone
                var groupMemberCount = 0;

                while (groupQueue.Any())
                {
                    groupCounter++; // Increment the group counter
                    var cabNumber = $"DummyCab{cabCounter++}"; // Assign a unique cab number

                    // Group members while adhering to the max size constraint
                    var currentGroup = groupQueue.Take(maxGroupSize).ToList();
                    groupMemberCount = currentGroup.Count;

                    foreach (var emp in currentGroup)
                    {
                        emp.Group = groupCounter.ToString(); // Assign group ID
                        emp.CabNumber = cabNumber; // Assign cab number
                        emp.groupMemberCount = groupMemberCount;
                        emp.missingEmployees = maxGroupSize - groupMemberCount;
                        groupedEmployees.Add(emp);
                    }

                    // Remove the grouped employees from the queue
                    groupQueue = groupQueue.Skip(maxGroupSize).ToList();
                }
            }
            return groupedEmployees;
        }

        private List<EmployeeGroup> TransformData(List<Employee> originalData)
        {
            return originalData.Select(emp => new EmployeeGroup
            {
                Id = emp.Id,
                Employee_Id = emp.Employee_Id,
                Gender = emp.Gender,
                CompanyName = ent.Customers.FirstOrDefault(c => c.Id == emp.Company_Id)?.OrgName,
                ComLatitude = ent.Customers.FirstOrDefault(c => c.Id == emp.Company_Id)?.Latitude ?? 0.0,
                ComLongitude = ent.Customers.FirstOrDefault(c => c.Id == emp.Company_Id)?.Longitude ?? 0.0,
                Latitude = emp.Latitude ?? 0.0,
                Longitude = emp.Longitude ?? 0.0,
                PickupandDropAddress = emp.EmployeeGeoCode,
                Name = $"{emp.Employee_First_Name} {emp.Employee_Middle_Name} {emp.Employee_Last_Name}",
                ZoneWise = ent.CompanyZones.FirstOrDefault(z => z.Id == emp.PrimaryFacilityZone)?.CompanyZone1,
                ZoneHomeWise = ent.CompanyZoneHomeRoutes.FirstOrDefault(z => z.Id == emp.HomeRouteName)?.HomeRouteName,
                DestinationAreaWise = ent.EmployeeDestinationAreas.FirstOrDefault(z => z.Id == emp.EmployeeDestinationArea)?.DestinationAreaName
            }).ToList();
        }
        [HttpGet]
        public ActionResult GetVehicleByDriver(int driverId)
        {
            var vehicle = (from c in ent.Cabs
                           join dl in ent.DriverLoginHistories on c.VehicleNumber equals dl.VehicleNumber
                           where dl.IsActive == true && c.IsLogin == true && dl.DriverId == driverId
                           select new
                           {
                               Id = c.Id
                           }
                           ).FirstOrDefault();
            if (vehicle != null)
            {
                return Json(new { vehicleId = vehicle.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { vehicleId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetVehicleByDeviceId(int deviceId)
        {
            var vehicle = (from c in ent.Cabs
                           join dl in ent.DriverLoginHistories on c.VehicleNumber equals dl.VehicleNumber
                           join d in ent.Drivers on dl.DriverId equals d.Id
                           join di in ent.DriverDeviceIds on d.DeviceId equals di.Id
                           where dl.IsActive == true && c.IsLogin == true && di.Id == deviceId
                           select new
                           {
                               Id = c.Id
                           }
                           ).FirstOrDefault();
            if (vehicle != null)
            {
                return Json(new { vehicleId = vehicle.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { vehicleId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDriverDeviceIdByDriver(int driverId)
        {
            var device = (from di in ent.DriverDeviceIds
                          join dl in ent.DriverLoginHistories on di.Driver_Id equals dl.DriverId
                          where dl.IsActive == true && dl.DriverId == driverId
                          select new
                          {
                              Id = di.Id
                          }
                           ).FirstOrDefault();
            if (device != null)
            {
                return Json(new { deviceId = device.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { deviceId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDriverDeviceIdByVehicle(int vehicleId)
        {
            var vehicleno = ent.Cabs.Where(c => c.Id == vehicleId).FirstOrDefault().VehicleNumber;
            var device = (from d in ent.Drivers
                          join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                          join di in ent.DriverDeviceIds on dl.DriverId equals di.Driver_Id
                          where dl.IsActive == true && dl.VehicleNumber == vehicleno
                          select new
                          {
                              Id = di.Id
                          }).FirstOrDefault();
            if (device != null)
            {
                return Json(new { deviceId = device.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { deviceId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDriverByVehicle(int vehicleId)
        {
            var vehicleno = ent.Cabs.Where(c => c.Id == vehicleId).FirstOrDefault().VehicleNumber;
            var driver = (from d in ent.Drivers
                          join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                          where dl.IsActive == true && dl.VehicleNumber == vehicleno
                          select new
                          {
                              Id = d.Id
                          }).FirstOrDefault();
            if (driver != null)
            {
                return Json(new { driverId = driver.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { driverId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDriverByDeviceId(int deviceId)
        {
            var driver = (from di in ent.DriverDeviceIds
                          join d in ent.Drivers on di.Id equals d.DeviceId
                          where d.IsActive == true && di.Id == deviceId
                          select new
                          {
                              Id = d.Id
                          }).FirstOrDefault();
            if (driver != null)
            {
                return Json(new { driverId = driver.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { driverId = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> AvailableDrivers()
        {
            try
            {
                var model = new AvailableDriverDTO();
                model.AvailableDrivers = await _ets.GetAvailableDrivers();

                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult> CheckInDrivers()
        {
            try
            {
                var model = new AvailableDriverDTO();
                model.AvailableDrivers = await _ets.GetAvailableDrivers();
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult> EscortCheckIn()
        {
            try
            {
                var model = new EscortDTO();
                model.EscortList = await _ets.GetChechinEscort();
                if (model.EscortList != null)
                {
                    ViewBag.Total = model.EscortList.Count();
                }
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult> EscortAvailable()
        {
            try
            {
                var model = new EscortDTO();
                model.EscortList = await _ets.GetEscortAvailable();
                if (model.EscortList != null)
                {
                    ViewBag.Total = model.EscortList.Count();
                }

                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DriverCheckoutRemark(DriverCheckoutRemarkModel model)
        {
            try
            {
                bool isCreated = await _ets.AddDriverCheckoutRemark(model);

                if (isCreated)
                {
                    return Json(new { success = true, message = "Remark added successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add remark." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }
        public ActionResult VehicleInspection()
        {
            try
            {
                var model = new VehicleInspectionDTO();
                model.Companies = new SelectList(ent.Vendors.Where(c => c.IsActive).ToList(), "Id", "CompanyName");
                model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
                model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> VehicleInspection(VehicleInspectionDTO model)
        {
            try
            {
                var companyList = new List<Customer>();
                int userId = int.Parse(User.Identity.Name);
                string isCreated = await _ets.AddVehicleInspection(model, userId);
                if (isCreated != null)
                {
                    TempData["msg"] = isCreated;
                }
                return RedirectToAction("VehicleInspection");
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error: " + ex.Message);
            }
        }
        public ActionResult GetVehicleNumbers(string term, int vendorId)
        {
            var data = ent.Cabs
                .Where(a => a.IsActive
                    && a.Vendor_Id == vendorId
                    && a.VehicleNumber.ToLower().Contains(term.ToLower()))
                .Select(a => new { a.Id, a.VehicleNumber })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVehicleModel(int vehicleId)
        {
            var vehicle = (from c in ent.Cabs
                           join vm in ent.VehicleModels on c.VehicleModel_Id equals vm.Id
                           where c.Id == vehicleId
                           select new { vm.ModelName })
                           .FirstOrDefault();

            if (vehicle != null)
            {
                return Json(new { vehicleModel = vehicle.ModelName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { vehicleModel = "Unknown Model" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult VehicleInspectionDetail()
        {
            try
            {
                var model = new VehicleInspectionDTO();
                model.Companies = new SelectList(ent.Vendors.Where(c => c.IsActive).ToList(), "Id", "CompanyName");
                model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList(), "Id", "ShiftTime");
                model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList(), "Id", "ShiftTime");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<ActionResult> SearchRoutingAndCabAllocation()
        {
            try
            {
                var model = new RoutingDTO();
                model.Customers = new SelectList(ent.Customers.Where(c => c.IsActive).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
                model.RouteStatuses = new SelectList(ent.RouteStatus.ToList(), "Id", "StatusName");
                model.TripTypes = new SelectList(ent.TripTypes.Where(x => x.TripMasterId == 1).ToList(), "Id", "TripTypeName");
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ActionResult> RoutingAndCabAllocationData(string term)
        {
            var routingList = await _ets.GetRoutingListByTerms(term);

            if (routingList == null || !routingList.routingcaballocation.Any())
            {
                return Json(new { success = false, message = "No data found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(routingList, JsonRequestBehavior.AllowGet);
        }

    }
}