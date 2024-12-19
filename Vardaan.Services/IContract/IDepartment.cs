using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IDepartment
    {
        Task<bool> AddDepartments(DepartmentMasterDTO model);
        Task<bool> DeleteDepartment(int id);
        Task<List<DepartmentList>> DepartmentList();
    }
}
