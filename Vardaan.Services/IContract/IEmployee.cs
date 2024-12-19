using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IEmployee
    {
        Task<string> ManageEmployee(EmployeeDTO model);
        Task<List<employeedetail>> GetEmployees();
        Task<string> DeleteEmployee(int id);
    }
}
