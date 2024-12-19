using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IEscort
    {
        Task<bool> AddUpdateEscort(EscortDTO model);
        Task<bool> DeleteEscort(int id);
        Task<List<Escorts>> GetEscorts();
    }
}
