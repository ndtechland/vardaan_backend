using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public SelectList Companies { get; set; }
    }
}
