using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class CabReadingDTO
    {
        public int Id { get; set; }
        public Nullable<int> RouteId { get; set; }
        public Nullable<int> DriverId { get; set; }
        public string MeterReading { get; set; }
        public string MeterReadingImage { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string MeterReadingImageBase64 { get; set; }
    }
}
