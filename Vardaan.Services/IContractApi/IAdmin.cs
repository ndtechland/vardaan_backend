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
        Task<List<AvailableDriverDTO>> GetAvailableDrivers(string Transpostcode, int VendorId);
        Task<List<VendorList>> GetVendorByCompany(string TransportCode);
        Task<(List<Drivers> DriversList, string VendorName)> GetDriverByTransportCodeVendorId(string TransportCode, int VendorId);

        Task<(List<Vehicles> VehiclesList, string VendorName)> GetVehiclesByTransportCodeVendorId(string TransportCode, int VendorId);
    }
}
