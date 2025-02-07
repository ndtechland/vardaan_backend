using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public async Task<(List<AvailableDriverDTO> CheckInDriversList, string VendorName)> GetCheckInDrivers(string Transpostcode, int VendorId)
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
                // Fetch vendor name
                var vendorName = await ent.Vendors
                                          .Where(v => v.Id == VendorId && v.IsActive == true)
                                          .Select(v => v.CompanyName)
                                          .FirstOrDefaultAsync();

                return (driver, vendorName);
                 
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
        public async Task<List<GetMobileNumbers>> GetDriverMobNo()
        {
            try
            {
                return await ent.Drivers.Where(x => x.IsActive)
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
        public async Task<List<VehicleNumbers>> GetVehicleNo()
        {
            try
            {
                return await ent.Cabs.Where(x => x.IsActive)
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
    }
}
