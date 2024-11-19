using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class DriverProfileDTO
    {
        public int Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string DriverName { get; set; }
        public string DlNumber { get; set; }
        public string DlImage { get; set; }
        public string AadharNo { get; set; }
        public string AadharImage { get; set; }
        public string DriverImage { get; set; } 
        public string MobileNumber { get; set; } 
        public string DriverAddress { get; set; } 
        public string PanImage { get; set; }
        public string PanNumber { get; set; } 
    }
}
