using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class EmployeeHomeRouteDTO
    {
        public int Id { get; set; }
        public Nullable<int> CompanyZoneId { get; set; }
        public Nullable<int> Company_Id { get; set; }
        public string CompanyZone { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MenuId { get; set; }
        public SelectList CompanyZones { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<HomeRouteList> HomeRouteLists { get; set; }
    }
    public class HomeRouteList
    {
        public int Id { get; set; }
        public Nullable<int> CompanyZoneId { get; set; }
        public string CompanyZone { get; set; }
        public string CompanyName { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
