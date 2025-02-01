using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTO
{
    public class AvailableDriverDTO
    {
        public int CheckinId { get; set; }
        public string DriverName { get; set; }
        public int DeviceId { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string VehicleType { get; set; }
        public string VehicleMake { get; set; }
        public string Status { get; set; }
        public string Capacity { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public int DriverId { get; set; }
        public string MobileNumber { get; set; }
        public IEnumerable<AvailableDriverDTO> AvailableDrivers { get; set; }
    }
    public class VendorList
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
    }
}
