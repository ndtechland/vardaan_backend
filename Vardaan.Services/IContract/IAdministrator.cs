using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.IContract
{
    public interface IAdministrator
    {
        Task<bool> AddUpdateAssignAccess(AccessAssignDTO model);
        Task<List<ListAccessAssign>> GetAccessAssigns();
        Task<List<UserRoleList>> GetRoles();
        Task<bool> AddupdateRole(UserRoleDTO model);
        Task<bool> DeleteRole(int id);
    }
}
