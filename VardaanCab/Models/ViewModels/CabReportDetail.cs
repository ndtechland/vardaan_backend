using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class CabReportDetail
    {
        //public int Id { get; set; }
        //public System.DateTime BookingDate { get; set; }
        //public System.DateTime PickupDate { get; set; }
        //public System.TimeSpan ReportingTime { get; set; }
        //public System.TimeSpan PickupTime { get; set; }
        //public string BookingId { get; set; }
        //public string PickupAddress { get; set; }
        //public string PickupLandMark { get; set; }
        //public string DropAddress { get; set; }
        //public string CustomerName { get; set; }
        //public string BookedBy { get; set; }
        //public string CompanyName { get; set; }
        //public string RentalTypeName { get; set; }
        //public string VehicleNumber { get; set; }
        //public string  PackageTypeName { get; set; }
        //public double TotalKms { get; set; }
        //public double TotalHrs { get; set; }
        //public int StartKms { get; set; }
        //public int EndKms { get; set; }
        //public TimeSpan StartHour { get; set; }
        //public TimeSpan EndHour { get; set; }
        //public double TollCharge { get; set; }
        //public int TotalNight { get; set; }
        //public double OutStationCharge { get; set; }
        //public double Fare { get; set; }
        //public double Total { get; set; }
        //public double NightCharge { get; set; }

        //updated by bhupesh

//        b.BookingId,d.DriverName,c.VehicleNumber,cb.JourneyStartDate,cb.JourneyClosingDate,b.CompanyName,cb.VisitedPlace,pt.PType as PackageTypeName,
//            rt.RentalTypeName,
//cb.Fare,cb.StartKms,cb.EndKms,cb.TotalKms,cb.StartHour,cb.EndHour,cb.TotalHrs,cb.ExtraHrs,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsCharge,cb.TotalNight,cb.NightCharge,cb.OutStationDays,cb.OutStationCharge,cb.ParkCharge,cb.MCD,
//cb.TollCharge,cb.StateCharge,cb.NetAmount

        public string BookingId { get; set; }
        public string DriverName { get; set; }
        
        public string VehicleNumber { get; set; }
        public string DesiredCar { get; set; }
        public string GuestName { get; set; }       
        public System.DateTime JourneyStartDate { get; set; }
        public System.DateTime JourneyClosingDate { get; set; }
        public string CompanyName { get; set; }
        public string VisitedPlace { get; set; }
        public string PackageTypeName { get; set; }
        public string RentalTypeName { get; set; }
        public double Fare { get; set; }

        public int StartKms { get; set; }
        public int EndKms { get; set; }
        public double TotalKms { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public double TotalHrs { get; set; }
        public double ExtraHrs { get; set; }
        public double ExtraHrsCharge { get; set; }
        public int ExtraKms { get; set; }
        public double ExtraKmsCharge { get; set; }
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
    }

   

    public class CabReportDetailModel
    {
        public IEnumerable<CabReportDetail> Data { get; set; }
        public string  CabNo { get; set; }
        public string SDate { get; set; }
        public string eDate { get; set; }
        public string term { get; set; }
        public int menuId { get; set; }
    }


    //public class DriverReportDetail
    //{
    //    public int Id { get; set; }
    //    public System.DateTime BookingDate { get; set; }
    //    public System.DateTime PickupDate { get; set; }
    //    public System.TimeSpan ReportingTime { get; set; }
    //    public System.TimeSpan PickupTime { get; set; }
    //    public string BookingId { get; set; }
    //    public string PickupAddress { get; set; }
    //    public string PickupLandMark { get; set; }
    //    public string DropAddress { get; set; }
    //    public string CustomerName { get; set; }
    //    public string BookedBy { get; set; }
    //    public string CompanyName { get; set; }
    //    public string RentalTypeName { get; set; }
    //    public string DriverName { get; set; }
    //    public string MobileNumber { get; set; }
    //    public string PackageTypeName { get; set; }
    //    public double TotalKms { get; set; }
    //    public double TotalHrs { get; set; }
    //    public string VehicleNumber { get; set; }
    //    public int StartKms { get; set; }
    //    public int EndKms { get; set; }
    //    public double Total { get; set; }
    //    public double Fare { get; set; }
    //    public double OutStationCharge { get; set; }
    //    public double NightCharge { get; set; }
    //    public int TotalNight  { get; set; }
    //    public double TollCharge { get; set; }
    //}

    //Updated by Bhupesh Bisht
    //Date 25/11/2020
    public class DriverReportDetail
    {
       
            
        public string DriverName { get; set; }
        public string BookingId { get; set; }
        public string VehicleNumber { get; set; }
        public System.DateTime JourneyStartDate { get; set; }
        public System.DateTime JourneyClosingDate { get; set; }
        public string CompanyName { get; set; }
        public string VisitedPlace { get; set; }
        public string PackageTypeName { get; set; }
        public string RentalTypeName { get; set; }
        public double Fare { get; set; }
        public double ExtraHrs { get; set; }
        public double ExtraHrsCharge { get; set; }
        public int ExtraKms { get; set; }
        public double ExtraKmsCharge { get; set; }
        public int TotalNight { get; set; }
        public double NightCharge { get; set; }
        public double OutStationDays { get; set; }
        public double OutStationCharge { get; set; }
        public double  ParkCharge { get; set; }
        public double MCD { get; set; }
        public double TollCharge { get; set; }
        public double StateCharge { get; set; }

        public double MiscCharge { get; set; }
        public double SubTotal { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }

//public int Id { get; set; }
//        public System.DateTime BookingDate { get; set; }
//        public System.DateTime PickupDate { get; set; }
//        public System.TimeSpan ReportingTime { get; set; }
//        public System.TimeSpan PickupTime { get; set; }
       
//        public string PickupAddress { get; set; }
//        public string PickupLandMark { get; set; }
//        public string DropAddress { get; set; }
//        public string CustomerName { get; set; }
//        public string BookedBy { get; set; }
        
        
      
//        public string MobileNumber { get; set; }
        
//        public double TotalKms { get; set; }
//        public double TotalHrs { get; set; }
      
//        public int StartKms { get; set; }
//        public int EndKms { get; set; }
//        public double Total { get; set; }      
        
        
        
        
    }
    public class DriverReportDetailModel
    {
        public IEnumerable<DriverReportDetail> Data { get; set; }
        public string DriverName { get; set; }
        public string SDate { get; set; }
        public string eDate { get; set; }
        public string term { get; set; }
        public int menuId { get; set; }
    }
}