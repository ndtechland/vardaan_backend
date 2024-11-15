using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class SubMenuController : Controller
    {
        // GET: SubMenu
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();

        //public ActionResult MasterEntrySubMenu()
        //{
        //    return View();
        //}

        //public ActionResult RegistrationSubMenu ()
        //{
        //    return View();
        //}

        //public ActionResult CabBookingSubMenu ()
        //{
        //    var currentDate = DateTime.Now.Date;
        //    var bookingStatus = cr.GetBookingStatus().ToList();
        //    var booking = ent.Bookings.Where(a=> DbFunctions.TruncateTime(a.BookingDate)== DbFunctions.TruncateTime(currentDate)).ToList();
        //    var bookingData = (from b in booking
        //                       join bs in bookingStatus
        //                        on b.BookingStatus equals bs.Id
        //                       group new { bs.StatusName, bs.Id }
        //                       by new { bs.Id }
        //                        into g
        //                       select new BookingStatusCount
        //                       {
        //                           StatusName = g.FirstOrDefault().StatusName,
        //                           Count = g.Count(),
        //                           BookingStatusId = g.FirstOrDefault().Id
        //                       }
        //                       ).ToList();
        //    var pendingBooking = bookingData.Where(a => a.BookingStatusId == (int)BookingStatus.Pending).Select(a => a.Count).FirstOrDefault();
        //    var completedBooking = bookingData.Where(a => a.BookingStatusId == (int)BookingStatus.Completed).Select(a => a.Count).FirstOrDefault();
        //    var dispatchedBooking = bookingData.Where(a => a.BookingStatusId == (int)BookingStatus.Dispatch).Select(a => a.Count).FirstOrDefault();
        //    var cancelledBooking = bookingData.Where(a => a.BookingStatusId == (int)BookingStatus.Cancelled).Select(a => a.Count).FirstOrDefault();
        //    var monthlyBooking = ent.MonthlyPackages.Where(a => !a.IsClosed && DbFunctions.TruncateTime(a.CreateDate)==currentDate).Count();
        //    var monthlyBookingRoute = ent.MonthlyPackageRoutes.Where(a => !a.IsClosed && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
        //    var model = new CabBookingSubMenuModel {
        //        MonthlyBooking=monthlyBooking,
        //        PendingBooking=pendingBooking,
        //        CancelledBooking=cancelledBooking,
        //        CompletedBooking=completedBooking,
        //        DispatchedBooking=dispatchedBooking,
        //        MonthlyRouteBooking=monthlyBookingRoute
        //    };
        //    model.AllBooking = monthlyBooking + pendingBooking + cancelledBooking + completedBooking + dispatchedBooking + monthlyBookingRoute;
        //    return View(model);
        //}

        //public ActionResult CorporatePackageSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult VendorPackageSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult LeaveSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult NRDPackageSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult ReportsSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult PackageSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult BillingSubMenu()
        //{
        //    return View();
        //}

        //public ActionResult UpdateSubMenu()
        //{
        //    return View();
        //}
    }
}