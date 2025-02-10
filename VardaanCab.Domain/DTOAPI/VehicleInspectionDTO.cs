using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class VehicleInspectionDTO
    {
        public int Vendor_Id { get; set; }
        public DateTime InspectionDate { get; set; }
        public string VehicleNumber { get; set; }
    }
}
