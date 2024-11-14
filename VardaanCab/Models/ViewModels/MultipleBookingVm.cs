using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class MultipleBookingVm
    {
        public int MenuId { get; set; }
        public bool IsPdf { get; set; }
        public int MultipleBookingId { get; set; }
        public string MultipleBookingFile { get; set; }
        public List<BookingDTO> Booking { get; set; }
    }
}