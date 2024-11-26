using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            var rm = new LoginModel();
            rm.ReturnUrl = ReturnUrl;
            return View(rm);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                var data = ent.UserLogins.FirstOrDefault(a => (a.Email == model.Username || a.MobileNumber == model.Username) && a.Password == model.Password && a.IsActive == true);

                if (data == null)
                {
                    TempData["msg"] = "Invalid username or password";
                    return View(model);
                }

                FormsAuthentication.SetAuthCookie(data.Id.ToString(), true);
                string hostName = Dns.GetHostName();
                string ip = Dns.GetHostByName(hostName).AddressList[0].ToString();
                Session["uEmail"] = data.Email;
                var lh = new LoginHistory
                {
                    UserLogin_Id = data.Id,
                    Ip_Address = ip,
                    UpdateDate = DateTime.Now
                };
                ent.LoginHistories.Add(lh);
                ent.SaveChanges();

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Dashboard");


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error" + "StackTrace-" + ex.Message + ex.StackTrace + "innerExpes-" + ex.InnerException;
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ShowMenus()
        {
            int userId = int.Parse(User.Identity.Name);
            var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
            var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
            foreach (var item in softwareLinks)
            {
                var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + " and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                item.ChildMenus = l;
            }
            return PartialView(softwareLinks);
        }

        public ActionResult Dashboard()
        {
            //try
            //{
            //    DeactivateInvalidCabs();
            //}
            //catch (Exception ex)
            //{

            //}
            int userId = int.Parse(User.Identity.Name);
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                var softwareLinks = ent.Database.SqlQuery<SoftwareLink>(query).ToList();
                var model = new DashboardModel();
                model.Data = softwareLinks;
                return View(model); 
        }

        public ActionResult Submenu(int menuId)
        {
            int userId = int.Parse(User.Identity.Name);
            var query = @"select * from SoftwareLink where  Parent_Id=" + menuId + " and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
            var data = ent.Database.SqlQuery<SoftwareLink>(query).ToList();
            ViewBag.menuId = menuId;
            string finStr =new CommonRepository().getCurrentFinYear();
            int finyear = Convert.ToInt32(finStr);
            if (menuId == 3 || menuId == 7)  // menuId=3 = CabBooking 
            {
                // var currentDate = DateTime.Now.Date;
                //  var bookingStatus = cr.GetBookingStatus().ToList();
                //var booking = ent.Bookings.Where(a => DbFunctions.TruncateTime(a.BookingDate) == DbFunctions.TruncateTime(currentDate)).ToList();
                //2023 update
                //var booking = ent.Bookings.ToList().Where(x=>x.PickupDate<= cr.GetISTDate().Date).ToList();
                var booking = ent.getbookingHeadCount(finyear).Where(x => x.PickupDate <= cr.GetISTDate().Date).ToList();

                //var bookingData = (from b in booking
                //                   join bs in bookingStatus
                //                    on b.BookingStatus equals bs.Id
                //                   group new { bs.StatusName, bs.Id }
                //                   by new { bs.Id }
                //                    into g
                //                   select new BookingStatusCount
                //                   {
                //                       StatusName = g.FirstOrDefault().StatusName,
                //                       Count = g.Count(),
                //                       BookingStatusId = g.FirstOrDefault().Id
                //                   }
                //                   ).ToList();
                //var monthlyBooking = ent.MonthlyPackages.Where(a => !a.IsClosed && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
                //var monthlyBookingRoute = ent.MonthlyPackageRoutes.Where(a => !a.IsClosed && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
                //var dispatchedBooking = bookingData.Where(a => a.BookingStatusId == (int)BookingStatus.Dispatch).Select(a => a.Count).FirstOrDefault();

                var pendingBooking = booking.Where(a => a.BookingStatus == (int)BookingStatus.Pending && a.PickupDate.Date <= cr.GetISTDate().Date).ToList().Where(x => x.PackageType_Id ==1 || x.PackageType_Id ==2 || x.PackageType_Id == 3).ToList().Count;
                var pendingBookinETSg = booking.Where(a => a.BookingStatus == (int)BookingStatus.Pending && a.PickupDate.Date <= cr.GetISTDate().Date).ToList().Where(x => x.PackageType_Id == 4 || x.PackageType_Id == 5 || x.PackageType_Id ==6).ToList().Count;
               // var completedBooking = booking.Where(a => a.BookingStatus == (int)BookingStatus.Completed).ToList().Count;              
                var dispatchedBooking = booking.Where(a => a.BookingStatus == (int)BookingStatus.Dispatched && a.IsReleasedCab == false).ToList().Count;
                var cancelledBooking = booking.Where(a => a.BookingStatus == (int)BookingStatus.Cancelled && a.PickupDate.Date == cr.GetISTDate().Date).ToList().Count;
                var unbilledBooking = booking.Where(a => a.BookingStatus == (int)BookingStatus.Dispatched && a.IsReleasedCab == true).ToList().Count;
                var availableCab = cr.GetAvailableCabs().Count;
                var availableDriver = cr.GetAvailableDrivers().Count;
               var allBooking = booking.Where(a => a.PickupDate.Date == cr.GetISTDate().Date).ToList().Count;
                foreach (var item in data)
                {
                    switch (item.Url)
                    {
                        case "/Booking/ShowAllBooking":
                            item.Counts = allBooking;
                            break;
                        case "/Booking/ShowBooking?bookingStatus=2&pbookingCat=1": //pending onCall
                            item.Counts = pendingBooking;
                            break;
                        case "/Booking/ShowBooking?bookingStatus=2&pbookingCat=0":  // pending booking(ETS)
                            item.Counts = pendingBookinETSg;
                            break;
                        case "/Booking/ShowBooking?bookingStatus=1": // dispatch booking
                            item.Counts = dispatchedBooking;
                            break;
                        //case "/Booking/ShowBooking?bookingStatus=5":  // complete booking
                        //    item.Counts = completedBooking;
                        //    break;
                        case "/Booking/ShowBooking?bookingStatus=3": //Cancelled booking
                            item.Counts = cancelledBooking;
                            break;
                        case "/Booking/UnbilledBooking": //Cancelled booking
                            item.Counts = unbilledBooking;
                            break;
                        case "/Booking/AvailableCabs": //Cancelled booking
                            item.Counts = availableCab;
                            break;
                        case "/Booking/AvailableDrivers": //Cancelled booking
                            item.Counts = availableDriver;
                            break;
                        default:
                            item.Counts = -1;
                            break;

                    }
                }
            }
            else if (menuId == 120)
            {

                //var booking = ent.Bookings.ToList();
                //var unbilledBookingLst = booking.Where(a => a.BookingStatus == (int)BookingStatus.Dispatched && a.IsReleasedCab == true).ToList();
                //var unbilledBooking = unbilledBookingLst.Count;
                var unbilledBooking =ent.getbookingHeadCount(finyear).Where(a => a.BookingStatus == (int)BookingStatus.Dispatched && a.IsReleasedCab == true).ToList();
                foreach (var item in data)
                {
                    switch (item.Url)
                    {
                        case "/Booking/UnbilledBooking": //unbilled booking
                            item.Counts = unbilledBooking.Count;
                            break;
                        case "/Booking/UnbilledBookingBypType?pType=2&bookingType=regular": //local booking
                            item.Counts = unbilledBooking.Where(x=>x.PackageType_Id==2).ToList().Count();
                            break;
                        case "/Booking/UnbilledBookingBypType?pType=4&bookingType=regular": //monthly fix booking
                            item.Counts = unbilledBooking.Where(x => x.PackageType_Id ==4).ToList().Count();
                            break;
                        case "/Booking/UnbilledBookingBypType?pType=5&bookingType=regular": //monthly route booking
                            item.Counts = unbilledBooking.Where(x => x.PackageType_Id == 5).ToList().Count();
                            break;
                        case "/Booking/UnbilledBookingBypType?pType=6&bookingType=regular": //monthly trip booking
                            item.Counts = unbilledBooking.Where(x => x.PackageType_Id ==6).ToList().Count();
                            break;
                        case "/Booking/UnbilledBookingBypType?pType=0&bookingType=nrd": //NRD booking
                            item.Counts = unbilledBooking.Where(x => x.BookingType == "nrd").ToList().Count();
                            break;
                        default:
                            item.Counts = 10;
                            break;

                    }
                    //item.Counts = 10;
                }
            }
            else if (menuId == 1120)
            {
                DateTime today = DateTime.Today;
                //var cb = ent.CorporateBills.Where(x=>x.IsCancelled==false&&x.IsBilled==false).ToList().Where(x=>Convert.ToDateTime(x.JourneyClosingDate).Month==today.Month&& Convert.ToDateTime(x.JourneyClosingDate).Year==today.Year).ToList();
                //var booking = ent.Bookings.ToList();
                //var cbData = from b in booking
                //             join cbil in cb.Where(x => x.IsNrg == false).ToList()
                //              on b.Id equals cbil.Booking_Id
                //             select b;
                var cbData = ent.getCloseBookingDatabyMonth().ToList();
                foreach (var item in data)
                {
                    switch (item.Url)
                    {
                        case "/Booking/CorporateInvoiceList?isNrg=false":
                            item.Counts = cbData.Where(x=>x.IsNrg==false).Count();
                            break;
                        case "/Booking/CorporateInvoiceListBypType?isNrg=false&pType=2&bookingType=regular":
                            item.Counts = cbData.Where(x=>x.PackageType_Id==2).Count();
                            break;
                        case "/Booking/CorporateInvoiceListBypType?isNrg=false&pType=4&bookingType=regular":
                            item.Counts = cbData.Where(x => x.PackageType_Id == 4).Count();
                            break;
                        case "/Booking/CorporateInvoiceListBypType?isNrg=false&pType=5&bookingType=regular":
                            item.Counts = cbData.Where(x => x.PackageType_Id == 5).Count();
                            break;
                        case "/Booking/CorporateInvoiceListBypType?isNrg=false&pType=6&bookingType=regular":
                            item.Counts = cbData.Where(x => x.PackageType_Id == 6).Count();
                            break;
                        case "/Booking/CorporateInvoiceListBypType?isNrg=true&pType=0&bookingType=regular":
                            item.Counts = cbData.Where(x => x.IsNrg == true).Count();
                            break;
                        default:
                            item.Counts = 10;
                            break;

                    }
                }
            }
            else
            {
                foreach (var item in data)
                {
                    item.Counts = -1;
                }
            }
            return View(data);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                int id = int.Parse(User.Identity.Name);
                var user = ent.UserLogins.Find(id);
                if (user.Password != model.OldPassword)
                {
                    TempData["msg"] = "Old password is incorrect";
                    return View(model);
                }
                user.Password = model.NewPassword;
                ent.SaveChanges();
                TempData["msg"] = "Password has updated";
                string msg = "Hi " + user.Email + ",\n Your Username Is: " + user.Email + "\n Password :" + user.Password + "";
                EmailOperation.SendEmail("bhupal@vardaanrentacar.com", "Reset Password on CRM of " + user.Email, msg, true);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("ChangePassword");
        }

        public ActionResult LoginHistory(DateTime? sDate, DateTime? eDate, string term = "", int page = 1, int menuId = 0)
        {
            var model = new LoginHistoryViewModel();
            var data = (from lh in ent.LoginHistories
                        join ul in ent.UserLogins on lh.UserLogin_Id equals ul.Id
                        orderby lh.Id descending
                        select new LoginHistoryDTO
                        {
                            Id = lh.Id,
                            UserLogin_Id = lh.UserLogin_Id,
                            UserName = ul.Email,
                            Ip_Address = lh.Ip_Address,
                            UpdateDate = lh.UpdateDate

                        }).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.UserName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            if (sDate != null && eDate != null)
            {
                data = data.Where(a => a.UpdateDate >= sDate && a.UpdateDate <= eDate).ToList();
                page = 1;
            }

            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.History = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        void DeactivateInvalidCabs()
        {
            var dt = DateTime.Now.Date;
            var cabs = ent.Cabs.Where(a => DbFunctions.TruncateTime(a.FitnessVality) < dt
            || DbFunctions.TruncateTime(a.InsurranceValidity) < dt
            || DbFunctions.TruncateTime(a.PolutionValidity) < dt
            || DbFunctions.TruncateTime(a.PermitValidity) < dt
            ).ToList();
            foreach (var cab in cabs)
            {
                cab.IsActive = false;
                ent.SaveChanges();
            }
        }

        public ActionResult Updates(int menuId = 0)
        {
            var logs = (from log in ent.Logs
                        join ul in ent.UserLogins
   on log.UserLogin_Id equals ul.Id
                        orderby log.Id descending
                        select new LogsDTO
                        {
                            Id = log.Id,
                            UserLogin_Id = log.UserLogin_Id,
                            UpdateDate = log.UpdateDate,
                            Description = log.Description,
                            UpdatedBy = ul.Email
                        }
                        ).Take(15).ToList();
            ViewBag.menuId = menuId;
            return View(logs);
        }

        public ActionResult ShowSidebarMenus()
        {
            int userId = int.Parse(User.Identity.Name);
            if (!ent.UserLogins.Any(x => x.Id == userId && x.Role == "Customer"))
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in softwareLinks)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + " and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
                return PartialView(softwareLinks);
            }
            else
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (" + 1141 + "," + 1142 + ")";
                var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in softwareLinks)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + "";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
                return PartialView(softwareLinks);
            }
        }
    }
}