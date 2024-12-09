using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class DriverDTO
    {
        public int Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        [Required]
        public string DriverName { get; set; }
        public string Email { get; set; }
        public Nullable<int> Vendor_Id { get; set; }
        public int UserLogin_Id { get; set; }
        public string DlNumber { get; set; }
        public string DlImage { get; set; }
        [MinLength(12, ErrorMessage = "AadharNo must have 12 characters long only"), MaxLength(12, ErrorMessage = "AadharNo must have 12 characters long only")]
        public string AadharNo { get; set; }
        public string AadharImage { get; set; }
        public string DriverImage { get; set; }
        
        //[Required]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        //[Required]
        public HttpPostedFileBase DlImageFile { get; set; }
        //[Required]
        public HttpPostedFileBase AadharImageFile { get; set; }
        public HttpPostedFileBase DriverImageFile { get; set; }
        public HttpPostedFileBase PolVerifImgFile { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive  { get; set; }
        public string DriverAddress { get; set; }
        public string PoliceVerificationNo { get; set; }
        [Required]
        public System.DateTime LicenceExpDate { get; set; }
        public string PolVerifImg { get; set; }
        public string FatherName { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string AlternateNo1 { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string AlternateNo2 { get; set; }
        [MaxLength(150)]
        public string LocalAddress { get; set; }
        [MaxLength(150)]
        public string PermanentAddress { get; set; }
        public string PanImage { get; set; }
        public HttpPostedFileBase PanFile { get; set; }
        public string PanNumber { get; set; }
        public int MenuId { get; set; }
        public Nullable<bool> IsOnline { get; set; }
    }
}