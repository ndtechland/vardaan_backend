using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface ICompanyZone
    {
        Task<bool> AddUpdateZone(CompanyZoneDTO model);
        Task<List<CompanyZoneList>> GetZones();
        Task<bool> DeleteZone(int id);
        Task<bool> AddUpdateHomeRoute(EmployeeHomeRouteDTO model);
        Task<List<HomeRouteList>> GetHomeRoutes();
        Task<bool> DeleteHomeRoute(int id);
        Task<bool> AddUpdateDestinationArea(EmployeeDestinationAreaDTO model);
        Task<List<DestinationAreaList>> GetDestinationAreas();
        Task<bool> DeleteDestinationArea(int id);
    }
}
