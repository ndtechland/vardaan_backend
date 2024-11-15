using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class CIData
    {
        public int Id { get; set; }
        public int CabBooking_Id  { get; set; }
        public string BookingId { get; set; }
        public DateTime JourneyDate { get; set; }
        public DateTime JourneyEndDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string BookingType { get; set; }
        public string BookedBy { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosedBy { get; set; }
        public string CabAlloted { get; set; }
        public string CabNo { get; set; }
        public int StartKm { get; set; }
        public int EndKm { get; set; }
        public double TotalKm { get; set; }
        public int ExtraKm { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double TotalHours { get; set; }
        public double ExtraHours { get; set; }
        public double Fare { get; set; }
        public int TotalNights { get; set; }
        public double NightCharges { get; set; }
        public double NightRate { get; set; }
        public double ParkingChage { get; set; }
        public double InterStateCharge { get; set; }
        public double TollCharge { get; set; }
        public double  ExtraHrsCharge { get; set; }
        public double ExtraKmsCharge { get; set; }
        public double ExtraHrsRate { get; set; }
        public double ExtraKmsRate { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public string Color { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public bool IsNrg { get; set; }
        public double VendorCommission { get; set; }
        public string Mobile { get; set; }
        public string NrgType { get; set; }
        public string PaymentType { get; set; }
        public string PackageTypeName { get; set; }
        public string GuestName { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public double MCD { get; set; }
        public string UpdateDescription { get; set; }
        public string CabBooked { get; set; }
        public int TotalRecords { get; set; }
        public double OutStationCharge { get; set; }
        public double OutStationChargeRate { get; set; }
        public double OutStationDays { get; set; }
        public double MiscCharge { get; set; }
        public int RouteNo { get; set; }
    }

    //updated by Bhupesh
    public class CIDataEdit
    {
        public int Id { get; set; }
        public int CabBooking_Id { get; set; }
        public string BookingId { get; set; }
        public DateTime JourneyDate { get; set; }
        public DateTime JourneyEndDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string BookingType { get; set; }
        public string BookedBy { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosedBy { get; set; }
        public string CabAlloted { get; set; }
        public string CabNo { get; set; }
        public int StartKm { get; set; }
        public int EndKm { get; set; }
        public double TotalKm { get; set; }
        public int ExtraKm { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double TotalHours { get; set; }
        public double ExtraHours { get; set; }
        public double Fare { get; set; }
        public int TotalNights { get; set; }
        public double NightCharges { get; set; }
        public double ParkingChage { get; set; }
        public double InterStateCharge { get; set; }
        public double TollCharge { get; set; }
        public double ExtraHrsCharge { get; set; }
        public double ExtraKmsCharge { get; set; }
        public double ExtraHrsRate { get; set; }
        public double ExtraKmsRate { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public string Color { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public bool IsNrg { get; set; }
        public double VendorCommission { get; set; }
        public string Mobile { get; set; }
        public string NrgType { get; set; }
        public string PaymentType { get; set; }
        public string PackageTypeName { get; set; }
        public string GuestName { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public double MCD { get; set; }
        public string UpdateDescription { get; set; }
        public string CabBooked { get; set; }
        public int TotalRecords { get; set; }
        public double OutStationCharge { get; set; }
        public double OutStationChargeRate { get; set; }
        public double OutStationDays { get; set; }
        public double MiscCharge { get; set; }
        public Boolean IsBilled { get; set; }
        public Nullable<int> CorporateInvoiceFile_Id { get; set; }
        public Boolean IsInvCreated { get; set; }
        public int RouteNo { get; set; }

    }


    public class VendorDsExcelModel
    {
        public string BookingId { get; set; }
        //   public string DsNo { get; set; }
        //   public DateTime DutyDate { get; set; }
        public string CabBooked { get; set; }
        public string CabNo { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public DateTime JourneyStartDate { get; set; }
        public DateTime JourneyEndDate { get; set; }
        public string GuestName { get; set; }
        public string PackageType { get; set; }
        public string Vendor { get; set; }
        //public string BookedBy { get; set; }
        // public string Mobile { get; set; }
        //  public string City { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double TotalHours { get; set; }
      //  public string BookingType { get; set; }
      //  public DateTime? ClosingDate { get; set; }
      //  public string ClosedBy { get; set; }
      
       // public string CabAlloted { get; set; }
        public double Fare { get; set; }
        public int StartKm { get; set; }
        public int EndKm { get; set; }
        public double TotalKm { get; set; }
        public int ExtraKm { get; set; }
        public double ExtraKmRate { get; set; }
        public double ExtraKmCharge { get; set; }       
        public double ExtraHours { get; set; }
        public double  ExtraHourRate { get; set; }
        public double  ExtraHourCharge { get; set; }       
        public int TotalNights { get; set; }
        public double NightCharges { get; set; }
        public double TACharge { get; set; }
        public double ParkingChage { get; set; }
        public double StateCharge { get; set; }
        public double TollCharge { get; set; }
        public double MCD { get; set; }     
        public double MiscellaneousCharge { get; set; }
        public double SubTotal { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }        
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        //public string DiscountType { get; set; }
        //public decimal DiscountValue { get; set; }
        
      //  public double VendorCommission { get; set; }
     //   public string NrgType { get; set; }
       
        
    }

    public class DsExcelModel
    {
        public int SlNo { get; set; }
        public string BookingId { get; set; }
        public string CabNo { get; set; }
        public string CabBooked { get; set; }
        // public DateTime JourneyStartDate { get; set; }
        public string JourneyStartDate { get; set; }
        //public DateTime JourneyEndDate { get; set; }
        public string JourneyEndDate { get; set; }
        public string GuestName { get; set; }
        public string Organization { get; set; }
       public string BookedBy { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string BookingType { get; set; }
        public string PackageType { get; set; }
        public DateTime? ClosingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double TotalHours { get; set; }
        public int StartKm { get; set; }
        public int EndKm { get; set; }
        public double TotalKm { get; set; }
        public int ExtraKm { get; set; }
        public double ExtraKmsRate { get; set; }
        public double ExtraHours { get; set; }
        public double ExtraHourRate { get; set; }
        public int TotalNights { get; set; }
        public double Fare { get; set; }
        public double ExtraKmsCharge { get; set; }
        public double ExtraHourCharge { get; set; }
        public double NightCharges { get; set; }
        public double TACharge { get; set; }
        public double ParkingChage { get; set; }
        public double StateTax { get; set; }
        public double TollTax { get; set; }
        public double MCD { get; set; }
        public double MiscellaneousCharge { get; set; }
        public double SubTotal { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        public int RouteNo { get; set; }
    }
}