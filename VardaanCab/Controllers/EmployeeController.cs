using System;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.UserModel;
using System.Data.Entity.Validation;
using NPOI.SS.Formula.Eval;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;


namespace VardaanCab.Controllers
{
    public class EmployeeController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly CommonOperations _random = new CommonOperations();
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;

        private const string GoogleMapsApiKey = "AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw"; // Replace with your API key
        public ActionResult GetEmployeeList(string term = "", int page = 1, bool export = false, int menuId = 0)
        {
            try
            {

               string fdsfds =  GetMatchedDayNames(ent.Employees.FirstOrDefault().WeekOff);
                var model = new EmployeeDTO();
                //var employee = ent.Employees.OrderByDescending(e=>e.Id).ToList();
                // Step 1: Fetch data into memory
                var data = (from e in ent.Employees
                            join c in ent.Customers on e.Company_Id equals c.Id
                            join z in ent.CompanyZones on e.PrimaryFacilityZone equals z.Id
                            join hr in ent.CompanyZoneHomeRoutes on e.HomeRouteName equals hr.Id
                            join rt in ent.EmployeeRegistrationTypes on e.EmployeeRegistrationType equals rt.Id
                            join da in ent.EmployeeDestinationAreas on e.EmployeeDestinationArea equals da.Id
                            join s in ent.StateMasters on e.StateId equals s.Id
                            join ct in ent.CityMasters on e.CityId equals ct.Id
                            where e.IsActive == true
                            orderby e.Id descending
                            select new
                            {
                                e.Id,
                                e.Employee_First_Name,
                                e.Employee_Middle_Name,
                                e.Employee_Last_Name,
                                e.MobileNumber,
                                e.Email,
                                e.Pincode,
                                e.EmployeeCurrentAddress,
                                e.EmployeeGeoCode,
                                e.EmployeeBusinessUnit,
                                e.EmployeeDepartment,
                                e.Employee_Id,
                                e.EmployeeProjectName,
                                e.ReportingManager,
                                e.AlternateNumber,
                                StateName = s.StateName,
                                CityName = ct.CityName,
                                PrimaryFacilityZone = z.CompanyZone1,
                                HomeRouteName = hr.HomeRouteName,
                                CompanyName = c.CompanyName,
                                EmployeeRegistrationType = rt.TypeName,
                                da.DestinationAreaName,
                                e.CreatedDate,
                                e.WeekOff 
                            }).ToList(); 

                var employeedetailList = data.Select(e => new employeedetail
                {
                    Id = e.Id,
                    Employee_First_Name = e.Employee_First_Name,
                    Employee_Middle_Name = e.Employee_Middle_Name,
                    Employee_Last_Name = e.Employee_Last_Name,
                    MobileNumber = e.MobileNumber,
                    Email = e.Email,
                    Pincode = e.Pincode,
                    EmployeeCurrentAddress = e.EmployeeCurrentAddress,
                    EmployeeGeoCode = e.EmployeeGeoCode,
                    EmployeeBusinessUnit = e.EmployeeBusinessUnit,
                    EmployeeDepartment = e.EmployeeDepartment,
                    Employee_Id = e.Employee_Id,
                    EmployeeProjectName = e.EmployeeProjectName,
                    ReportingManager = e.ReportingManager,
                    AlternateNumber = e.AlternateNumber,
                    StateName = e.StateName,
                    CityName = e.CityName,
                    PrimaryFacilityZone = e.PrimaryFacilityZone,
                    HomeRouteName = e.HomeRouteName,
                    EmployeeDestinationArea = e.DestinationAreaName,
                    CompanyName = e.CompanyName,
                    EmployeeRegistrationType = e.EmployeeRegistrationType,
                    CreatedDate = e.CreatedDate,
                    WeekOffName = GetMatchedDayNames(e.WeekOff) 
                }).ToList();

                model.employeedetailList = employeedetailList;
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }

        public string GetMatchedDayNames(string weekOffIds)
        {
            if (string.IsNullOrWhiteSpace(weekOffIds))
            {
                return weekOffIds;
            }

            try
            {
                // Convert the comma-separated IDs to a list of integers
                var ids = weekOffIds
                    .Split(',')
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();

                // Query the DaysNames table to get matching day names
                var matchedDays = ent.DaysNames
                    .Where(d => ids.Contains(d.Id))
                    .Select(d => d.DayName)
                    .ToList();

                return string.Join(",", matchedDays);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return ex.Message;
            }
        }
        [HttpGet]
        public ActionResult Add(int id = 0)
        {
            try
            {
                var model = new EmployeeDTO();
                model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
                //model.DayLists = new SelectList(ent.DaysNames.ToList(), "Id", "DayName");
                model.DayLists = ent.DaysNames.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DayName
                }).ToList();
                model.Customers = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CustomerName");
                model.RegistrationTypes = new SelectList(ent.EmployeeRegistrationTypes.ToList(), "Id", "TypeName");

                if (id > 0)
                {
                    var data = ent.Employees.Where(x => x.Id == id).FirstOrDefault();
                    model.Id = data.Id;
                    model.Company_location = data.Company_location;
                    model.Company_Id = data.Company_Id;
                    model.Employee_Id = data.Employee_Id;
                    model.Employee_First_Name = data.Employee_First_Name;
                    model.Employee_Middle_Name = data.Employee_Middle_Name;
                    model.Employee_Last_Name = data.Employee_Last_Name;
                    model.MobileNumber = data.MobileNumber;
                    model.Email = data.Email;
                    model.StateId = data.StateId;
                    model.CityId = data.CityId;
                    ViewBag.CityId = data.CityId;
                    model.Pincode = data.Pincode;
                    model.EmployeeCurrentAddress = data.EmployeeCurrentAddress;
                    model.LoginUserName = data.LoginUserName;
                    //model.WeekOff = data.WeekOff;
                    model.WeekOffs = data.WeekOff.Split(',').ToList();

                    model.EmployeeGeoCode = data.EmployeeGeoCode;
                    model.EmployeeBusinessUnit = data.EmployeeBusinessUnit;
                    model.EmployeeDepartment = data.EmployeeDepartment;
                    model.EmployeeProjectName = data.EmployeeProjectName;
                    model.ReportingManager = data.ReportingManager;
                    model.PrimaryFacilityZone = data.PrimaryFacilityZone;
                    ViewBag.PrimaryFacilityZone = data.PrimaryFacilityZone;
                    model.HomeRouteName = data.HomeRouteName;
                    ViewBag.HomeRouteName = data.HomeRouteName;
                    model.EmployeeDestinationArea = data.EmployeeDestinationArea;
                    ViewBag.EmployeeDestinationArea = data.EmployeeDestinationArea;
                    model.EmployeeRegistrationType = data.EmployeeRegistrationType;
                    model.Gender = data.Gender;
                    model.AlternateNumber = data.AlternateNumber;
                    ViewBag.Heading = "Update Employee";
                    ViewBag.BtnTXT = "Update";
                    return View(model);
                }
                else
                {
                    model.Id = 0;
                    model.Company_location = null;
                    model.Company_Id = 0;
                    model.Employee_Id = null;
                    model.Employee_First_Name = null;
                    model.Employee_Middle_Name = null;
                    model.Employee_Last_Name = null;
                    model.MobileNumber = null;
                    model.Email = null;
                    model.StateId = 0;
                    model.CityId = 0;
                    model.Pincode = null;
                    model.EmployeeCurrentAddress = null;
                    model.LoginUserName = null;
                    model.WeekOff = null;
                    model.EmployeeGeoCode = null;
                    model.EmployeeBusinessUnit = null;
                    model.EmployeeDepartment = null;
                    model.EmployeeProjectName = null;
                    model.ReportingManager = null;
                    model.PrimaryFacilityZone = 0;
                    model.HomeRouteName = 0;
                    model.EmployeeDestinationArea = 0;
                    model.Gender = null;
                    model.AlternateNumber = null;
                    ViewBag.CityId = 0;
                    ViewBag.PrimaryFacilityZone = 0;
                    ViewBag.HomeRouteName = 0;
                    ViewBag.EmployeeDestinationArea = 0;
                    ViewBag.BtnTXT = "Save";
                    ViewBag.Heading = "Create New Employee";
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Add(EmployeeDTO model)
        {
            try
            {
                string RandomPassword = _random.GenerateRandomPassword();
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var latlong = LatLog(model.EmployeeGeoCode);
                double latitude = latlong.Count > 0 && latlong[0].ContainsKey("latitude") ? latlong[0]["latitude"] : 0.0;
                double longitude = latlong.Count > 0 && latlong[0].ContainsKey("longitude") ? latlong[0]["longitude"] : 0.0;
                using (var entityConnection = new EntityConnection(entityConnectionString))
                {
                    entityConnection.Open();
                    using (var command = entityConnection.CreateCommand())
                    {
                        command.CommandText = "Vardaan_AdminEntities.ManageEmployee";
                        command.CommandType = CommandType.StoredProcedure;
                        string checkaction = model.Id == 0 ? "INSERT" : "UPDATE";
                        string combinedString = string.Join(",", model.WeekOff);
                        command.Parameters.Add(new EntityParameter("Action", DbType.String) { Value = checkaction });
                        command.Parameters.Add(new EntityParameter("Id", DbType.Int32) { Value = model.Id });
                        command.Parameters.Add(new EntityParameter("Company_Id", DbType.Int32) { Value = model.Company_Id });
                        command.Parameters.Add(new EntityParameter("Company_location", DbType.String) { Value = model.Company_location });
                        command.Parameters.Add(new EntityParameter("Employee_Id", DbType.Int32) { Value = model.Employee_Id });
                        command.Parameters.Add(new EntityParameter("Employee_First_Name", DbType.String) { Value = model.Employee_First_Name });
                        command.Parameters.Add(new EntityParameter("Employee_Middle_Name", DbType.String) { Value = model.Employee_Middle_Name ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("Employee_Last_Name", DbType.String) { Value = model.Employee_Last_Name });
                        command.Parameters.Add(new EntityParameter("Gender", DbType.String) { Value = model.Gender });
                        command.Parameters.Add(new EntityParameter("MobileNumber", DbType.String) { Value = model.MobileNumber });
                        command.Parameters.Add(new EntityParameter("Email", DbType.String) { Value = model.Email });
                        command.Parameters.Add(new EntityParameter("StateId", DbType.Int32) { Value = model.StateId });
                        command.Parameters.Add(new EntityParameter("CityId", DbType.Int32) { Value = model.CityId });
                        command.Parameters.Add(new EntityParameter("Pincode", DbType.String) { Value = model.Pincode });
                        command.Parameters.Add(new EntityParameter("EmployeeCurrentAddress", DbType.String) { Value = model.EmployeeCurrentAddress });
                        command.Parameters.Add(new EntityParameter("LoginUserName", DbType.String) { Value = model.LoginUserName });
                        command.Parameters.Add(new EntityParameter("WeekOff", DbType.String) { Value = combinedString ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("EmployeeGeoCode", DbType.String) { Value = model.EmployeeGeoCode ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("EmployeeBusinessUnit", DbType.String) { Value = model.EmployeeBusinessUnit ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("EmployeeDepartment", DbType.String) { Value = model.EmployeeDepartment ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("EmployeeProjectName", DbType.String) { Value = model.EmployeeProjectName ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("ReportingManager", DbType.String) { Value = model.ReportingManager ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("PrimaryFacilityZone", DbType.String) { Value = model.PrimaryFacilityZone });
                        command.Parameters.Add(new EntityParameter("HomeRouteName", DbType.String) { Value = model.HomeRouteName });
                        command.Parameters.Add(new EntityParameter("EmployeeDestinationArea", DbType.String) { Value = model.EmployeeDestinationArea });
                        command.Parameters.Add(new EntityParameter("EmployeeRegistrationType", DbType.String) { Value = model.EmployeeRegistrationType });
                        command.Parameters.Add(new EntityParameter("IsActive", DbType.Boolean) { Value = true });
                        command.Parameters.Add(new EntityParameter("Password", DbType.String) { Value = RandomPassword });
                        command.Parameters.Add(new EntityParameter("Latitude", DbType.Double) { Value = latitude });
                        command.Parameters.Add(new EntityParameter("Longitude", DbType.Double) { Value = longitude });
                        // Output parameter for the response message
                        var responseMessageParam = new EntityParameter("ResponseMessage", DbType.String) { Direction = ParameterDirection.Output, Size = 255 };
                        command.Parameters.Add(responseMessageParam);

                        command.ExecuteNonQuery();

                        // Get the response message
                        string responseMessage = responseMessageParam.Value.ToString();

                        TempData["msg"] = responseMessage;
                    }
                }

                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error: " + ex.Message);
            }
        }
        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;

                using (var entityConnection = new EntityConnection(entityConnectionString))
                {
                    entityConnection.Open();
                    using (var command = entityConnection.CreateCommand())
                    {
                        command.CommandText = "Vardaan_AdminEntities.ManageEmployee";
                        command.CommandType = CommandType.StoredProcedure;

                        string checkaction = "DELETE";

                        command.Parameters.Add(new EntityParameter("Action", DbType.String) { Value = checkaction });
                        command.Parameters.Add(new EntityParameter("Id", DbType.Int32) { Value = id });
                         
                        var responseMessageParam = new EntityParameter("ResponseMessage", DbType.String) { Direction = ParameterDirection.Output, Size = 255 };
                        command.Parameters.Add(responseMessageParam);

                        command.ExecuteNonQuery(); 
                        string responseMessage = responseMessageParam.Value.ToString();

                        TempData["dltmsg"] = responseMessage;
                    }
                }
                return RedirectToAction("GetEmployeeList");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Company_Id");
            dt.Columns.Add("Company_location");
            dt.Columns.Add("Employee_Id");
            dt.Columns.Add("Employee_First_Name");
            dt.Columns.Add("Employee_Middle_Name");
            dt.Columns.Add("Employee_Last_Name");
            dt.Columns.Add("MobileNumber");
            dt.Columns.Add("Email");
            dt.Columns.Add("StateId");
            dt.Columns.Add("CityId");
            dt.Columns.Add("Pincode");
            dt.Columns.Add("EmployeeCurrentAddress");
            dt.Columns.Add("LoginUserName");
            dt.Columns.Add("WeekOff");
            dt.Columns.Add("EmployeeGeoCode");
            dt.Columns.Add("EmployeeBusinessUnit");
            dt.Columns.Add("EmployeeDepartment");
            dt.Columns.Add("EmployeeProjectName");
            dt.Columns.Add("ReportingManager");
            dt.Columns.Add("PrimaryFacilityZone");
            dt.Columns.Add("HomeRouteName");
            dt.Columns.Add("EmployeeDestinationArea");
            dt.Columns.Add("EmployeeRegistrationType");
            dt.Columns.Add("Gender");
            dt.Columns.Add("AlternateNumber");

            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
            {
            { "Company_Id", "Company Name" },
            { "Company_location", "Company Location" },
            { "Employee_Id", "Employee ID" },
            { "Employee_First_Name", "First Name" },
            { "Employee_Middle_Name", "Middle Name" },
            { "Employee_Last_Name", "Last Name" },
            { "MobileNumber", "Mobile Number" },
            { "Email", "Email Address" },
            { "StateId", "State Name" },
            { "CityId", "City Name" },
            { "Pincode", "Postal Code" },
            { "EmployeeCurrentAddress", "Current Address" },
            { "LoginUserName", "Login Username" },
            { "WeekOff", "Week Off" },
            { "EmployeeGeoCode", "Geo Location" },
            { "EmployeeBusinessUnit", "Business Unit" },
            { "EmployeeDepartment", "Department" },
            { "EmployeeProjectName", "Project Name" },
            { "ReportingManager", "Reporting Manager" },
            { "PrimaryFacilityZone", "Facility Zone" },
            { "HomeRouteName", "Home Route Name" },
            { "EmployeeDestinationArea", "Destination Area" },
            { "EmployeeRegistrationType", "Registration Type" },
            { "Gender", "Gender" },
            { "AlternateNumber", "Alternate Contact" }
            };

            // Export to Excel
            using (XLWorkbook workbook = new XLWorkbook())
            {
                // Create a worksheet and add the headers
                var worksheet = workbook.Worksheets.Add("Employee");

                // Add the headers based on the mapping
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
                var StatehiddenSheet = workbook.Worksheets.Add("StateList");
                var CityhiddenSheet = workbook.Worksheets.Add("CityList");
                var GenderList = "\"Male,Female,Other\"";
                var WeekOffList = "\"Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday\"";
                var ZonehiddenSheet = workbook.Worksheets.Add("CompanyZone");
                var HomeRoutehiddenSheet = workbook.Worksheets.Add("HomeRoute");
                var DestinationAreahiddenSheet = workbook.Worksheets.Add("DestinationArea");
                var RegistrationTypehiddenSheet = workbook.Worksheets.Add("RegistrationType");

                // Retrieve active customers for the dropdown list
                var companyList = ent.Customers.Where(x => x.IsActive == true).ToList();
                var StateList = ent.StateMasters.ToList();
                var CityList = ent.CityMasters.ToList();
                var ZoneList = ent.CompanyZones.ToList();
                var HomeRouteList = ent.CompanyZoneHomeRoutes.ToList();
                var DestinationAreaList = ent.EmployeeDestinationAreas.ToList();
                var RegistrationTypeList = ent.EmployeeRegistrationTypes.ToList();

                // Populate hidden sheet with company names
                int hiddenRow = 1;
                foreach (var company in companyList.OrderByDescending(x => x.Id))
                {
                    hiddenSheet.Cell(hiddenRow++, 1).Value = company.CompanyName;
                }
                hiddenRow = 1;
                foreach (var state in StateList.OrderByDescending(x => x.Id))
                {
                    StatehiddenSheet.Cell(hiddenRow++, 1).Value = state.StateName;

                }
                hiddenRow = 1;
                foreach (var city in CityList.OrderByDescending(x => x.Id))
                {
                    CityhiddenSheet.Cell(hiddenRow++, 1).Value = city.CityName;
                }
                //zone
                hiddenRow = 1;
                foreach (var zones in ZoneList.OrderByDescending(x => x.Id))
                {
                    ZonehiddenSheet.Cell(hiddenRow++, 1).Value = zones.CompanyZone1;
                }
                //home route
                hiddenRow = 1;
                foreach (var routes in HomeRouteList.OrderByDescending(x => x.Id))
                {
                    HomeRoutehiddenSheet.Cell(hiddenRow++, 1).Value = routes.HomeRouteName;
                }
                //Destination Area
                hiddenRow = 1;
                foreach (var areas in DestinationAreaList.OrderByDescending(x => x.Id))
                {
                    DestinationAreahiddenSheet.Cell(hiddenRow++, 1).Value = areas.DestinationAreaName;
                }
                //Registration Type
                hiddenRow = 1;
                foreach (var Regtypes in RegistrationTypeList.OrderByDescending(x => x.Id))
                {
                    RegistrationTypehiddenSheet.Cell(hiddenRow++, 1).Value = Regtypes.TypeName;
                }
                // Define the dropdown list range
                var companyRange = hiddenSheet.Range($"A1:A{companyList.Count}");
                var StateRange = StatehiddenSheet.Range($"A1:A{StateList.Count}");
                var CityRange = CityhiddenSheet.Range($"A1:A{CityList.Count}");
                var ZoneRange = ZonehiddenSheet.Range($"A1:A{ZoneList.Count}");
                var HomeRouteRange = HomeRoutehiddenSheet.Range($"A1:A{ZoneList.Count}");
                var DestinationAreaRange = DestinationAreahiddenSheet.Range($"A1:A{DestinationAreaList.Count}");
                var RegistrationTypeRange = RegistrationTypehiddenSheet.Range($"A1:A{RegistrationTypeList.Count}");


                //Apply dropdown list validation to cell A2(under "Company ID")
                var validationOne = worksheet.Cell(2, 1).DataValidation;
                validationOne.List(companyRange); // Dropdown from hidden sheet
                validationOne.IgnoreBlanks = true;
                validationOne.InCellDropdown = true;

                var validationTwo = worksheet.Cell(2, 9).DataValidation;
                validationTwo.List(StateRange); // Dropdown from hidden sheet
                validationTwo.IgnoreBlanks = true;
                validationTwo.InCellDropdown = true;

                var validationThree = worksheet.Cell(2, 10).DataValidation;
                validationThree.List(CityRange); // Dropdown from hidden sheet
                validationThree.IgnoreBlanks = true;
                validationThree.InCellDropdown = true;

                var validationFour = worksheet.Cell(2, 24).DataValidation;
                validationFour.List(GenderList); // Dropdown from hidden sheet
                validationFour.IgnoreBlanks = true;
                validationFour.InCellDropdown = true;

                var validationFive = worksheet.Cell(2, 14).DataValidation;
                validationFive.List(WeekOffList); // Dropdown from hidden sheet
                validationFive.IgnoreBlanks = true;
                validationFive.InCellDropdown = true;

                //zone
                var validationZone = worksheet.Cell(2, 20).DataValidation;
                validationZone.List(ZoneRange); // Dropdown from hidden sheet
                validationZone.IgnoreBlanks = true;
                validationZone.InCellDropdown = true;
                //Route
                var validationRoute = worksheet.Cell(2, 21).DataValidation;
                validationRoute.List(HomeRouteRange); // Dropdown from hidden sheet
                validationRoute.IgnoreBlanks = true;
                validationRoute.InCellDropdown = true;
                //Destination Area
                var validationArea = worksheet.Cell(2, 22).DataValidation;
                validationArea.List(DestinationAreaRange); // Dropdown from hidden sheet
                validationArea.IgnoreBlanks = true;
                validationArea.InCellDropdown = true;
                //Destination Area
                var validationRegistrationType = worksheet.Cell(2, 23).DataValidation;
                validationRegistrationType.List(RegistrationTypeRange); // Dropdown from hidden sheet
                validationRegistrationType.IgnoreBlanks = true;
                validationRegistrationType.InCellDropdown = true;

                // Hide the sheet containing company names
                //hiddenSheet.Hide();
                //StatehiddenSheet.Hide();
                //CityhiddenSheet.Hide();

                // Save and return Excel file as download
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeData.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ImportEmployeeData(HttpPostedFileBase file)
        {
            try
            {
                string RandomPassword = _random.GenerateRandomPassword();

                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<Employee> employees = new List<Employee>();

                        foreach (var row in rows)
                        {
                            string CompanyName = row.Cell(1).GetValue<string>();
                            string StateName = row.Cell(9).GetValue<string>();
                            string CityName = row.Cell(10).GetValue<string>();
                            string CompanyZoneName = row.Cell(20).GetValue<string>();
                            string HomeRouteName = row.Cell(21).GetValue<string>();
                            string DestinationAreaName = row.Cell(22).GetValue<string>();
                            string RegistrationTypeName = row.Cell(23).GetValue<string>();
                            string DaysName = row.Cell(14).GetValue<string>();

                            Employee employee = new Employee
                            {
                                Company_Id = string.IsNullOrEmpty(CompanyName) ? 0 :
                                    ent.Customers.Where(x => x.CompanyName.ToLower() == CompanyName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                Company_location = row.Cell(2).GetValue<int>().ToString(),
                                Employee_Id = row.Cell(3).GetValue<string>() ?? string.Empty,
                                Employee_First_Name = row.Cell(4).GetValue<string>() ?? string.Empty,
                                Employee_Middle_Name = row.Cell(5).GetValue<string>() ?? string.Empty,
                                Employee_Last_Name = row.Cell(6).GetValue<string>() ?? string.Empty,
                                MobileNumber = row.Cell(7).GetValue<string>() ?? string.Empty,
                                Email = row.Cell(8).GetValue<string>() ?? string.Empty,

                                StateId = string.IsNullOrEmpty(StateName) ? 0 :
                                    ent.StateMasters.Where(x => x.StateName.ToLower() == StateName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                CityId = string.IsNullOrEmpty(CityName) ? 0 :
                                    ent.CityMasters.Where(x => x.CityName.ToLower() == CityName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                Pincode = row.Cell(11).GetValue<int>(),
                                EmployeeCurrentAddress = row.Cell(12).GetValue<string>() ?? string.Empty,
                                LoginUserName = row.Cell(13).GetValue<string>() ?? string.Empty,
                                WeekOff = string.IsNullOrEmpty(DaysName) ? "" :
    ent.DaysNames.Where(x => x.DayName.ToLower() == DaysName.ToLower())
        .FirstOrDefault()?.Id.ToString() ?? "",

                                EmployeeGeoCode = row.Cell(15).GetValue<string>() ?? string.Empty,
                                EmployeeBusinessUnit = row.Cell(16).GetValue<string>() ?? string.Empty,
                                EmployeeDepartment = row.Cell(17).GetValue<string>() ?? string.Empty,
                                EmployeeProjectName = row.Cell(18).GetValue<string>() ?? string.Empty,
                                ReportingManager = row.Cell(19).GetValue<string>() ?? string.Empty,

                                PrimaryFacilityZone = string.IsNullOrEmpty(CompanyZoneName) ? 0 :
                                    ent.CompanyZones.Where(x => x.CompanyZone1.ToLower() == CompanyZoneName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                HomeRouteName = string.IsNullOrEmpty(HomeRouteName) ? 0 :
                                    ent.CompanyZoneHomeRoutes.Where(x => x.HomeRouteName.ToLower() == HomeRouteName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                EmployeeDestinationArea = string.IsNullOrEmpty(DestinationAreaName) ? 0 :
                                    ent.EmployeeDestinationAreas.Where(x => x.DestinationAreaName.ToLower() == DestinationAreaName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                EmployeeRegistrationType = string.IsNullOrEmpty(RegistrationTypeName) ? 0 :
                                    ent.EmployeeRegistrationTypes.Where(x => x.TypeName.ToLower() == RegistrationTypeName.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                IsFirst = true,
                                Password = RandomPassword,
                                Gender = row.Cell(24).GetValue<string>(),
                                AlternateNumber = row.Cell(25).GetValue<string>() ?? string.Empty
                            };

                            employees.Add(employee);
                        }

                        if (employees.Any())
                        {
                            ent.Employees.AddRange(employees);
                            ent.SaveChanges();
                        }
                        TempData["dltmsg"] = "Data imported successfully!";
                        return RedirectToAction("GetEmployeeList");
                    }
                }

                ViewBag.Message = "Please select an Excel file to import.";
                return View();
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
        public List<Dictionary<string, double>> LatLog(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={GoogleMapsApiKey}";
            var resultList = new List<Dictionary<string, double>>();

            using (var client = new HttpClient())
            {
                // Send the request synchronously
                var response = client.GetStringAsync(url).Result;

                // Parse the response
                var json = JObject.Parse(response);

                // Check the response status
                var status = json["status"].ToString();
                if (status == "OK")
                {
                    // Extract latitude and longitude
                    double lat = (double)json["results"][0]["geometry"]["location"]["lat"];
                    double lng = (double)json["results"][0]["geometry"]["location"]["lng"];

                    // Add latitude and longitude to the dictionary
                    var locationData = new Dictionary<string, double>
            {
                { "latitude", lat },
                { "longitude", lng }
            };

                    // Add the dictionary to the list
                    resultList.Add(locationData);
                }
            }

            return resultList;
        }
    }
}