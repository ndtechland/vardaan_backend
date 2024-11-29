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
            // Get data from the database
            DataTable dt = GetTableData();

            var columnsToRemove = new List<string> { "Id", "IsActive", "CreatedDate", "Password", "IsFirst", "OTP" };

            foreach (string columnName in columnsToRemove)
            {
                if (dt.Columns.Contains(columnName))
                {
                    dt.Columns.Remove(columnName);
                }
            }


            // Fetch the list of active customers for the dropdown
            var activeCustomers = ent.Customers.Where(a => a.IsActive).ToList();

            // Create a SelectList for the Company_Id column (assuming Company_Id maps to Customer.Id)
            SelectList companySelectList = new SelectList(activeCustomers, "Id", "CustomerName");

            // Get the available options for the dropdown
            var companyIds = companySelectList.Items.Cast<SelectListItem>()
                                                    .Select(item => item.Value)
                                                    .ToList();

            // Export to Excel
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dt, "Employee");

                // Find the column index of the "Company_Id" column
                int companyIdColumnIndex = -1;
                for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                {
                    if (dt.Columns[colIndex].ColumnName == "Company_Id")
                    {
                        companyIdColumnIndex = colIndex + 1; // Excel columns are 1-indexed
                        break;
                    }
                }

                // Save the workbook to a memory stream
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeDataWithCompanyDropdown.xlsx");
                }
            }
        }

        private DataTable GetTableData()
        {
            var entityBuilder = new EntityConnectionStringBuilder(efConnectionString);
            string sqlConnectionString = entityBuilder.ProviderConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

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
                    // Get the first worksheet (assuming data is in the first worksheet)
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1); // Skip the header row

                    // Create a list to store employee data
                    //Employee employees = new Employee();

                        List<Employee> employees = new List<Employee>();

                        foreach (var row in rows)
                        {
                            // Initialize a new Employee object
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

                            // Add the Employee object to the list
                            employees.Add(employee);
                        }

                        // Save all Employee objects to the database in bulk
                        if (employees.Any())
                        {
                            ent.Employees.AddRange(employees);
                            ent.SaveChanges();
                        }



                        // Return success message
                        ViewBag.Message = "Data imported successfully!";
                    return RedirectToAction("GetEmployeeList");
                }
            }

            // If no file is selected, return an error message
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