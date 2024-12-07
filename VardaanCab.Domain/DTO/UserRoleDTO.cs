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
        public SelectList Companies { get; set; }
        public int[] IsReadChecked { get; set; }
        public int[] IsWriteChecked { get; set; }
        public int[] IsSubReadChecked { get; set; }
        public int[] IsSubWriteChecked { get; set; }
        public string ReadPermissions { get; set; }
        public string WritePermissions { get; set; }
        public IEnumerable<SoftwareLinkDTO> SoftwareLinkDTO { get; set; }
    }
}
