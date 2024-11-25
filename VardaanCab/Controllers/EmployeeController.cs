using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;


namespace VardaanCab.Controllers
{
    public class EmployeeController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly CommonOperations _random = new CommonOperations();

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
                model.CompanyZone = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
                model.CompanyZoneHomeRoute = new SelectList(ent.CompanyZoneHomeRoutes.ToList(), "Id", "HomeRouteName");
                model.DestinationArea = new SelectList(ent.EmployeeDestinationAreas.ToList(), "Id", "DestinationAreaName");
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
                        command.Parameters.Add(new EntityParameter("Password", DbType.String) { Value = RandomPassword });

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