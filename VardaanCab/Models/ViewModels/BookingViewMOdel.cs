using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class BookingViewMOdel
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public IEnumerable<BookingDTO> Bookings  { get; set; }
        public string BookingType { get; set; }
        public int BookingStatus { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }

        public int SrNo { get; set; }
        public int cityid { get; set; }
        public int routeno { get; set; }
    }

    public class BookingExcelModel
    {
        public string BookingId { get; set; }
        public string Organization { get; set; }
        public string Service_City { get; set; }
        public string PickupDate { get; set; }
        public string PickupTime { get; set; }
        public string DesiredCar { get; set; }
        public string RentalTypeName { get; set; }
        public string User_Name { get; set; }
        public string PickupAddress { get; set; }

        public string DropAddress { get; set; }
        public string Mobile { get; set; }
        public string BookerName { get; set; }
        public string BookedByPersonMobileNo { get; set; }
        
        public string Email { get; set; }        
       // public DateTime PickupDateTime { get; set; }
        
        public String Booked_On { get; set; }
      //  public System.DateTime Booked_On { get; set; }
       
      

        public string Booked_By { get; set; }
        

        public string Vehicle_Chauffeur{get;set;}
      

        public string UpdatedBy { get; set; }
        public string UpdateDate { get; set; }
        
        public string Status { get; set; }
        public int RouteNo { get; set; }
    }
}