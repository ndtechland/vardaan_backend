using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class RoutingDTO
    {
        public DateTime PickupAndDropDate { get; set; } 
        public int Company_Id { get; set; } 
        public int Vehicle_Type { get; set; } 
        public string Routing_Type { get; set; } 
        public int Routing_Options { get; set; } 
        public int WhereTripStartAndEnd { get; set; } 
        public int Trip_Type { get; set; } 
        public int Shift_Time { get; set; } 
        public string Adhoc_Shift_Time { get; set; } 
        public bool Generate_Routing { get; set; } 
        public bool Remember_Routing { get; set; } 
        public string Actual_vehicle { get; set; } 
        public string Escort_Id { get; set; } 
        public string IsVendorAllocation { get; set; } 
        public bool Assign_Cab_by_Route { get; set; } 
        public int Zone_Id { get; set; }
        public SelectList Customers { get; set; }
        public SelectList TripTypes { get; set; }
        public SelectList PickUpshiftTimes { get; set; }
        public SelectList DropshiftTimes { get; set; }
        public SelectList ShiftTypes { get; set; }
    }
}
