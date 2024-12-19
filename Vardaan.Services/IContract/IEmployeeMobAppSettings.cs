using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IEmployeeMobAppSettings
    {
        Task<List<CabReqDays>> GetCabRequestDayList();
        Task<bool> AddCabRequestDays(EmployeeMobAppSettingDTO model);
        Task<bool> DeletedRequestDay(int id);
    }
}
