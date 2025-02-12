using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.IContractApi
{
    public interface IAdmin
    {
        Task<AdminLoginDTO> GetProfile(int Id);
        Task<(List<AvailableDriverDTO> AvailableDriversList, string VendorName)> GetAvailableDrivers(string Transpostcode, int VendorId);
        Task<(List<AvailableDriverDTO> CheckInDriversList, string VendorName, int Vendor_Id)> GetCheckInDrivers(string Transpostcode, int VendorId);
        Task<List<VendorList>> GetVendorByCompany(string TransportCode);
        Task<(List<Drivers> DriversList, string VendorName)> GetDriverByTransportCodeVendorId(string TransportCode, int VendorId);

        Task<(List<Vehicles> VehiclesList, string VendorName)> GetVehiclesByTransportCodeVendorId(string TransportCode, int VendorId);
        Task<List<GetMobileNumbers>> GetDriverMobNo(int VendorId);
        Task<GetDriverName> GetDriverNameByDriverId(int Driverid);
        Task<AvailableDriverDTO> GetCheckInDriverDetail(int id);
        Task<List<VehicleNumbers>> GetVehicleNo(int VendorId);
        Task<List<DeviceIds>> GetDeviceId();
        Task<bool> AddDriverCheckoutRemark(DriverCheckoutRemarkModel model);
        Task<bool> UpdateCheckinDriver(AvailableDriverDTO model);
        Task<bool> AddCheckinDriverVehicle(AvailableDriverDTO model);
        Task<string> AddVehicleInspection(VehicleInspectionDTO model);
    }
}
