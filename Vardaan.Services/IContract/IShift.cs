using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IShift
    {
        Task<List<GetShift>> GetShifts();
        Task<bool> AddUpdateShift(ShiftMaster model);
        Task<bool> DeleteShift(int id);
    }
}
