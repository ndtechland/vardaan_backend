using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IETS
    {
        Task<bool> AddUpdateRequest(CreateRequestDTO model);
        Task<List<EmployeeRequests>> GetEmployeerequests();
        Task<bool> DeleteRequest(int id);
        Task<List<AvailableDriverDTO>> GetAvailableDrivers();
        Task<List<Escorts>> GetChechinEscort();
        Task<List<Escorts>> GetEscortAvailable();
        Task<bool> AddDriverCheckoutRemark(DriverCheckoutRemarkModel model);
        Task<string> AddVehicleInspection(VehicleInspectionDTO model, int userId);
        Task<RoutingCabAllCounts> GetRoutingListByTerms(string term);
    }
}
