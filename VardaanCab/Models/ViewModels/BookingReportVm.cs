using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class BookingReportVm
    {
        public IEnumerable<BookingStatusCount> BookingStatusData { get; set; }
        public int TotalBooking { get; set; }
        // Add by Anupma
        public int Count { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public string Term { get; set; }
    }
}