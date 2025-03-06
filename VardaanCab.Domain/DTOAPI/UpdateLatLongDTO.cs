using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class UpdateLatLongDTO
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public double Lat{ get; set; }
        public double Long { get; set; }
    }
}
