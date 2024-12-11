using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace VardaanCab.Domain.DTO
{
    public class AccessAssignDTO
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> UserRoleId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public int[] IsReadChecked { get; set; }
        public int[] IsWriteChecked { get; set; }
        public int[] IsSubReadChecked { get; set; }
        public int[] IsSubWriteChecked { get; set; }
        public string ReadPermissions { get; set; }
        public string WritePermissions { get; set; }
        public bool IsAllRead { get; set; }
        public bool IsAllWrite { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<SoftwareLinkDTO> SoftwareLinkDTO { get; set; }
        public IEnumerable<ListAccessAssign> AccessAssignList { get; set; }
    }
    public class ListAccessAssign
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CompanyName { get; set; }
    }
}
