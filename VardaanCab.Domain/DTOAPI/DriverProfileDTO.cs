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
        public string DriverImage { get; set; } 
        public string MobileNumber { get; set; } 
        public string Address { get; set; }
        public string AlternateNo1 { get; set; }
        public string Email { get; set; }
        public string Pincode { get; set; }
        public string DriverImageBase64 { get; set; }
    }
}
