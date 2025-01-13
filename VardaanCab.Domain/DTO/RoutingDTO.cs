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
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public DateTime ShiftDate { get; set; } 
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
        public SelectList Zones { get; set; }
    }

    public class RouteAssignment
    {
        public string RouteName { get; set; }
        public string RouteId { get; set; }
        public string CabNumber { get; set; }
        public string CabType { get; set; }
        public List<EmployeeRouting> Employees { get; set; }
    }

    public class EmployeeRouting
    {
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Facility { get; set; }
        public string PickupDropLocation { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public TimeSpan PickupTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
