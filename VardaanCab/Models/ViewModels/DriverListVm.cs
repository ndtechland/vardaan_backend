using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class DriverListVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public IEnumerable<Driver> Drivers { get; set; }
        public int SrNo { get; set; }
    }

    public class DriverExcelModel
    {
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public string AlternateNo1 { get; set; }
        public string AlternateNo2 { get; set; }
        public string PoliceVerificationNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string LocalAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string AadharNo { get; set; }
        public string LicenceNumber { get; set; }     
        public DateTime LicenceExpDate { get; set; }    
        public string PanNumber { get; set; } 
    }
}