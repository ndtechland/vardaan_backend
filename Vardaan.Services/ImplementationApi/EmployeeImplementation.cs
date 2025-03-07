using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class EmployeeImplementation : IEmployee
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public async Task<EmployeeProfileDTO> GetProfileDetail(int id)
        {
            try
            {
                var empdata = ent.Employees.SingleOrDefault(x => x.Id == id && x.IsActive == true);

                if (empdata != null)
                {
                    var getstate = ent.StateMasters.Where(s => s.Id == empdata.StateId).FirstOrDefault();
                    var getcity = ent.CityMasters.Where(s => s.Id == empdata.CityId).FirstOrDefault();

                    var data = new EmployeeProfileDTO()
                    {
                        Id = empdata.Id,
                        Employee_First_Name = empdata.Employee_First_Name,
                        Employee_Middle_Name = empdata.Employee_Middle_Name,
                        Employee_Last_Name = empdata.Employee_Last_Name,
                        Email = empdata.Email,
                        MobileNumber = empdata.MobileNumber,
                        EmergencyContactNumber = empdata.AlternateNumber,
                        Employee_Id = empdata.Employee_Id,
                        EmployeeDepartment = empdata.EmployeeDepartment,
                        StateId = empdata.StateId,
                        CityId = empdata.CityId,
                        StateName = getstate.StateName,
                        CityName = getcity.CityName,
                        Pincode = empdata.Pincode,
                        CreatedDate = empdata.CreatedDate,
                        EmployeeCurrentAddress = empdata.EmployeeCurrentAddress,
                        Gender = empdata.Gender,
                        Company_Id = empdata.Company_Id,
                    };
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> UpdateProfile(EmployeeProfileDTO model)
        {
            try
            {
                var emp = ent.Employees.Find(model.Id);
                if (emp != null)
                {
                    emp.Employee_First_Name = model.Employee_First_Name;
                    emp.Employee_Middle_Name = model.Employee_Middle_Name;
                    emp.Employee_Last_Name = model.Employee_Last_Name;
                    emp.Email = model.Email;
                    //emp.MobileNumber = model.MobileNumber;
                    emp.EmployeeCurrentAddress = model.EmployeeCurrentAddress;
                    emp.StateId = model.StateId;
                    emp.CityId = model.CityId;
                    emp.Pincode = model.Pincode;
                    emp.AlternateNumber = model.EmergencyContactNumber;
                    ent.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> Addhelp(HelpEmployee model)
        {
            try
            {
                var data = new HelpEmployee()
                {
                    Employee_id = model.Employee_id,
                    PhoneNumber = model.PhoneNumber,
                    Reason = model.Reason,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                ent.HelpEmployees.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddFeedback(FeedBackEmployee model)
        {
            try
            {
                var data = new FeedBackEmployee()
                {
                    Employee_Id = model.Employee_Id,
                    Feedback = model.Feedback,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                ent.FeedBackEmployees.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<EmployeeBookingDTO>> GetCabUpcommingListByEmployeeId(string employeeId)
        {
            var results = new List<EmployeeBookingDTO>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (var command = new SqlCommand("GetUpcomingCab", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = employeeId });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new EmployeeBookingDTO
                                {
                                    ID = reader.GetInt32(0),
                                    Date = reader.GetDateTime(1),
                                    VehicleNumber = reader.GetString(2),
                                    CabId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                    CabName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    TripTypeName = reader.GetString(5),
                                    PickupShiftTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    DropShiftTime = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    CompanyName = reader.GetString(8),
                                    PickupLocation = reader.GetString(9),
                                    DropLocation = reader.GetString(10),
                                    Employee_Id = reader.GetString(11)
                                });
                            }
                        }

                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching routing list", ex);
            }
        }
        public async Task<List<LiveCabs>> GetLiveCabByEmployeeId(string employeeId)
        {
            var results = new List<LiveCabs>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (var command = new SqlCommand("GetLiveCab", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = employeeId });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new LiveCabs
                                {
                                    RoutingID = reader.GetInt32(0),
                                    Date = reader.GetDateTime(1),
                                    VehicleNumber = reader.GetString(2),
                                    CabId = reader.GetInt32(3),
                                    CabName = reader.GetString(4),
                                    TripTypeName = reader.GetString(5),
                                    PickupShiftTime = reader.GetString(6),
                                    DropShiftTime = reader.GetString(7),
                                    CompanyName = reader.GetString(8),
                                    PickupLocation = reader.IsDBNull(9) ? null : reader.GetString(8),
                                    DropLocation = reader.IsDBNull(10) ? null : reader.GetString(9),
                                    DriverId = reader.GetInt32(11)
                                });
                            }
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching routing list", ex);
            }
        }
        public async Task<List<FinishCabs>> GetFinishCabBookingHisByEmployeeId(string employeeId)
        {
            var results = new List<FinishCabs>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (var command = new SqlCommand("GetFinishCabBookingHistory", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = employeeId });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new FinishCabs
                                {
                                    StartDate = reader.GetDateTime(0),
                                    EndDate = reader.GetDateTime(1),
                                    CompanyName = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching routing list", ex);
            }
        }
        


    }
}