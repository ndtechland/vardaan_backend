using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class TaxInvoiceViewModel
    {
        public int Id { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int Booking_Id { get; set; }
        public DateTime? JourneyDate { get; set; }
        public DateTime? JourneyEndDate { get; set; }
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
        public string PlaceOfService  { get; set; }
        public string RentalTypeName { get; set; }
        public string VehicleName { get; set; }
        public string VehicleNo { get; set; }

        public string FromCompanyName { get; set; }
        public string FromAddress { get; set; }
        public string FromStateName  { get; set; }
        public string FromGSTIN  { get; set; }
        public string FromEmail { get; set; }

        public string FromBankName { get; set; }
        public string FromAC_No { get; set; }
        public string FromBranchAddress { get; set; }
        public string FromIFS_Code { get; set; }
        public string BilledAddress { get; set; }
        
        public string CompanyName  { get; set; }
        public string CompanyGSTIN  { get; set; }
        public string CompanyState  { get; set; }

        public string CompanyRegAdd  { get; set; }
        public string CompanyRegState  { get; set; }
        public string BookingId  { get; set; }
        public DateTime PickupDate  { get; set; }
        public double ParkTollStateCharge { get; set; }
        
        public double IgstPercent { get; set; }
        public double CgstPercent { get; set; }
        public double SgstPercent { get; set; }
        public bool IsTaxInvoice { get; set; }
        public string BillFile { get; set; }
        public double InterStateCharge { get; set; }
        public double OutStationCharge { get; set; }
        public double OutStationChargeRate { get; set; }
        public double OutStationDays { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public string PAN { get; set; }
        public string CIN { get; set; }
        public double MCD { get; set; }
        public string PackageTypeName { get; set; }

        //
        public int NoOfDays { get; set; }
        public bool isPdf { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public DateTime ? TaxInvoiceDate { get; set; }

        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        public string VisitedPlace { get; set; }
        public bool ShouldRoundOff { get; set; }
        public double MiscCharge { get; set; }

        public long ? ProformNo { get; set; }

        public DateTime ? sDate { get; set; }
        public DateTime ? eDate { get; set; }
        public string Term { get; set; }
        public string IsNrg { get; set; }
        public string Display { get; set; }
        public int Page { get; set; }
        public bool Export { get; set; }
    }
}