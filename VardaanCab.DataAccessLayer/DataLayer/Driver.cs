//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VardaanCab.DataAccessLayer.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Driver
    {
        public int Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string DriverName { get; set; }
        public string DlNumber { get; set; }
        public string DlImage { get; set; }
        public string AadharNo { get; set; }
        public string AadharImage { get; set; }
        public string DriverImage { get; set; }
        public bool IsActive { get; set; }
        public string MobileNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string DriverAddress { get; set; }
        public string PoliceVerificationNo { get; set; }
        public System.DateTime LicenceExpDate { get; set; }
        public string PolVerifImg { get; set; }
        public string FatherName { get; set; }
        public string AlternateNo1 { get; set; }
        public string AlternateNo2 { get; set; }
        public string LocalAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string PanImage { get; set; }
        public string PanNumber { get; set; }
        public bool IsOutsider { get; set; }
        public Nullable<bool> IsFirst { get; set; }
        public string Password { get; set; }
        public Nullable<int> OTP { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public Nullable<int> DeviceId { get; set; }
        public Nullable<int> Vendor_Id { get; set; }
        public Nullable<bool> IsLogin { get; set; }
        public Nullable<double> CurrentLat { get; set; }
        public Nullable<double> CurrentLong { get; set; }
    }
}
