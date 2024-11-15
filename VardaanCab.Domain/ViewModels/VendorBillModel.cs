using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class VendorBillModel
    {
        //public int Id { get; set; }
        //public int UpdatedBy { get; set; }
        //public System.DateTime UpdateDate { get; set; }
        //public int Booking_Id { get; set; }
        //public double CGST { get; set; }
        //public double SGST { get; set; }
        //public double IGST { get; set; }
        //public int Company_Id { get; set; }
        //public string GuestName { get; set; }
        //public string DutyAddress { get; set; }
        //public string DsNo { get; set; }
        //public System.DateTime DsDate { get; set; }
        //public int StartKms { get; set; }
        //public int EndKms { get; set; }
        //public double TotalHrs { get; set; }
        //public int ExtraKms { get; set; }
        //public double ExtraKmsRate { get; set; }
        //public double ExtraKmsCharge { get; set; }
        //public double ExtraHrs { get; set; }
        //public double ExtraHrsRate { get; set; }
        //public double ExtraHrsCharge { get; set; }
        //public double Total { get; set; }
        //public int StateGstin_Id { get; set; }
        //public double FuelCharge { get; set; }
        //public double Fare { get; set; }
        //public double NightCharge { get; set; }
        //public double MinKm { get; set; }
        //public double MinHrs { get; set; }
        //public double TotalKms { get; set; }
        //public double IncreaseDecreaseInFuel { get; set; }
        //public double FuelEfficiency { get; set; }
        //public int TotalNight { get; set; }
        //public System.TimeSpan StartHour { get; set; }
        //public System.TimeSpan EndHour { get; set; }
        //public double ParkCharge { get; set; }
        //public double TollCharge { get; set; }
        //public double StateCharge { get; set; }
        //public double NightRate { get; set; }
        //public bool IsCancelled { get; set; }
        //public Nullable<System.DateTime> CancellationDate { get; set; }
        //public string CancelledBy { get; set; }
        //public Nullable<System.DateTime> JourneyStartDate { get; set; }
        //public Nullable<System.DateTime> JourneyClosingDate { get; set; }
        //public double IgstPercent { get; set; }
        //public double CgstPercent { get; set; }
        //public double SgstPercent { get; set; }
        //public string BillFile { get; set; }
        //public double InterStateCharge { get; set; }
        //public double OutStationCharge { get; set; }
        //public double OutStationChargeRate { get; set; }
        //public double OutStationDays { get; set; }
        //public double VendorCommision { get; set; }
        //public double MCD { get; set; }
        //public string VendorName { get; set; }
        //public string BookingId { get; set; }


        //updated by bhupesh            
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string BookingId { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public string VehicleNumber { get; set; }
        public System.DateTime JourneyStartDate { get; set; }
        public System.DateTime JourneyClosingDate { get; set; }
        public string ClientName { get; set; }
        public string VisitedPlace { get; set; }
        public string PackageTypeName { get; set; }
        public string RentalTypeName { get; set; }
        public double CommisionableAmt { get; set; }
        public double VendorCommision { get; set; }
        public double payble { get; set; }
        public double Fare { get; set; }
        public double ExtraHrs { get; set; }
        public double ExtraHrsCharge { get; set; }
        public int ExtraKms { get; set; }
        public double ExtraKmsCharge { get; set; }
        public double BasicPayble { get; set; }
        public int TotalNight { get; set; }
        public double NightCharge { get; set; }
        public double OutStationDays { get; set; }
        public double OutStationCharge { get; set; }
        public double ParkCharge { get; set; }
        public double MCD { get; set; }
        public double TollCharge { get; set; }
        public double StateCharge { get; set; }

        public double MiscCharge { get; set; }
        public double SubTotal { get; set; }
        public double DiscountAmount { get; set; }
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