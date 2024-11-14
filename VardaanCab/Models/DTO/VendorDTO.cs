using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class VendorDTO
    {
        public int Id { get; set; }
        public int UserLogin_Id { get; set; }
        [Required]
        public string VendorName { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public string FullAddress { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string AlernateMobile { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ParentVendor_Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }

        public string CityName { get; set; }
        public string StateName { get; set; }
        public string ParentVendorName { get; set; }

        public SelectList  Cities { get; set; }
        public SelectList States { get; set; }
        public SelectList Vendors { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(15, ErrorMessage = "GSTIN must have 15 characters long only")]
        public string GSTIN { get; set; }
        public string PAN { get; set; }
        public string CIN { get; set; }
        public string PanImage { get; set; }
        public HttpPostedFileBase PanFile { get; set; }
        public List<VendorPersonalPackageDTO> Packages { get; set; }

        public int MenuId { get; set; }

    }
}