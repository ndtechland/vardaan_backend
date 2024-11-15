using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Domain.ViewModels
{
    public class VendorListVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<VendorDTO> Vendors { get; set; }
    }

    public class VendorExcelModel
    {
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string AlernateMobile { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ParentVendorName { get; set; }
        public string FullAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string GSTIN { get; set; }
        public string CIN { get; set; }
        public string PAN { get; set; }
       
    }
}