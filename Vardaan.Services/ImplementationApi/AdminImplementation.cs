﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class AdminImplementation:IAdmin
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public async Task<AdminLoginDTO> GetProfile(int Id)
        {
            try
            {
                var data = (from c in ent.Customers
                            join e in ent.Employees on c.Id equals e.Company_Id
                            join aa in ent.AccessAssigns on e.Id equals aa.EmployeeId
                            join r in ent.UserRoles on aa.UserRoleId equals r.Id
                            where e.Id == Id && e.IsActive == true
                            select new AdminLoginDTO
                            {
                                Id = e.Id,
                                TransportCode = c.OrgName,
                                EmployeeName = e.Employee_First_Name + e.Employee_Middle_Name + e.Employee_Last_Name,
                                MobileNumber = e.MobileNumber,
                                Email = e.Email,
                                Address = e.EmployeeCurrentAddress,
                                UserRole = r.RoleName,
                            }
                              ).FirstOrDefault();
                if (data != null)
                {
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
        public async Task<(List<AvailableDriverDTO> AvailableDriversList, string VendorName)> GetAvailableDrivers(string Transpostcode, int VendorId)
        {
            try
            {
                var driver = await (from d in ent.Drivers
                                    join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                                    join c in ent.Cabs on dl.VehicleNumber equals c.VehicleNumber
                                    join cm in ent.VehicleModels on c.VehicleModel_Id equals cm.Id
                                    join v in ent.Vendors on d.Vendor_Id equals v.Id
                                    join di in ent.DriverDeviceIds on d.DeviceId equals di.Id
                                    join com in ent.Customers on v.Company_Id equals com.Id
                                    where dl.IsActive == true && com.OrgName == Transpostcode && v.Id== VendorId
                                    select new AvailableDriverDTO
                                    {
                                        CheckInId = dl.Id,
                                        DriverId = d.Id,
                                        DeviceId = d.DeviceId,
                                        DriverName = d.DriverName,
                                        MobileNumber = d.MobileNumber,
                                        VehicleNumber = c.VehicleNumber,
                                        VehicleModel = cm.ModelName,
                                    }).ToListAsync();

                 
                 // Fetch vendor name
                var vendorName = await ent.Vendors
                                          .Where(v => v.Id == VendorId && v.IsActive == true)
                                          .Select(v => v.CompanyName)
                                          .FirstOrDefaultAsync();

                return (driver, vendorName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableDrivers: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> AddDriverCheckoutRemark(DriverCheckoutRemarkModel model)
        {
            try
            {
                var data = new DriverCheckoutRemark()
                {
                    Driver_Id = model.DriverId,
                    Remark = model.Remark,
                    RemarkDate = DateTime.Now
                };
                ent.DriverCheckoutRemarks.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<(List<AvailableDriverDTO> CheckInDriversList, string VendorName,int Vendor_Id)> GetCheckInDrivers(string Transpostcode, int VendorId)
        {
            try
            {
                var driver = await (from d in ent.Drivers
                                    join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                                    join c in ent.Cabs on dl.VehicleNumber equals c.VehicleNumber
                                    join cm in ent.VehicleModels on c.VehicleModel_Id equals cm.Id
                                    join v in ent.Vendors on d.Vendor_Id equals v.Id
                                    join di in ent.DriverDeviceIds on d.DeviceId equals di.Id
                                    join com in ent.Customers on v.Company_Id equals com.Id
                                    where dl.IsActive == true && com.OrgName == Transpostcode && v.Id == VendorId
                                    select new AvailableDriverDTO
                                    {
                                        CheckInId = dl.Id,
                                        DriverId = d.Id,
                                        DeviceId=d.DeviceId,
                                        DriverName = d.DriverName,
                                        MobileNumber = d.MobileNumber,
                                        VehicleNumber = c.VehicleNumber,
                                    }).ToListAsync();
               
                
                var vendor = await ent.Vendors
                      .Where(v => v.Id == VendorId && v.IsActive)
                      .Select(v => new
                      {
                          v.Id,
                          v.CompanyName
                      })
                      .FirstOrDefaultAsync();

                return (driver, vendor.CompanyName, vendor.Id);
                 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCheckinDrivers: {ex.Message}");
                throw;
            }
        }
        public async Task<List<VendorList>> GetVendorByCompany(string TransportCode)
        {
            try
            {
                var data = (from v in ent.Vendors
                            join c in ent.Customers on v.Company_Id equals c.Id
                            where v.IsActive && c.OrgName == TransportCode
                            select new VendorList
                            {
                                Vendor_Id = v.Id,
                                VendorName = v.VendorName,
                                CompanyName = v.CompanyName
                            }
                          ).ToList();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }       
        public async Task<(List<Drivers> DriversList, string VendorName)> GetDriverByTransportCodeVendorId(string TransportCode, int VendorId)
        {
            try
            {
                var driver = await (from d in ent.Drivers
                                    join v in ent.Vendors on d.Vendor_Id equals v.Id
                                    join com in ent.Customers on v.Company_Id equals com.Id
                                    where d.IsActive == true && v.IsActive == true && com.IsActive == true && com.OrgName == TransportCode && v.Id == VendorId
                                    select new Drivers
                                    {
                                        Driver_Id = d.Id,
                                        DriverName = d.DriverName,
                                        MobileNumber = d.MobileNumber,
                                    }).ToListAsync();

                // Fetch vendor name
                var vendorName = await ent.Vendors
                                          .Where(v => v.Id == VendorId && v.IsActive == true)
                                          .Select(v => v.CompanyName)
                                          .FirstOrDefaultAsync();

                return (driver, vendorName);
            }
            catch (Exception ex)
            {
                // Handle exception
                throw new Exception("An error occurred while retrieving vehicles and vendor name.", ex);
            }
        }public async Task<(List<Vehicles> VehiclesList, string VendorName)> GetVehiclesByTransportCodeVendorId(string TransportCode, int VendorId)
        {
            try
            {
                var cabs = await (from c in ent.Cabs
                                  join vm in ent.VehicleModels on c.VehicleModel_Id equals vm.Id
                                  join v in ent.Vendors on c.Vendor_Id equals v.Id
                                  join com in ent.Customers on v.Company_Id equals com.Id
                                  where c.IsActive == true && v.IsActive == true && com.IsActive == true
                                        && com.OrgName == TransportCode && v.Id == VendorId
                                  select new Vehicles
                                  {
                                      Vehicle_Id = c.Id,
                                      ModelName = vm.ModelName,
                                      VehicleNumber = c.VehicleNumber,
                                  }).ToListAsync();

                // Fetch vendor name
                var vendorName = await ent.Vendors
                                          .Where(v => v.Id == VendorId && v.IsActive == true)
                                          .Select(v => v.CompanyName)
                                          .FirstOrDefaultAsync();

                return (cabs, vendorName);
            }
            catch (Exception ex)
            {
                // Handle exception
                throw new Exception("An error occurred while retrieving vehicles and vendor name.", ex);
            }
        }
        public async Task<List<GetMobileNumbers>> GetDriverMobNo(int VendorId)
        {
            try
            {                 
                return await ent.Drivers.Where(x => x.IsActive && x.Vendor_Id== VendorId && (x.IsLogin==false || x.IsLogin==null))
                    .Select(x => new GetMobileNumbers
                    {
                        Driver_Id = x.Id,
                        MobileNumber = x.MobileNumber
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<GetDriverName> GetDriverNameByDriverId(int Driverid)
        {
            try
            {
                var data = ent.Drivers.Where(x => x.IsActive && x.Id == Driverid).Select(x => new GetDriverName
                {
                    Driver_Id = x.Id,
                    DriverName = x.DriverName
                }).FirstOrDefault();

                if (data!=null)
                {
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
        public async Task<AvailableDriverDTO> GetCheckInDriverDetail(int id)
        {
            try
            {
                var driver = await (from d in ent.Drivers
                                    join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                                    join c in ent.Cabs on dl.VehicleNumber equals c.VehicleNumber
                                    join cm in ent.VehicleModels on c.VehicleModel_Id equals cm.Id
                                    join v in ent.Vendors on d.Vendor_Id equals v.Id
                                    join di in ent.DriverDeviceIds on d.DeviceId equals di.Id
                                    join com in ent.Customers on v.Company_Id equals com.Id
                                    where dl.IsActive == true && dl.Id == id
                                    select new AvailableDriverDTO
                                    {
                                        CheckInId = dl.Id,
                                        DriverId = d.Id,
                                        DeviceId = d.DeviceId,
                                        DriverName = d.DriverName,
                                        MobileNumber = d.MobileNumber,
                                        VehicleNumber = c.VehicleNumber,
                                    }).FirstOrDefaultAsync();


                return driver;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCheckinDrivers: {ex.Message}");
                throw;
            }
        }
       // get not checkin vehicle number based on vendor
        public async Task<List<VehicleNumbers>> GetVehicleNo(int VendorId)
        {
            try
            {
                return await ent.Cabs.Where(x => x.IsActive && x.Vendor_Id==VendorId && (x.IsLogin == false || x.IsLogin == null))
                    .Select(x => new VehicleNumbers
                    {
                        Vehicle_Id = x.Id,
                        VehicleNumber = x.VehicleNumber
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<List<DeviceIds>> GetDeviceId()
        {
            try
            {
                return await ent.DriverDeviceIds.Select(x => new DeviceIds
                    {
                        Device_Id = x.Id
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> UpdateCheckinDriver(AvailableDriverDTO model)
        {
            try
            {
                var checkininfo = ent.DriverLoginHistories.FirstOrDefault(x => x.Id == model.CheckInId);
                if (checkininfo == null)
                {
                    return false;
                }

                // Set previous cab's login status to false
                var oldCab = ent.Cabs.FirstOrDefault(x => x.VehicleNumber == checkininfo.VehicleNumber);
                if (oldCab != null)
                {
                    oldCab.IsLogin = false;
                }

                // Set previous driver's login status to false
                var oldDriver = ent.Drivers.FirstOrDefault(x => x.Id == checkininfo.DriverId);
                if (oldDriver != null)
                {
                    oldDriver.IsLogin = false;
                }

                // Update check-in details with new vehicle and driver
                checkininfo.DriverId = model.DriverId;
                checkininfo.VehicleNumber = model.VehicleNumber;

                // Set new cab's login status to true
                var newCab = ent.Cabs.FirstOrDefault(x => x.VehicleNumber == model.VehicleNumber);
                if (newCab != null)
                {
                    newCab.IsLogin = true;
                }

                // Set new driver's login status to true
                var newDriver = ent.Drivers.FirstOrDefault(x => x.Id == model.DriverId);
                if (newDriver != null)
                {
                    newDriver.IsLogin = true;
                }

                await ent.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AddCheckinDriverVehicle(AvailableDriverDTO model)
        {
            try
            {
                var driverinfo = ent.Drivers.Find(model.DriverId);

                var cab = ent.Cabs.FirstOrDefault(x => x.VehicleNumber == model.VehicleNumber && x.IsActive == true);
                var activeHistories = ent.DriverLoginHistories.Where(x => x.DriverId == model.DriverId && x.IsActive == true).ToList();

                foreach (var history in activeHistories)
                {
                    history.IsActive = false;
                }
                ent.SaveChanges();

                if (cab != null)
                {
                    cab.IsLogin = true;
                    ent.SaveChanges();
                }
                if (driverinfo != null)
                {
                    cab.IsLogin = true;
                    ent.SaveChanges();
                }

                var data = new DriverLoginHistory()
                {
                    VehicleNumber = model.VehicleNumber,
                    DriverId = model.DriverId,
                    IsActive = true,
                    LoginDate = DateTime.Now
                };
                ent.DriverLoginHistories.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> AddVehicleInspection(VehicleInspectionDTO model)
        {
            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;

                using (var entityConnection = new EntityConnection(entityConnectionString))
                {
                    entityConnection.Open();
                    using (var command = entityConnection.CreateCommand())
                    {
                        command.CommandText = "Vardaan_AdminEntities.ManageVehicleInspection";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new EntityParameter("Vendor_Id", DbType.Int32) { Value = model.Vendor_Id });
                        command.Parameters.Add(new EntityParameter("Vehicle_Id", DbType.Int32) { Value = model.Vehicle_Id });
                        command.Parameters.Add(new EntityParameter("InspectionDate", DbType.DateTime) { Value = model.InspectionDate });
                        command.Parameters.Add(new EntityParameter("AC_Working", DbType.Boolean) { Value = model.AC_Working });
                        command.Parameters.Add(new EntityParameter("AC_Remarks", DbType.String) { Value = model.AC_Remarks ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("UnderInfluence", DbType.Boolean) { Value = model.UnderInfluence });
                        command.Parameters.Add(new EntityParameter("UnderInfluence_Remarks", DbType.String) { Value = model.UnderInfluence_Remarks });
                        command.Parameters.Add(new EntityParameter("Wiper_Seasonal", DbType.Boolean) { Value = model.Wiper_Seasonal });
                        command.Parameters.Add(new EntityParameter("Wiper_Remarks", DbType.String) { Value = model.Wiper_Remarks });
                        command.Parameters.Add(new EntityParameter("National_Permit", DbType.Boolean) { Value = model.National_Permit });
                        command.Parameters.Add(new EntityParameter("NationalPermit_Remarks", DbType.String) { Value = model.NationalPermit_Remarks });
                        command.Parameters.Add(new EntityParameter("Windshield_Broken", DbType.Boolean) { Value = model.Windshield_Broken });
                        command.Parameters.Add(new EntityParameter("Windshield_Remarks", DbType.String) { Value = model.Windshield_Remarks });
                        command.Parameters.Add(new EntityParameter("Trip_Type", DbType.Int32) { Value = model.Trip_Type });
                        command.Parameters.Add(new EntityParameter("Visible_Body_Dent", DbType.Boolean) { Value = model.Visible_Body_Dent });
                        command.Parameters.Add(new EntityParameter("BodyDent_Remarks", DbType.String) { Value = model.BodyDent_Remarks });
                        command.Parameters.Add(new EntityParameter("Seat_Belts_Working", DbType.Boolean) { Value = model.Seat_Belts_Working });
                        command.Parameters.Add(new EntityParameter("SeatBelts_Remarks", DbType.String) { Value = model.SeatBelts_Remarks });
                        command.Parameters.Add(new EntityParameter("GPS_Not_Available", DbType.Boolean) { Value = model.GPS_Not_Available });
                        command.Parameters.Add(new EntityParameter("GPS_Remarks", DbType.String) { Value = model.GPS_Remarks ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("State_Permit", DbType.Boolean) { Value = model.State_Permit });
                        command.Parameters.Add(new EntityParameter("StatePermit_Remarks", DbType.String) { Value = model.StatePermit_Remarks });
                        command.Parameters.Add(new EntityParameter("Unregistered_Drivers", DbType.Boolean) { Value = model.Unregistered_Drivers });
                        command.Parameters.Add(new EntityParameter("UnregisteredDrivers_Remarks", DbType.String) { Value = model.UnregisteredDrivers_Remarks });
                        command.Parameters.Add(new EntityParameter("Shift_Time", DbType.Int64) { Value = model.Shift_Time });
                        command.Parameters.Add(new EntityParameter("Dirty_Unclean_Vehicle", DbType.Boolean) { Value = model.Dirty_Unclean_Vehicle });
                        command.Parameters.Add(new EntityParameter("UncleanVehicle_Remarks", DbType.String) { Value = model.UncleanVehicle_Remarks });
                        command.Parameters.Add(new EntityParameter("Seat_Cover", DbType.Boolean) { Value = model.Seat_Cover });
                        command.Parameters.Add(new EntityParameter("SeatCover_Remarks", DbType.String) { Value = model.SeatCover_Remarks });
                        command.Parameters.Add(new EntityParameter("Headlights_Indicators", DbType.Boolean) { Value = model.Headlights_Indicators });
                        command.Parameters.Add(new EntityParameter("Headlights_Remarks", DbType.String) { Value = model.Headlights_Remarks });
                        command.Parameters.Add(new EntityParameter("Insurance", DbType.Boolean) { Value = model.Insurance });
                        command.Parameters.Add(new EntityParameter("Insurance_Remarks", DbType.String) { Value = model.Insurance_Remarks });
                        command.Parameters.Add(new EntityParameter("Unregistered_Cab", DbType.Boolean) { Value = model.Unregistered_Cab });
                        command.Parameters.Add(new EntityParameter("UnregisteredCab_Remarks", DbType.String) { Value = model.UnregisteredCab_Remarks });
                        command.Parameters.Add(new EntityParameter("City_Name", DbType.String) { Value = model.City_Name });
                        command.Parameters.Add(new EntityParameter("Driver_Uniform", DbType.Boolean) { Value = model.Driver_Uniform });
                        command.Parameters.Add(new EntityParameter("DriverUniform_Remarks", DbType.String) { Value = model.DriverUniform_Remarks });
                        command.Parameters.Add(new EntityParameter("Spare_Wheel", DbType.Boolean) { Value = model.Spare_Wheel });
                        command.Parameters.Add(new EntityParameter("SpareWheel_Remarks", DbType.String) { Value = model.SpareWheel_Remarks });
                        command.Parameters.Add(new EntityParameter("RC_Book", DbType.Boolean) { Value = model.RC_Book });
                        command.Parameters.Add(new EntityParameter("RCBook_Remarks", DbType.String) { Value = model.RCBook_Remarks });
                        command.Parameters.Add(new EntityParameter("Pollution", DbType.Boolean) { Value = model.Pollution });
                        command.Parameters.Add(new EntityParameter("Pollution_Remarks", DbType.String) { Value = model.Pollution_Remarks });
                        command.Parameters.Add(new EntityParameter("Penalty_Amount", DbType.String) { Value = model.Penalty_Amount });
                        command.Parameters.Add(new EntityParameter("Feedback", DbType.String) { Value = model.Feedback });
                        command.Parameters.Add(new EntityParameter("Fire_Extinguisher", DbType.Boolean) { Value = model.Fire_Extinguisher });
                        command.Parameters.Add(new EntityParameter("FireExtinguisher_Remarks", DbType.String) { Value = model.FireExtinguisher_Remarks });
                        command.Parameters.Add(new EntityParameter("Tool_Kit", DbType.Boolean) { Value = model.Tool_Kit });
                        command.Parameters.Add(new EntityParameter("ToolKit_Remarks", DbType.String) { Value = model.ToolKit_Remarks });
                        command.Parameters.Add(new EntityParameter("Fitness", DbType.Boolean) { Value = model.Fitness });
                        command.Parameters.Add(new EntityParameter("Fitness_Remarks", DbType.String) { Value = model.Fitness_Remarks });
                        command.Parameters.Add(new EntityParameter("Commercial_License", DbType.Boolean) { Value = model.Commercial_License });
                        command.Parameters.Add(new EntityParameter("CommercialLicense_Remarks", DbType.String) { Value = model.CommercialLicense_Remarks });
                        command.Parameters.Add(new EntityParameter("Penalty_Description", DbType.String) { Value = model.Penalty_Description });
                        command.Parameters.Add(new EntityParameter("First_Aid_Box", DbType.Boolean) { Value = model.First_Aid_Box });
                        command.Parameters.Add(new EntityParameter("FirstAidBox_Remarks", DbType.String) { Value = model.FirstAidBox_Remarks });
                        command.Parameters.Add(new EntityParameter("Fog_Lamp", DbType.String) { Value = model.Fog_Lamp });
                        command.Parameters.Add(new EntityParameter("FogLamp_Remarks", DbType.String) { Value = model.FogLamp_Remarks });
                        command.Parameters.Add(new EntityParameter("Passenger_Tax", DbType.Boolean) { Value = model.Passenger_Tax });
                        command.Parameters.Add(new EntityParameter("PassengerTax_Remarks", DbType.String) { Value = model.PassengerTax_Remarks });
                        command.Parameters.Add(new EntityParameter("Vehicle_Model_Over_5_Years", DbType.String) { Value = model.Vehicle_Model_Over_5_Years });
                        command.Parameters.Add(new EntityParameter("VehicleModel_Remarks", DbType.String) { Value = model.VehicleModel_Remarks });
                        command.Parameters.Add(new EntityParameter("Total_NC_Count", DbType.String) { Value = model.Total_NC_Count });
                        command.Parameters.Add(new EntityParameter("IsActive", DbType.Boolean) { Value = true });
                        command.Parameters.Add(new EntityParameter("InspectionByEmployeeId", DbType.String) { Value = model.InspectionByEmployeeId });
                        command.Parameters.Add(new EntityParameter("AdhocTime", DbType.String) { Value = model.AdhocTime });
                        command.Parameters.Add(new EntityParameter("TripTypeMaster", DbType.String) { Value = model.TripTypeMaster });

                        var responseMessageParam = new EntityParameter("ResponseMessage", DbType.String) { Direction = ParameterDirection.Output, Size = 255 };
                        command.Parameters.Add(responseMessageParam);

                        command.ExecuteNonQuery();

                        string responseMessage = responseMessageParam.Value.ToString();

                        return responseMessage;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<TripMaster>> GetTripTypeMaster()
        {
            try
            {
                var data = ent.TripMasters.ToListAsync();
                return await data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<VehicleNumbers>> GetVehicleNoByVendor(int VendorId)
        {
            try
            {
                return await ent.Cabs.Where(x => x.IsActive && x.IsLogin==true && x.Vendor_Id == VendorId)
                    .Select(x => new VehicleNumbers
                    {
                        Vehicle_Id = x.Id,
                        VehicleNumber = x.VehicleNumber
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<GetDriverName> GetDriverNameByVehicleNumber(string VehicleNumber)
        {
            try
            {
                var data = (from d in ent.Drivers
                          join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                          where d.IsActive && d.IsLogin==true && dl.IsActive==true && dl.VehicleNumber== VehicleNumber
                            select new GetDriverName
                          {
                              DriverName=d.DriverName,
                              Driver_Id=d.Id
                          }
                        ).FirstOrDefault();
               
                if (data != null)
                {
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
        public async Task<List<TripType>> GetTriptype()
        {
            try
            {
                var data = ent.TripTypes.Where(x => x.TripMasterId == 1 && x.Id!=3).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<RoutingDTO>> GetRoutingListByTerms(string term)
        {
            try
            {
                int? routeId = null;
                if (int.TryParse(term, out int parsedRouteId))
                {
                    routeId = parsedRouteId;
                }

                var dd = await (from r in ent.Routings
                                join ar in ent.AllRoutes on r.ID equals ar.Routing_Id
                                join c in ent.Cabs on ar.CabNumber equals c.VehicleNumber
                                join e in ent.Employees on ar.Employee_Id equals e.Employee_Id
                                join z in ent.CompanyZones on ar.RouteNameId equals z.Id
                                join v in ent.Vendors on c.Vendor_Id equals v.Id
                                where e.Employee_Id == term
                                      || c.VehicleNumber == term
                                      || c.StickerNumber == term
                                      || (routeId.HasValue && ar.RouteId == routeId.Value)
                                select new RoutingDTO
                                {
                                    RouteId = ar.RouteId,
                                    VehicleNumber = c.VehicleNumber,
                                    ZoneName = z.CompanyZone1,
                                    VendorName = v.CompanyName,
                                    AvailableSeat = ar.AvailableSeats,
                                }).ToListAsync();

                return dd;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
