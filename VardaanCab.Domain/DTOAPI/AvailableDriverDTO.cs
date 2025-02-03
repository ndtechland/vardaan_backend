using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class AvailableDriverDTO
    {
        public int CheckInId { get; set; }
        public string DriverName { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public int DriverId { get; set; }
        public int? DeviceId { get; set; }
        public string MobileNumber { get; set; }
    }
    public class VendorList
    {
        public int Vendor_Id { get; set; }
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
    }
    public class Drivers
    {
        public int Driver_Id { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
    }
    public class Vehicles
    {
        public int Vehicle_Id { get; set; }
        public string VehicleNumber { get; set; }
        public string ModelName { get; set; }
    }
}
