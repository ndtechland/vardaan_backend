using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IArea
    {
        Task<bool> AddUpdateArea(AreaMasterDTO model);
        Task<List<AreaMasterDTO>> GetAreas();
        Task<bool> DeleteArea(int id);
    }
}
