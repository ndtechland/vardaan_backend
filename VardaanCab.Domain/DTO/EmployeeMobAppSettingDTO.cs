using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace VardaanCab.Domain.DTO
{
    public class EmployeeMobAppSettingDTO
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> CabRequestDays { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<CabReqDays> CabReqDayList { get; set; }
    }
    public class CabReqDays
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> CabRequestDays { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
