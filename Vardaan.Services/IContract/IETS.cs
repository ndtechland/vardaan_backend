using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IETS
    {
        Task<bool> AddUpdateRequest(CreateRequestDTO model);
        Task<List<EmployeeRequests>> GetEmployeerequests();
        Task<bool> DeleteRequest(int id);
    }
}
