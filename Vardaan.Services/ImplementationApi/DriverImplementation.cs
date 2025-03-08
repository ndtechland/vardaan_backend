using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class DriverImplementation:IDriver
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<DriverProfileDTO> GetProfile(int id)
        {
			try
			{
                var driver = ent.Drivers.SingleOrDefault(d => d.Id == id && d.IsActive);
                var model = new DriverProfileDTO
                {
                    Id = driver.Id,
                    DriverName = driver.DriverName,
                    Address = driver.DriverAddress,
                    Email = driver.Email,
                    MobileNumber = driver.MobileNumber,
                    DlImage = driver.DlImage,
                    DriverImage = driver.DriverImage,
                    DlNumber = driver.DlNumber,
                    CreateDate = driver.CreateDate,
                    AlternateNo1 = driver.AlternateNo1,
                };
                return model;
            }
            catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> UpdateProfile(DriverProfileDTO model)
        {
            try
            {
                var checkdriver = ent.Drivers.Find(model.Id);
                if (checkdriver != null)
                {
                    checkdriver.DriverName = model.DriverName;
                    checkdriver.Email = model.Email;
                    checkdriver.DriverAddress = model.Address;
                    checkdriver.AlternateNo1 = model.AlternateNo1;
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
        public async Task<bool> UpdateActiveStatus(DriverDTO model)
        {
            try
            {
                var data = ent.Drivers.Where(d => d.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.IsOnline = model.IsOnline;
                    ent.SaveChanges();
                    if (model.IsOnline == true)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
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
        public async Task<List<TrackCabEmployeePickupViewModel>> GetTrackCabEmployee(long driverId)
        {
            var results = new List<TrackCabEmployeePickupViewModel>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (var command = new SqlCommand("GetTrackCabEmployees", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.BigInt) { Value = driverId });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new TrackCabEmployeePickupViewModel
                                {
                                    ID = (int)reader.GetInt32(0),
                                    RouteDate = reader.GetDateTime(1),
                                    EmployeeName = reader.GetString(2),
                                    CompanyName = reader.GetString(3),
                                    TripTypeName = reader.GetString(4),
                                    Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    ShiftTime = reader.IsDBNull(6) ? null : reader.GetString(6)
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
        public async Task<List<TrackEmployeeLocationModel>> GetTrackEmployeeLocationIndividually(long driverId,int Id)
        {
            var results = new List<TrackEmployeeLocationModel>();

            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
                var sqlConnectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                using (var sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (var command = new SqlCommand("TrackEmployeeLocation", sqlConnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.BigInt) { Value = driverId });
                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Id });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new TrackEmployeeLocationModel
                                {
                                    ID = (int)reader.GetInt32(0),
                                    RouteDate = reader.GetDateTime(1),
                                    EmployeeName = reader.GetString(2),
                                    CompanyName = reader.GetString(3),
                                    TripTypeName = reader.GetString(4),
                                    PickupLocation = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Destination = reader.IsDBNull(6) ? null : reader.GetString(6)
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
