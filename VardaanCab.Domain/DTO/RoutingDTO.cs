using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
        public int[] Vehicle_Type { get; set; }
        public string Routing_Type { get; set; }
        public string Routing_Options { get; set; }
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
        public List<int> PickupShiftid { get; set; }
        public List<int> DropShiftid { get; set; }
        public SelectList Customers { get; set; }
        public SelectList RouteStatuses { get; set; }
        public SelectList Triptypes { get; set; }
        public SelectList TripTypes { get; set; }
        public SelectList PickUpshiftTimes { get; set; }
        public SelectList DropshiftTimes { get; set; }
        public SelectList ShiftTypes { get; set; }
        public SelectList Zones { get; set; }
        public IEnumerable<RoutingCabAllocation> RoutingCabAllocations { get; set; }
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
    public class RoutingCabAllocation
    {
        public long? RouteId { get; set; }
        public DateTime Date { get; set; }
        public string VehicleNumber { get; set; }
        public string ZoneName { get; set; }
        public int TotalRoutes { get; set; }
        public string CabId { get; set; }
        public string DriverName { get; set; }
        public string VehicleCapacity { get; set; }
        public int? AvailableSeat { get; set; }
        public string LastLocation { get; set; }
    }

    public class RoutingCabAllCounts
    {
        public int totalzone { get; set; }
        public int totalroute { get; set; }
        public int totalcloseroute { get; set; }
        public int totalrouteNotStarted { get; set; }
        public int totalStartedRoutes { get; set; }
        public int totalOpenRoutes { get; set; }
        public int totalBackToBackRoutes { get; set; }
        public int totalEmployees { get; set; }
        public int totalMaleEmployees { get; set; }
        public int totalFemaleEmployees { get; set; }
        public int TotalNoVendors { get; set; }
        public int EscortRequired { get; set; }
        public int TotalEmployeeOnboard { get; set; }
        public int TotalEmployeeYetToOnboard { get; set; }
        public decimal VehicleOccupancy { get; set; }
        public List<Dictionary<string, int>> totalVehicleType { get; set; }
        public List<RoutingCabAllocation> routingcaballocation { get; set; } = new List<RoutingCabAllocation>();
        public List<Vehicletype> Vehicletypes { get; set; } = new List<Vehicletype>();
    }
    public class Vehicletype
    {
        public string Capacity { get; set; }
        public int EmpInCab { get; set; }
    }
}
