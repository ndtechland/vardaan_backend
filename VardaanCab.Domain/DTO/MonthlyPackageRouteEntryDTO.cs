using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class MonthlyPackageRouteEntryDTO
    {
        public int Id { get; set; }
        public int MonthlyPackageRoute_Id { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string CabNo { get; set; }
        public Nullable<System.TimeSpan> PickupTime { get; set; }
        public string PickupPlace { get; set; }
        public Nullable<System.TimeSpan> DropTime { get; set; }
        public string DropPlace { get; set; }
        public int Driver_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdatedBy_Id { get; set; }
        public string UpdatedBy { get; set; }
        public int Cab_Id { get; set; }
        public System.DateTime JourneyStartDate { get; set; }
        public DateTime ? JourneyClosingDate { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        public string DriverName { get; set; }
        public string VehicleModel { get; set; }
        public double Toll { get; set; }
        public double MCD { get; set; }
    }
}