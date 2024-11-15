using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class CabBookingSubMenuModel
    {
        public int PendingBooking { get; set; }
        public int CompletedBooking { get; set; }
        public int DispatchedBooking { get; set; }
        public int CancelledBooking { get; set; }
        public int MonthlyBooking { get; set; }
        public int MonthlyRouteBooking { get; set; }
        public int AllBooking { get; set; }
    }
}