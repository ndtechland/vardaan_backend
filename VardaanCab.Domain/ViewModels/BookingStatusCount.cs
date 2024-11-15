using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class BookingStatusCount
    {
        public string StatusName { get; set; }
        public int BookingStatusId { get; set; }
        public int Count { get; set; }
        public DateTime BookingDate { get; set; }
    }
}