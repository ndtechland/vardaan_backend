using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class CorporateBillingDTO
    {
        public int Id { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        [Required]
        public int Booking_Id { get; set; }
        [Required]
        public string PackageType { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        [Required]
        public int Company_Id { get; set; }
        [Required]
        public string GuestName { get; set; }
        public string DutyAddress { get; set; }
        public string DsNo { get; set; }
        public System.DateTime DsDate { get; set; }
        [Required]
        public int StartKms { get; set; }
        [Required]
        public int EndKms { get; set; }

        [Required]
        public Nullable<System.DateTime> JourneyStartDate { get; set; }
        [Required]
        public Nullable<System.DateTime> JourneyClosingDate { get; set; }

        [Required]
        public System.TimeSpan StartHour { get; set; }
        [Required]
        public System.TimeSpan EndHour { get; set; }
        [Required]
        public double TotalHrs { get; set; }
        [Required]
        public double MinKm { get; set; }
        [Required]
        public double MinHrs { get; set; }
        [Required]
        public double TotalKms { get; set; }

        [Required]
        public int ExtraKms { get; set; }
        [Required]
        public double ExtraKmsRate { get; set; }
        [Required]
        public double ExtraKmsCharge { get; set; }
        [Required]
        public double ExtraHrs { get; set; }
        [Required]
        public double ExtraHrsRate { get; set; }
        [Required]
        public double ExtraHrsCharge { get; set; }
        [Required]
        public double Fare { get; set; }
        [Required]
        public double NightCharge { get; set; }
        [Required]
        public double ParkCharge { get; set; }
        [Required]
        public double TollCharge { get; set; }
        [Required]
        public double StateCharge { get; set; }
        [Required]
        public int TotalNight { get; set; }

        public double Total { get; set; }
        [Required]
        public int StateGstin_Id { get; set; }
        public double FuelCharge { get; set; }
        public double IncreaseDecreaseInFuel { get; set; }
        public double FuelEfficiency { get; set; }
        [Required]
        public double NightRate { get; set; }
        public bool IsCancelled { get; set; }
        public Nullable<System.DateTime> CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public double IgstPercent { get; set; }
        public double CgstPercent { get; set; }
        public double SgstPercent { get; set; }
        public SelectList StateWiseGstinList { get; set; }
        public int Page { get; set; }
        public string Term { get; set; }
        public double Package8HourFare { get; set; }
        [Required]
        public double InterStateCharge { get; set; }
        [Required]
        public double OutStationCharge { get; set; }
        [Required]
        public double OutStationChargeRate { get; set; }
        [Required]
        public double OutStationDays { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public SelectList DiscountTypes { get; set; }

        public bool? IsNrg { get; set; }
        public double VendorCommision { get; set; }
        [Required]
        public string CommisionType { get; set; }
        public double MCD { get; set; }
        public int MenuId { get; set; }
        public string VisitedPlace { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public string DesiredCar { get; set; }
        public double MiscCharge { get; set; }

    }



    public class CorporateBillModel
    {
        public int Id { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int Booking_Id { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public int Company_Id { get; set; }
        public string GuestName { get; set; }
        public string DutyAddress { get; set; }
        public string DsNo { get; set; }
        public System.DateTime DsDate { get; set; }
        public int StartKms { get; set; }
        public int EndKms { get; set; }
        public double TotalHrs { get; set; }
        public int ExtraKms { get; set; }
        public double ExtraKmsRate { get; set; }
        public double ExtraKmsCharge { get; set; }
        public double ExtraHrs { get; set; }
        public double ExtraHrsRate { get; set; }
        public double ExtraHrsCharge { get; set; }
        public double Total { get; set; }
        public int StateGstin_Id { get; set; }
        public double FuelCharge { get; set; }
        public double Fare { get; set; }
        public double NightCharge { get; set; }
        public double MinKm { get; set; }
        public double MinHrs { get; set; }
        public double TotalKms { get; set; }
        public double IncreaseDecreaseInFuel { get; set; }
        public double FuelEfficiency { get; set; }
        public int TotalNight { get; set; }
        public System.TimeSpan StartHour { get; set; }
        public System.TimeSpan EndHour { get; set; }
        public double ParkCharge { get; set; }
        public double TollCharge { get; set; }
        public double StateCharge { get; set; }
        public double NightRate { get; set; }
        public bool IsCancelled { get; set; }
        public Nullable<System.DateTime> CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public Nullable<System.DateTime> JourneyStartDate { get; set; }
        public Nullable<System.DateTime> JourneyClosingDate { get; set; }
        public double IgstPercent { get; set; }
        public double CgstPercent { get; set; }
        public double SgstPercent { get; set; }
        public string BillFile { get; set; }
        public double InterStateCharge { get; set; }
        public double OutStationCharge { get; set; }
        public double OutStationChargeRate { get; set; }
        public double OutStationDays { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public double DiscountAmount { get; set; }

        public double NetAmount { get; set; }
        public bool IsNrg { get; set; }
        public double MCD { get; set; }
        public string PickupDropAddress { get; set; }
        public int VehicleModel_Id { get; set; }
        public string ModelName { get; set; }
        public string RentalTypeName { get; set; }
        public string PackageTypeName { get; set; }
        public string VisitedPlace { get; set; }
        public double MiscCharge { get; set; }

    }
    //updated by bhupesh
    public class CorporateBillReportModel
    {

//        b.BookingId,c.VehicleNumber,cb.TaxInvoiceNumber,cb.TaxInvoiceDate,cb.JourneyStartDate,cb.JourneyClosingDate,cb.GuestName,cb.VisitedPlace,pt.PType as PackageTypeName,rt.RentalTypeName,
//cb.Fare,cb.ExtraHrs,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsCharge,cb.TotalNight,cb.NightCharge,cb.OutStationDays,cb.OutStationCharge,cb.ParkCharge,cb.MCD,
//cb.TollCharge,cb.StateCharge,
//CgstPercent,cb.CGST,cb.SgstPercent,cb.SGST,cb.IgstPercent,cb.IGST,cb.Total

      //  public string DriverName { get; set; }
    public string BookingId { get; set; }
    public string VehicleNumber { get; set; }

        public string ModelName { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public System.DateTime JourneyStartDate { get; set; }
    public System.DateTime JourneyClosingDate { get; set; }
    public string CompanyName { get; set; }
        public string GuestName { get; set; }
        public string VisitedPlace { get; set; }
    public string PackageTypeName { get; set; }
    public string RentalTypeName { get; set; }
    public double Fare { get; set; }
    public double ExtraHrs { get; set; }
    public double ExtraHrsRate { get; set; }
    public double ExtraHrsCharge { get; set; }
    public int ExtraKms { get; set; }
    public double ExtraKmsRate { get; set; }
    public double ExtraKmsCharge { get; set; }
    public int TotalNight { get; set; }
        public double NightRate { get; set; }
        public double NightCharge { get; set; }
    public double OutStationDays { get; set; }
        public double OutStationChargeRate { get; set; }
        public double OutStationCharge { get; set; }
        public double DiscountAmount { get; set; }
        public double ParkCharge { get; set; }
    public double MCD { get; set; }
    public double TollCharge { get; set; }
    public double StateCharge { get; set; }
        public double MiscCharge { get; set; }
        public double SubTotal { get; set; }
        public double NetAmount { get; set; }
    public double CgstPercent { get; set; }
        public double CGST { get; set; }
        public double SgstPercent { get; set; }
        public double SGST { get; set; }
        public double IgstPercent { get; set; }
        public double IGST { get; set; }
        public double Total { get; set; }
}
}