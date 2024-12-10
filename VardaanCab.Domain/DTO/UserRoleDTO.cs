using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        //public SelectList Companies { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; }

        public int[] IsReadChecked { get; set; } = new int[0];
        public int[] IsWriteChecked { get; set; } = new int[0];
        public int[] IsSubReadChecked { get; set; } = new int[0];
        public int[] IsSubWriteChecked { get; set; } =new int[0];
        public string ReadPermissions { get; set; }
        public string WritePermissions { get; set; }
        public bool IsAllRead { get; set; }
        public bool IsAllWrite { get; set; }
        public IEnumerable<SoftwareLinkDTO> SoftwareLinkDTO { get; set; }
        public IEnumerable<UserRoleList> UserRoleLists { get; set; }
    }
    public class UserRoleList
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }
        public string OrgName { get; set; }

    }

           
}
