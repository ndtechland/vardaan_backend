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
        public string DestinationAreaName { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MenuId { get; set; }
        public SelectList HomeRoutes { get; set; }
        public IEnumerable<DestinationAreaList> DestinationAreaLists { get; set; }
    }
    public class DestinationAreaList
    {
        public int Id { get; set; }
        public Nullable<int> HomeRouteId { get; set; }
        public string DestinationAreaName { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
