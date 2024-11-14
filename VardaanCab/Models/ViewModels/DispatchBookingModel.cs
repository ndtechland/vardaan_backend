using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class DispatchBookingModel
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        public int StartKms { get; set; }
        [Required]
        public TimeSpan StartHour { get; set; }
        [Required]
        public string BookingType { get; set; }
        public int MenuId { get; set; }
    }
}