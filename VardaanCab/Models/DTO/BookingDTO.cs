using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;

namespace VardaanCab.Models.DTO
{
    public class BookingDTO
    {
        public int BookingNo { get; set; }
        public int TotalRecords { get; set; }
        public int Id { get; set; }
        public System.DateTime BookingDate { get; set; }
       
        public System.DateTime PickupDate { get; set; }
        public Nullable<int> Cab_Id { get; set; }
        public string BookingId { get; set; }
        [Required]
        public string PickupAddress { get; set; }
        public string PickupLandMark { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public string BookedBy { get; set; }
        public string CompanyName { get; set; }
        public int Client_Id { get; set; }
        public string EmpCode { get; set; }
        public string CostCode { get; set; }
        public int BookingStatus { get; set; }
        public int RentalType { get; set; }
        public Nullable<int> DriverId { get; set; }
        [Required(ErrorMessage ="City Required")]
        public Nullable<int> City_Id { get; set; }
        [Required]
        public string DropAddress { get; set; }
        
        public string DropLandmark { get; set; }
        [Required]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [RegularExpression(@"^[-,0-9 ]+$", ErrorMessage = "Not a valid phone number")]
        public string ContactNo { get; set; }
        public System.TimeSpan PickupTime { get; set; }
        public System.TimeSpan ReportingTime { get; set; }
        public string Email { get; set; }
        public string CityName  { get; set; }
        public string RentalTypeName  { get; set; }
        public string BookingConfirmFile { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UserRole { get; set; }
        public string BookingStatusName  {
            get
            {
                var status = (Utilities.BookingStatus)BookingStatus;
                return status.ToString();
            }
        }
        public string DriverName  { get; set; }
        public string DriverContactNo   { get; set; }
        public string VehicleName {get; set;}
        public string VehicleNo  { get; set; }
        public SelectList OrganizationList { get; set; }
       
        public SelectList RentalList { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public DateTime PickupDateTime { get; set; }
        public string ClosedBy { get; set; }
        public Nullable<System.DateTime> ClosingDate { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        public SelectList VehicleModels { get; set; }
        public string CarModelName { get; set; }
        [Required]
        public string BookedByPerson { get; set; }
        public string BookedByPersonMobileNo { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Color {
            get {

                var currentDt = DateTime.Now;
                TimeSpan remain = PickupDateTime - currentDt;
                double hour = remain.TotalHours;
                string color= hour <= 2 ? "#F56F6F" : hour > 2 && hour <= 3 ? "#fee55b" : "#4df562";
                return color;
            }
        }
        [Required]
        public string BookingType { get; set; }
        public string NrdBookingMode { get; set; }
        [Required]
        public int ? NrdStateId { get; set; }
        public SelectList StateList { get; set; }
        public string DesiredCar { get; set; }
        public int StartKms { get; set; }
        public Nullable<System.TimeSpan> StartHour { get; set; }
        public string NrgType { get; set; }
        public string UpdateDescription { get; set; }
        [Required]
        public int PackageType_Id { get; set; }
        public SelectList PacakgeTypes { get; set; }
        public int MenuId { get; set; }
        public int ? OtherPackage_Id { get; set; }
        public string PackageTypeName { get; set; }
        public int DaysInMonth { get; set; }
        public bool IsReleasedCab { get; set; }
        public SelectList Cities { get; set; }
        public List<PickupDateTimeModel> PickupDateTimeList { get; set; }
        public string PreviousBookingType { get; set; }
        public string BookingInstruction { get; set; }
        public bool IsVIP { get; set; }
        public string Term { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public int Page { get; set; }
        public bool FromUnbilled { get; set; }
        public int bStatus { get; set; }
        public int FinYear { get; set; }
        public int RouteNo { get; set; }
    }

    public class PickupDateTimeModel
    {
        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan ReportingTime { get; set; }
        public string Type { get; set; }

    }


    public class ScheduleBookingDTO
    {
        public int TotalRecords { get; set; }
        public int Id { get; set; }
        public System.DateTime BookingDate { get; set; }
        
        public Nullable<int> Cab_Id { get; set; }
        public string BookingId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public string BookedBy { get; set; }
        public string CompanyName { get; set; }
        public int Client_Id { get; set; }
        public string EmpCode { get; set; }
        public string CostCode { get; set; }
        public int BookingStatus { get; set; }
        public Nullable<int> DriverId { get; set; }
        [Required(ErrorMessage = "City Required")]
        public Nullable<int> City_Id { get; set; }
        public List<BookingRecordModel> BookingRecords { get; set;}
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactNo { get; set; }
      
        public string Email { get; set; }
        public string CityName { get; set; }
        public string RentalTypeName { get; set; }
        public string BookingConfirmFile { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UserRole { get; set; }
        public string BookingStatusName
        {
            get
            {
                var status = (Utilities.BookingStatus)BookingStatus;
                return status.ToString();
            }
        }
        public string DriverName { get; set; }
        public string DriverContactNo { get; set; }
        public string VehicleName { get; set; }
        public string VehicleNo { get; set; }
        public SelectList OrganizationList { get; set; }

        public TimeSpan TimeLeft { get; set; }
        public DateTime PickupDateTime { get; set; }
        public string ClosedBy { get; set; }
        public Nullable<System.DateTime> ClosingDate { get; set; }
       
        public string CarModelName { get; set; }
        [Required]
        public string BookedByPerson { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Color
        {
            get
            {

                var currentDt = DateTime.Now;
                TimeSpan remain = PickupDateTime - currentDt;
                double hour = remain.TotalHours;
                string color = hour <= 2 ? "#F56F6F" : hour > 2 && hour <= 3 ? "#fee55b" : "#4df562";
                return color;
            }
        }
        [Required]
        public string BookingType { get; set; }
        public string NrdBookingMode { get; set; }
        public int? NrdStateId { get; set; }
        public SelectList StateList { get; set; }
        public string DesiredCar { get; set; }
        public int StartKms { get; set; }
        public Nullable<System.TimeSpan> StartHour { get; set; }
        public string NrgType { get; set; }
        public string UpdateDescription { get; set; }
        public int MenuId { get; set; }
        public int? OtherPackage_Id { get; set; }
        public string PackageTypeName { get; set; }
        public int DaysInMonth { get; set; }
        public bool IsReleasedCab { get; set; }
        public SelectList Cities { get; set; }
        public string PreviousBookingType { get; set; }
        public string BookingInstruction { get; set; }
        public bool IsVIP { get; set; }
        public string Term { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public int Page { get; set; }
        public bool FromUnbilled { get; set; }
        public int bStatus { get; set; }
        public int FinYear { get; set; }
        public int RouteNo { get; set; }
    }

    public class BookingRecordModel
    {
        [Required]
        public System.DateTime PickupDate { get; set; }
        [Required]
        public System.TimeSpan PickupTime { get; set; }
        [Required]
        public System.TimeSpan ReportingTime { get; set; }
        [Required]
        public string PickupAddress { get; set; }
        public string PickupLandMark { get; set; }
        [Required]
        public string DropAddress { get; set; }
        public string DropLandmark { get; set; }
        [Required]
        public int PackageType_Id { get; set; }
        [Required]
        public int RentalType { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        public SelectList PacakgeTypes { get; set; }
        public SelectList RentalList { get; set; }
        public SelectList VehicleModels { get; set; }
    }

}