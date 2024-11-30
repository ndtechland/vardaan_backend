using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class DepartmentMasterDTO
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MenuId { get; set; } 
        public SelectList Companies { get; set; }
        public IEnumerable<DepartmentList> DepartmentMasterList { get; set; }
    }
    public class DepartmentList
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public string CustomerName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
