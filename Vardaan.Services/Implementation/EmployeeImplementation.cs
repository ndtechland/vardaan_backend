using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.Domain.DTO;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;
using VardaanCab.DataAccessLayer.DataLayer;
using System.Linq;

namespace Vardaan.Services.Implementation
{
    public class EmployeeImplementation:IEmployee
    {
        private const string GoogleMapsApiKey = "AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";
        Vardaan_AdminEntities ent =new Vardaan_AdminEntities();

        public async Task<string> ManageEmployee(EmployeeDTO model)
        {
			try
			{
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
                        command.Parameters.Add(new EntityParameter("AlternateNumber", DbType.String) { Value = model.AlternateNumber });
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
                        //command.Parameters.Add(new EntityParameter("Password", DbType.String) { Value = null });
                        command.Parameters.Add(new EntityParameter("Latitude", DbType.Double) { Value = latitude });
                        command.Parameters.Add(new EntityParameter("Longitude", DbType.Double) { Value = longitude });

                        var responseMessageParam = new EntityParameter("ResponseMessage", DbType.String) { Direction = ParameterDirection.Output, Size = 255 };
                        command.Parameters.Add(responseMessageParam);

                        command.ExecuteNonQuery();

                        string responseMessage = responseMessageParam.Value.ToString();

                        return responseMessage;
                    }
                }
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<string> DeleteEmployee(int id)
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

                        return responseMessage;
                    }
                }
            }
			catch (Exception)
			{

				throw;
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
        public async Task <List<employeedetail>> GetEmployees()
        {
            try
            {
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

                return employeedetailList;
            }
            catch (Exception)
            {

                throw;
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
    }
}
