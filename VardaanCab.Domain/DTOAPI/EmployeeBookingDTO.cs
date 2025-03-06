using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class EmployeeBookingDTO
    {
        public int RoutingID { get; set; } 
        public string CompanyName { get; set; } 
        public int CabId { get; set; } 
        public string CabName { get; set; } 
        public string TripTypeName { get; set; } 
        public string VehicleNumber { get; set; } 
        public DateTime Date { get; set; } 
        public string PickupShiftTime { get; set; }
        public string Employee_Id { get; set; }
        public string DropShiftTime { get; set; }
        public string PickupLocation { get; set; } 
        public string DropLocation { get; set; } 
    }
    public class LiveCabs
    {
        public int RoutingID { get; set; }
        public string CompanyName { get; set; }
        public int CabId { get; set; }
        public string CabName { get; set; }
        public string TripTypeName { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime Date { get; set; }
        public string PickupShiftTime { get; set; }
        public int DriverId { get; set; }
        public string DropShiftTime { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
    }
    public class FinishCabs
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CompanyName { get; set; }
    }
}
