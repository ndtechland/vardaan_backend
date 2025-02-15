using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class EmployeeDestinationAreaDTO
    {
        public int Id { get; set; }
        public Nullable<int> HomeRouteId { get; set; }
        public Nullable<int> Company_Id { get; set; }
        public Nullable<int> CompanyZoneId { get; set; }
        public string DestinationAreaName { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MenuId { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<DestinationAreaList> DestinationAreaLists { get; set; }
    }
    public class DestinationAreaList
    {
        public int Id { get; set; }
        public Nullable<int> HomeRouteId { get; set; }
        public string DestinationAreaName { get; set; }
        public string HomeRouteName { get; set; }
        public string CompanyZoneName { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
