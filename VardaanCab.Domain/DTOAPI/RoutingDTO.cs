using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class RoutingDTO
    {
        public int Id { get; set; }
        public long? RouteId { get; set; }
        public string VehicleNumber { get; set; }
        public int? AvailableSeat { get; set; }
        public string VendorName { get; set; }
        public string ZoneName { get; set; }
    }
    public class RoutingDetailModel
    {
        public long? RouteId { get; set; }
        public DateTime Date { get; set; }
        public string VehicleNumber { get; set; }
        public string ZoneName { get; set; }
        public string CabId { get; set; }
        public string DriverName { get; set; }
        public string VehicleCapacity { get; set; }
        public int? AvailableSeat { get; set; }
        public string LastLocation { get; set; }
    }
    public class TrackCabEmployeePickupViewModel
    {
        public long RoutingID { get; set; }  
        public DateTime RouteDate { get; set; }  
        public string EmployeeName { get; set; }   
        public string CompanyName { get; set; }  
        public string PickupLocation { get; set; }   
        public string DropLocation { get; set; }  
        public string ShiftTime { get; set; }   
    }

}