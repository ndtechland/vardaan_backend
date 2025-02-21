using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class RoutingDTO
    {
        public int RoutId { get; set; }
        public string VehicleNumber { get; set; }
        public string Location { get; set; }
        public int CabId { get; set; }
    }
}
