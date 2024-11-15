using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Domain.ViewModels
{
    public class VendorPersonalPackageVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int Vendor_Id  { get; set; }
        public int MenuId { get; set; }
        public int SrNo { get; set; }
        public List<VendorPersonalPackageDTO> Packages { get; set; }
    }
}