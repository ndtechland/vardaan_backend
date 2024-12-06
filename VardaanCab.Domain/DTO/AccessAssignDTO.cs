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
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> UserRoleId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<SoftwareLinkDTO> SoftwareLinkDTO { get; set; }
    }
}
