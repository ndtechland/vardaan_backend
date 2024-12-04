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


namespace VardaanCab.Controllers
{
    public class EmployeeController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly CommonOperations _random = new CommonOperations();
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
        public ActionResult GetEmployeeList(string term = "", int page = 1, bool export = false, int menuId = 0)
        {
            try
            {
                var employee = ent.Employees.ToList();
                return View(employee);
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
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
                //model.CompanyZone = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
                //model.CompanyZoneHomeRoute = new SelectList(ent.CompanyZoneHomeRoutes.ToList(), "Id", "HomeRouteName");
                //model.DestinationArea = new SelectList(ent.EmployeeDestinationAreas.ToList(), "Id", "DestinationAreaName");
                model.RegistrationTypes = new SelectList(ent.EmployeeRegistrationTypes.ToList(), "Id", "TypeName");

                return View(model);
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

                using (var entityConnection = new EntityConnection(entityConnectionString))
                {
                    entityConnection.Open();

                    using (var command = entityConnection.CreateCommand())
                    {
                        command.CommandText = "Vardaan_AdminEntities.ManageEmployee";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new EntityParameter("Action", DbType.String) { Value = "INSERT" });
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
                        command.Parameters.Add(new EntityParameter("WeekOff", DbType.String) { Value = model.WeekOff ?? (object)DBNull.Value });
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
                        command.Parameters.Add(new EntityParameter("Password", DbType.String) { Value = null });

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                throw new Exception("Server Error: " + ex.Message);
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

                // Retrieve active customers for the dropdown list
                var companyList = ent.Customers.Where(x => x.IsActive == true).ToList();
                var StateList = ent.StateMasters.ToList();
                var CityList = ent.CityMasters.ToList();

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
                // Define the dropdown list range
                var companyRange = hiddenSheet.Range($"A1:A{companyList.Count}");
                var StateRange = StatehiddenSheet.Range($"A1:A{StateList.Count}");
                var CityRange = CityhiddenSheet.Range($"A1:A{CityList.Count}");


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

        #region no use
        //public ActionResult ExportToExcel()
        //{
        //    DataTable dt = GetTableData(); // Get data from the database

        //    var columnsToRemove = new List<string> { "Id", "IsActive", "CreatedDate", "Password", "IsFirst", "OTP" };

        //    foreach (string columnName in columnsToRemove)
        //    {
        //        if (dt.Columns.Contains(columnName))
        //        {
        //            dt.Columns.Remove(columnName);
        //        }
        //    }
        //    // Create a new DataTable with only the column names
        //    DataTable dtWithColumnNames = new DataTable();
        //    // Add columns to the new DataTable
        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        dtWithColumnNames.Columns.Add(column.ColumnName);
        //    }

        //    // Add a single row with column names as values
        //    DataRow headerRow = dtWithColumnNames.NewRow();
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        headerRow[i] = dt.Columns[i].ColumnName;
        //    }
        //    dtWithColumnNames.Rows.Add(headerRow);

        //    // Export to Excel
        //    using (XLWorkbook workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add(dtWithColumnNames, "Employee");
        //        var hiddenSheet = workbook.Worksheets.Add("Country");
        //        var dd = ent.Customers.Where(x =>x.IsActive == true).ToList();
        //        int countryRow = 1;
        //        foreach (var ctry in dd.OrderByDescending(x =>x.Id))
        //        {
        //            hiddenSheet.Cell(countryRow++, 2).Value = ctry.CompanyName;
        //        }

        //        var countryRange = hiddenSheet.Range($"B1:B{dd.Count}");

        //        var validation = worksheet.Cell(2, 1).DataValidation; // Apply to cell E2 as an example
        //        validation.List(countryRange); // Refer to hidden list
        //        validation.IgnoreBlanks = true; // Optional: allows blank entries
        //        validation.InCellDropdown = true; // Shows dropdown
        //        //countryValidation.List(countryRange);
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            stream.Position = 0;
        //            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeData.xlsx");
        //        }
        //    }
        //}

        //private DataTable GetTableData()
        //{
        //    var entityBuilder = new EntityConnectionStringBuilder(efConnectionString);
        //    string sqlConnectionString = entityBuilder.ProviderConnectionString;
        //    DataTable dt = new DataTable();

        //    using (SqlConnection con = new SqlConnection(sqlConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con))
        //        {
        //            con.Open();
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //        }
        //    }

        //    return dt;
        //}
        #endregion

        [HttpPost]
        public ActionResult ImportEmployeeData(HttpPostedFileBase file)
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
                        List<Employee> employees = new List<Employee>();

                        foreach (var row in rows)
                        {
                            Employee employee = new Employee
                            {
                                Company_Id = row.Cell(1).GetValue<int>(),
                                Company_location = row.Cell(2).GetValue<int>().ToString(),
                                Employee_Id = row.Cell(3).GetValue<string>() ?? string.Empty,
                                Employee_First_Name = row.Cell(4).GetValue<string>() ?? string.Empty,
                                Employee_Middle_Name = row.Cell(5).GetValue<string>() ?? string.Empty,
                                Employee_Last_Name = row.Cell(6).GetValue<string>() ?? string.Empty,
                                MobileNumber = row.Cell(7).GetValue<string>() ?? string.Empty,
                                Email = row.Cell(8).GetValue<string>() ?? string.Empty,
                                StateId = row.Cell(9).GetValue<int>(),
                                CityId = row.Cell(10).GetValue<int>(),
                                Pincode = row.Cell(11).GetValue<int>(),
                                EmployeeCurrentAddress = row.Cell(12).GetValue<string>() ?? string.Empty,
                                LoginUserName = row.Cell(13).GetValue<string>() ?? string.Empty,
                                WeekOff = row.Cell(14).GetValue<string>() ?? "Sunday",  // Default to "Sunday" if null
                                EmployeeGeoCode = row.Cell(15).GetValue<string>() ?? string.Empty,
                                EmployeeBusinessUnit = row.Cell(16).GetValue<string>() ?? string.Empty,
                                EmployeeDepartment = row.Cell(17).GetValue<string>() ?? string.Empty,
                                EmployeeProjectName = row.Cell(18).GetValue<string>() ?? string.Empty,
                                ReportingManager = row.Cell(19).GetValue<string>() ?? string.Empty,
                                PrimaryFacilityZone = row.Cell(20).GetValue<int>(),
                                HomeRouteName = row.Cell(21).GetValue<int>(),
                                EmployeeDestinationArea = row.Cell(22).GetValue<int>(),
                                EmployeeRegistrationType = row.Cell(23).GetValue<int>(),
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                IsFirst = true,
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
                       
                        ViewBag.Message = "Data imported successfully!";
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
        }

    }
}