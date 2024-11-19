using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;


namespace VardaanCab.Controllers
{
    public class EmployeeController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        [HttpGet]
        public ActionResult Add()
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
        [HttpPost]
        public ActionResult Add(EmployeeDTO model)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("ManageEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@Action", "INSERT");
                        command.Parameters.AddWithValue("@Company_Id", model.Company_Id);
                        command.Parameters.AddWithValue("@Company_location", model.Company_location);
                        command.Parameters.AddWithValue("@Employee_Id", model.Employee_Id);
                        command.Parameters.AddWithValue("@Employee_First_Name", model.Employee_First_Name);
                        command.Parameters.AddWithValue("@Employee_Middle_Name", model.Employee_Middle_Name ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Employee_Last_Name", model.Employee_Last_Name);
                        command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@StateId", model.StateId);
                        command.Parameters.AddWithValue("@CityId", model.CityId);
                        command.Parameters.AddWithValue("@Pincode", model.Pincode);
                        command.Parameters.AddWithValue("@EmployeeCurrentAddress", model.EmployeeCurrentAddress);
                        command.Parameters.AddWithValue("@LoginUserName", model.LoginUserName);
                        command.Parameters.AddWithValue("@WeekOff", model.WeekOff ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EmployeeGeoCode", model.EmployeeGeoCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EmployeeBusinessUnit", model.EmployeeBusinessUnit ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EmployeeDepartment", model.EmployeeDepartment ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EmployeeProjectName", model.EmployeeProjectName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ReportingManager", model.ReportingManager ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PrimaryFacilityZone", model.PrimaryFacilityZone);
                        command.Parameters.AddWithValue("@HomeRouteName", model.HomeRouteName);
                        command.Parameters.AddWithValue("@EmployeeDestinationArea", model.EmployeeDestinationArea);
                        command.Parameters.AddWithValue("@EmployeeRegistrationType", model.EmployeeRegistrationType);
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@Password", "12345");

                        // Execute the command
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
    }
}