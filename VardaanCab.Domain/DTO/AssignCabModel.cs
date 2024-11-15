using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class AssignCabModel
    {
        public int Booking_Id { get; set; }
        [Required]
        public int Cab_Id { get; set; }
        [Required]
        public int ? Driver_Id { get; set; }
        public int? PDriver_Id  { get; set; }
        public int? PCab_Id  { get; set; }
        public string ModelName  { get; set; }
        public string VehicleNo { get; set; }
        public string BookingType { get; set; }
        public int MenuId { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        [Required]
        public int StartKms { get; set; }
        [Required]
        public TimeSpan? StartHour { get; set; }
        public SelectList Vendors { get; set; }
        public SelectList VehicleModels { get; set; }
        public string cabName { get; set; }
        public string DesiredCar { get; set; }

        public TimeSpan PickupTime { get; set; }

        public string Term { get; set; }
        public int Page { get; set; }
        public DateTime?sDate { get; set; }
        public DateTime?eDate { get; set; }

        public string BookingStatus { get; set; }
    }
}