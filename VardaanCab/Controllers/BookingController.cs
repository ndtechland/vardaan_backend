using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;
using VardaanCab.Filter;

namespace VardaanCab.Controllers
{
    [Authorize]
    [CustomFilter]
    public class BookingController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();
        // GET: Booking

        //public ActionResult ShowAllBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0,bool export=false)
        //{
        //    var model = new BookingViewMOdel();
        //    string query = @"select b.*,dbo.[GetPickupDateTime](b.PickupDate,b.PickupTime ) as PickupDateTime,
        //    c.CityName,r.RentalTypeName,ul.Email as UserRole,dc.ModelName as DesiredCar,[dbo].[getUpdateDescription](b.Id) as UpdateDescription,
        //    vm.ModelName as VehicleName,cb.VehicleNumber as VehicleNo,driver.DriverName,ptype.PType as PackageTypeName,
        //    driver.MobileNumber as DriverContactNo from Booking b
        //                            left join CityMaster c on b.City_Id = c.Id
        //                            left join RentalType r on b.RentalType = r.Id                                                          
        //    						left join UserLogin ul on b.UpdatedBy = ul.Id 
        //	left join VehicleModel dc on b.VehicleModel_Id=dc.Id
        //    						left join  Cab cb on b.Cab_Id = cb.Id                        
        //                            left join VehicleModel vm on cb.VehicleModel_Id = vm.Id
        //                            left join Driver driver on b.DriverId = driver.Id
        //	left join PackageType ptype on b.PackageType_Id = ptype.Id
        //	order by PickupDateTime";
        //    var data = ent.Database.SqlQuery<BookingDTO>(query).ToList();
        //    // default condition
        //    if (string.IsNullOrEmpty(term) && sDate == null && eDate == null)
        //    {
        //        DateTime range = cr.GetISTDate();
        //        data = data.Where(a => a.PickupDate.Date == range.Date).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(term) && sDate != null && eDate != null)
        //    {
        //        term = term.ToLower().Trim();
        //        data = data.Where(a => (a.CustomerName.ToLower().Contains(term) || a.ContactNo.Contains(term)  || a.CompanyName.ToLower().Contains(term)|| a.VehicleNo.ToLower().Contains(term)) && (a.PickupDate.Date >= sDate.Value.Date && a.PickupDate.Date <= eDate.Value.Date)).ToList();
        //    }
        //    if ( sDate != null && eDate != null)
        //    {
        //        data = data.Where(a =>  a.PickupDate.Date >= sDate.Value.Date && a.PickupDate.Date <= eDate.Value.Date).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(term))
        //    {
        //        term = term.ToLower().Trim();
        //        data = data.Where(a => a.CustomerName.ToLower().Contains(term) || a.ContactNo.Contains(term) || a.BookingId.ToLower().Contains(term) || a.CompanyName.ToLower().Contains(term)|| a.VehicleNo.ToLower().Contains(term)).ToList();
        //    }



        //    if (export)
        //    {
        //        var ed = (from d in data
        //                  select new BookingExcelModel
        //                  {
        //                      BookingId = d.BookingId,
        //                      Service_City = d.CityName,
        //                      User_Name = d.CustomerName,
        //                      BookerName = d.BookedByPerson,
        //                      Mobile = d.ContactNo,
        //                      Email = d.Email,
        //                      PickupDateTime = d.PickupDateTime,
        //                      Booked_On = d.BookingDate,
        //                      PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
        //                      DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
        //                      Organization = d.CompanyName,
        //                      Booked_By = d.BookedBy,
        //                      DesiredCar = d.DesiredCar,
        //                      Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
        //                      RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
        //                      UpdatedBy = d.UserRole,
        //                      UpdateDate = d.UpdateDate.Date,
        //                      Status = d.BookingStatusName
        //                  }).ToList(); ;
        //        var grid = new GridView();
        //        grid.DataSource = ed;
        //        grid.DataBind();

        //        Response.ClearContent();
        //        Response.Buffer = true;
        //        Response.AddHeader("content-disposition", "attachment; filename =allBookings.xls");
        //        Response.ContentType = "application/vnd.ms-excel";

        //        Response.Charset = "";
        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter htw = new HtmlTextWriter(sw);

        //        grid.RenderControl(htw);

        //        Response.Output.Write(sw.ToString());
        //        Response.Flush();
        //        Response.End();
        //    }

        //    // pagination
        //    int total = data.Count;
        //    int pageSize = 100;
        //    double totalPages = Math.Ceiling(total / (double)pageSize);
        //    int skip = pageSize * (page - 1);
        //    data = data.Skip(skip).Take(pageSize).ToList();
        //    //foreach (var d in data)
        //    //{
        //    //    var logs = ent.Logs.Where(a => a.Booking_Id == d.Id).ToList();
        //    //    string desc = string.Join(",", logs.Select(a => a.Description));
        //    //    d.UpdateDescription = desc;
        //    //}
        //    model.Bookings = data;
        //    model.Term = term;
        //    model.Page = page;
        //    model.NumberOfPages = (int)totalPages;
        //    model.SrNo = skip;
        //    ViewBag.menuId = menuId;
        //    model.sDate = sDate;
        //    model.eDate = eDate;
        //    return View(model);
        //}

        //public ActionResult ShowBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", string bookingStatus = "2", int menuId = 0, bool export = false)
        //{
        //    int pageSize = 100;
        //    int pos = bookingStatus.IndexOf('?');
        //    if (pos >= 0)
        //    {
        //        bookingStatus = bookingStatus.Remove(pos, 1);
        //    }
        //    int bStatus = Convert.ToInt32(bookingStatus);
        //    var model = new BookingViewMOdel();
        //    model.BookingStatus = bStatus;
        //    string query = @"select b.*,dbo.[GetPickupDateTime](b.PickupDate,b.PickupTime ) as PickupDateTime,
        //                               c.CityName,r.RentalTypeName,ul.Email as UserRole,dc.ModelName as DesiredCar,
        //                               vm.ModelName as VehicleName,cb.VehicleNumber as VehicleNo,driver.DriverName,
        //                      ptype.PType as PackageTypeName,[dbo].[getUpdateDescription](b.Id) as UpdateDescription,
        //                               driver.MobileNumber as DriverContactNo from Booking b
        //                               left join CityMaster c on b.City_Id = c.Id
        //                               left join RentalType r on b.RentalType = r.Id                                                          
        //       						left join UserLogin ul on b.UpdatedBy = ul.Id 
        //    left join VehicleModel dc on b.VehicleModel_Id=dc.Id
        //       						left join  Cab cb on b.Cab_Id = cb.Id                        
        //                               left join VehicleModel vm on cb.VehicleModel_Id = vm.Id
        //                               left join Driver driver on b.DriverId = driver.Id
        //    left join PackageType ptype on b.PackageType_Id = ptype.Id
        //    where b.IsReleasedCab=0 and b.BookingStatus=" + bookingStatus + " order by PickupDateTime";



        //    var data = ent.Database.SqlQuery<BookingDTO>(query).ToList();

        //    if (!string.IsNullOrEmpty(term) && sDate != null && eDate != null)
        //    {
        //        term = term.ToLower().Trim();
        //        data = data.Where(a => (!string.IsNullOrEmpty(a.VehicleNo) && a.VehicleNo.ToLower().Contains(term) || a.CompanyName.ToLower().Contains(term)) && (a.PickupDate.Date >= sDate.Value.Date && a.PickupDate.Date <= eDate.Value.Date)).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(term))
        //    {
        //        term = term.ToLower().Trim();
        //        data = data.Where(a => a.CustomerName.ToLower().Contains(term) || a.ContactNo.Contains(term) || a.BookingId.ToLower().Contains(term) || a.CompanyName.ToLower().Contains(term) || !string.IsNullOrEmpty(a.VehicleNo) && a.VehicleNo.ToLower().Contains(term)).ToList();
        //    }
        //    if (sDate != null && eDate != null)
        //    {
        //        data = data.Where(a => a.PickupDate.Date >= sDate.Value.Date && a.PickupDate.Date <= eDate.Value.Date).ToList();
        //    }

        //    if (sDate == null && eDate == null && bookingStatus == "2")
        //    {

        //        data = data.Where(a => a.PickupDate.Date <= cr.GetISTDate()).ToList();
        //    }
        //    if (sDate == null && eDate == null && bookingStatus == "3")
        //    {

        //        data = data.Where(a => a.PickupDate.Date == cr.GetISTDate()).ToList();
        //    }


        //    var total = data.Count;

        //    if (export)
        //    {
        //        string bookings = (BookingStatus)bStatus + " Bookings";

        //        var ed = (from d in data
        //                  select new BookingExcelModel
        //                  {
        //                      BookingId = d.BookingId,
        //                      Service_City = d.CityName,
        //                      User_Name = d.CustomerName,
        //                      BookerName = d.BookedByPerson,
        //                      Mobile = d.ContactNo,
        //                      Email = d.Email,
        //                      PickupDateTime = d.PickupDateTime,
        //                      Booked_On = d.BookingDate,
        //                      PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
        //                      DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
        //                      Organization = d.CompanyName,
        //                      Booked_By = d.BookedBy,
        //                      DesiredCar = d.DesiredCar,
        //                      Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
        //                      RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
        //                      UpdatedBy = d.UserRole,
        //                      UpdateDate = d.UpdateDate.Date,
        //                      Status = d.BookingStatusName
        //                  }).ToList(); ;
        //        var grid = new GridView();
        //        grid.DataSource = ed;
        //        grid.DataBind();

        //        Response.ClearContent();
        //        Response.Buffer = true;
        //        Response.AddHeader("content-disposition", "attachment; filename =" + bookings + ".xls");
        //        Response.ContentType = "application/vnd.ms-excel";

        //        Response.Charset = "";
        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter htw = new HtmlTextWriter(sw);

        //        grid.RenderControl(htw);

        //        Response.Output.Write(sw.ToString());
        //        Response.Flush();
        //        Response.End();
        //    }
        //    // pagination
        //    //total= data.Count;
        //    double totalPages = Math.Ceiling(total / (double)pageSize);
        //    int skip = pageSize * (page - 1);
        //    data = data.Skip(skip).Take(pageSize).ToList();

        //    model.Bookings = data;
        //    model.Term = term;
        //    model.Page = page;
        //    model.NumberOfPages = (int)totalPages;
        //    model.SrNo = skip;
        //    model.sDate = sDate;
        //    model.eDate = eDate;
        //    ViewBag.menuId = menuId;
        //    return View(model);
        //}



        //public ActionResult UnbilledBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0,bool export=false)
        //{
        //    string bookingType = "regular";
        //    var model = new BookingViewMOdel();
        //    model.BookingType = bookingType;
        //    string query = @"select b.*,dbo.[GetPickupDateTime](b.PickupDate,b.PickupTime ) as PickupDateTime,
        //                            c.CityName,r.RentalTypeName,ul.Email as UserRole,dc.ModelName as DesiredCar,
        //                            vm.ModelName as VehicleName,cb.VehicleNumber as VehicleNo,driver.DriverName,
        //                   ptype.PType as PackageTypeName,[dbo].[getUpdateDescription](b.Id) as UpdateDescription,
        //                            driver.MobileNumber as DriverContactNo from Booking b
        //                            left join CityMaster c on b.City_Id = c.Id
        //                            left join RentalType r on b.RentalType = r.Id                                                          
        //    						left join UserLogin ul on b.UpdatedBy = ul.Id 
        //	left join VehicleModel dc on b.VehicleModel_Id=dc.Id
        //    						left join  Cab cb on b.Cab_Id = cb.Id                        
        //                            left join VehicleModel vm on cb.VehicleModel_Id = vm.Id
        //                            left join Driver driver on b.DriverId = driver.Id
        //	left join PackageType ptype on b.PackageType_Id = ptype.Id
        //	where IsReleasedCab=1 and b.BookingStatus=1 order by PickupDateTime";

        //    var data = ent.Database.SqlQuery<BookingDTO>(query).ToList();

        //    if (!string.IsNullOrEmpty(term))
        //    {
        //        term = term.ToLower().Trim();
        //        data = data.Where(a => a.CustomerName.ToLower().Contains(term) || a.ContactNo.Contains(term) || a.BookingId.ToLower().Contains(term) || a.CompanyName.ToLower().Contains(term) || a.VehicleNo.ToLower().Contains(term)).ToList();
        //        //page = 1;
        //    }
        //    if (sDate != null && eDate != null)
        //    {
        //        data = data.Where(a => a.PickupDate.Date >= sDate.Value.Date && a.PickupDate.Date <= eDate.Value.Date).ToList();
        //        //page = 1;
        //    }

        //    if (export)
        //    {
        //        var ed = (from d in data
        //                  select new BookingExcelModel
        //                  {
        //                      BookingId = d.BookingId,
        //                      Service_City = d.CityName,
        //                      User_Name = d.CustomerName,
        //                      BookerName = d.BookedByPerson,
        //                      Mobile = d.ContactNo,
        //                      Email = d.Email,
        //                      PickupDateTime = d.PickupDateTime,
        //                      Booked_On = d.BookingDate,
        //                      PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
        //                      DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
        //                      Organization = d.CompanyName,
        //                      Booked_By = d.BookedBy,
        //                      DesiredCar = d.DesiredCar,
        //                      Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
        //                      RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
        //                      UpdatedBy = d.UserRole,
        //                      UpdateDate = d.UpdateDate.Date,
        //                      Status = d.BookingStatusName
        //                  }).ToList(); ;
        //        var grid = new GridView();
        //        grid.DataSource = ed;
        //        grid.DataBind();

        //        Response.ClearContent();
        //        Response.Buffer = true;
        //        Response.AddHeader("content-disposition", "attachment; filename =Unbilled_Bookings.xls");
        //        Response.ContentType = "application/vnd.ms-excel";

        //        Response.Charset = "";
        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter htw = new HtmlTextWriter(sw);

        //        grid.RenderControl(htw);

        //        Response.Output.Write(sw.ToString());
        //        Response.Flush();
        //        Response.End();
        //    }
        //    // pagination
        //    int total = data.Count;
        //    int pageSize = 100;
        //    double totalPages = Math.Ceiling(total / (double)pageSize);
        //    int skip = pageSize * (page - 1);
        //    data = data.Skip(skip).Take(pageSize).ToList();
        //    //foreach (var d in data)
        //    //{
        //    //    var logs = ent.Logs.Where(a => a.Booking_Id == d.Id).ToList();
        //    //    string desc = string.Join(",", logs.Select(a => a.Description));
        //    //    d.UpdateDescription = desc;
        //    //}
        //    model.Bookings = data;
        //    model.Term = term;
        //    model.Page = page;
        //    model.NumberOfPages = (int)totalPages;
        //    model.SrNo = skip;
        //    ViewBag.menuId = menuId;
        //    model.sDate = sDate;
        //    model.eDate = eDate;
        //    return View(model);
        //}

        public ActionResult ShowBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", string bookingStatus = "2", int menuId = 0, int pbookingCat = 2, bool export = false)
        {
            int pendingBookingType = pbookingCat;
            ViewBag.pendingCat = pendingBookingType;
            int pageSize = 100;
           // int pageSize1 = 2000;
            int pos = bookingStatus.IndexOf('?');
            if (pos >= 0)
            {
                bookingStatus = bookingStatus.Remove(pos, 1);
            }
            int bStatus = Convert.ToInt32(bookingStatus);
            var model = new BookingViewMOdel();
            model.BookingStatus = bStatus;
            //string query = @"execute GetBooking @bookingStatus,@term,@sDate,@eDate,@page,@pageSize,@isExport";
            string query = @"execute GetBookingByPendBookCategory @bookingStatus,@term,@sDate,@eDate,@page,@pageSize,@isExport,@pbookingCat";
            
            var data = ent.Database.SqlQuery<BookingDTO>(query,
                 new SqlParameter("@bookingStatus", bStatus),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", export),//Add new parameter
                new SqlParameter("@pbookingCat", pendingBookingType)
                //new SqlParameter("@userid", 2766)
                ).ToList();
            if (pendingBookingType==1)
            {
                data = data.Where(x => x.PackageType_Id == 1 || x.PackageType_Id ==2 || x.PackageType_Id ==3).ToList();
            }
            else if(pendingBookingType == 0)
            { 
                data = data.Where(x => x.PackageType_Id == 4 || x.PackageType_Id ==5 || x.PackageType_Id ==6).ToList();
            }

            //get count for  pagination
            string query2 = string.Format("execute GetBookingCountByPendBookCategory @bookingStatus,@term,@sDate,@eDate,@page,@pageSize,@isExport,@pbookingCat");
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@bookingStatus", bStatus),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true),//Add new parameter
                new SqlParameter("@pbookingCat", pendingBookingType)
                //new SqlParameter("@userid", 2766)
                ).FirstOrDefault();

           // var total = data.Count;
            if (export)
            {
                string bookings = (BookingStatus)bStatus + " Bookings";

                var ed = (from d in data
                          select 
                          
                          new BookingExcelModel
                          {
                              BookingId = d.BookingId,
                              Organization = d.CompanyName,
                              Service_City = d.CityName,
                              PickupDate = d.PickupDateTime.ToString(),
                              PickupTime = d.PickupDateTime.ToString("HH:mm"),
                              DesiredCar = d.DesiredCar,
                              RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
                              User_Name = d.CustomerName,
                              PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
                              DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,                           
                              Mobile = d.ContactNo,
                              BookerName = d.BookedByPerson,
                              Email = d.Email,
                             // PickupDateTime = d.PickupDateTime,
                           
                              // Booked_On = d.BookingDate,
                              Booked_On = d.BookingDate.ToString("dd/MM/yyyy"),
                              Booked_By = d.BookedBy,                              
                              Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
                           
                              UpdatedBy = d.UserRole,
                              UpdateDate = d.UpdateDate.Date.ToString("dd/MM/yyyy"),
                              Status = d.BookingStatusName,
                              RouteNo=d.BookingNo
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =" + bookings + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
           double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);

            model.Bookings = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            model.sDate = sDate;
            model.eDate = eDate;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult ShowAllBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0, bool export = false)
        {
            int pageSize = 100;
            var model = new BookingViewMOdel();
            string query = @"execute GetAllBooking @term,@sDate,@eDate,@page,@pageSize,@isExport";
            var data = ent.Database.SqlQuery<BookingDTO>(query,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", export)
                ).ToList();

            // for getting all records,so that we can do pagination
            string query2 = @"execute GetAllBookingCount @term,@sDate,@eDate,@page,@pageSize,@isExport";
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true)
                ).FirstOrDefault();
            //var total = data2.Count;
            if (export)
            {
                var ed = (from d in data
                          select new BookingExcelModel
                          {
                              BookingId = d.BookingId,
                              Organization = d.CompanyName,
                              Service_City = d.CityName,
                              PickupDate = d.PickupDate.ToString("dd/MM/yyyy"),
                              PickupTime = d.PickupTime.ToString("dd\\:hh\\:mm"),
                              DesiredCar = d.DesiredCar,
                              RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
                              User_Name = d.CustomerName,
                              PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
                              DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
                              Mobile = d.ContactNo,
                              BookerName = d.BookedByPerson,                              
                              Email = d.Email,
                             // PickupDateTime = d.PickupDateTime,
                             // Booked_On = d.BookingDate,
                           
                              Booked_On = d.BookingDate.ToString("dd/MM/yyyy"),
                            
                              Booked_By = d.BookedBy,
                            
                              Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
                             
                              UpdatedBy = d.UserRole,
                              UpdateDate = d.UpdateDate.Date.ToString("dd/MM/yyyy"),
                              Status = d.BookingStatusName,
                              RouteNo=d.RouteNo
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =allBookings.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            model.Bookings = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            model.sDate = sDate;
            model.eDate = eDate;
            return View(model);
        }

        public ActionResult UnbilledBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "",string routeNo="", int menuId = 0, bool export = false)
        {
            int pageSize = 40;
            string bookingType = "regular";
            string routeno = String.Empty;
            routeno = Convert.ToString(routeNo);
            var model = new BookingViewMOdel();
            model.BookingType = bookingType;
            string query = @"execute GetUnbilledBookingNew @term,@sDate,@eDate,@page,@pageSize,@isExport,@routeNo";
            var data = ent.Database.SqlQuery<BookingDTO>(query,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", export),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).ToList();

            // count for pagination
            string query2 = @"execute [GetUnbilledBookingCountNew] @term,@sDate,@eDate,@page,@pageSize,@isExport,@routeNo";
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).FirstOrDefault();
            //  var total = data2.Count;

            if (export)
            {
                var ed = (from d in data
                          select new BookingExcelModel
                          {
                              BookingId = d.BookingId,
                              Service_City = d.CityName,
                              User_Name = d.CustomerName,
                              BookerName = d.BookedByPerson,
                              BookedByPersonMobileNo=d.BookedByPersonMobileNo,
                              Mobile = d.ContactNo,
                              Email = d.Email,
                              //PickupDateTime = d.PickupDateTime,
                              //Booked_On = d.BookingDate,
                              PickupDate = d.PickupDate.ToString("dd/MM/yyyy"),
                              PickupTime = d.PickupTime.ToString("dd\\:hh\\:mm"),
                              Booked_On = d.BookingDate.ToString("dd/MM/yyyy"),
                              PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
                              DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
                              Organization = d.CompanyName,
                              Booked_By = d.BookedBy,
                              DesiredCar = d.DesiredCar,
                              Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
                              RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
                              UpdatedBy = d.UserRole,
                              UpdateDate = d.UpdateDate.Date.ToString("dd/MM/yyyy"),
                              Status = d.BookingStatusName,
                              RouteNo=d.RouteNo
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =Unbilled_Bookings.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            // pagination
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);

            model.Bookings = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            model.sDate = sDate;
            model.eDate = eDate;
            model.routeno = String.IsNullOrEmpty(routeno) ? 0 : Convert.ToInt32(routeno); 
            return View(model);
        }

        public ActionResult UnbilledBookingBypType(DateTime? sDate, DateTime? eDate,  int page = 1, string term = "", string routeNo = "", int menuId = 0, int pType=0, string bookingType="", bool export = false)
        {
            int pageSize = 40;
            string bookingType1 = bookingType;
            string routeno = String.Empty;
            routeno = Convert.ToString(routeNo);
            var model = new BookingViewMOdel();
            model.BookingType = bookingType1;
            
            string query = @"execute GetUnbilledBookingByPTypeNew @term,@sDate,@eDate,@page,@pageSize,@pType,@bType,@isExport,@routeNo";
            var data = ent.Database.SqlQuery<BookingDTO>(query,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pType", pType),
                new SqlParameter("@bType", bookingType1),
                new SqlParameter("@isExport", export),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).ToList();

            // count for pagination
            string query2 = @"execute [GetUnbilledBookingCountByPTypeNew] @term,@sDate,@eDate,@page,@pageSize,@pType,@bType,@isExport,@routeNo";
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pType", pType),
                new SqlParameter("@bType", bookingType1),
                new SqlParameter("@isExport", true),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).FirstOrDefault();
            //var total = data.Count;

           
            if (pType == 0)
            {
                ViewBag.PackageType = "NRD";
                ViewBag.PackageTypeID = 0;
            }
            else
            {
                ViewBag.PackageType = ent.PackageTypes.Where(x => x.Id == pType).FirstOrDefault().PType;
                ViewBag.PackageTypeID = pType;
            }

            if (export)
            {
                var ed = (from d in data
                          select new BookingExcelModel
                          {
                              BookingId = d.BookingId,
                              Service_City = d.CityName,
                              User_Name = d.CustomerName,
                              BookerName = d.BookedByPerson,
                              BookedByPersonMobileNo = d.BookedByPersonMobileNo,
                              Mobile = d.ContactNo,
                              Email = d.Email,
                              //PickupDateTime = d.PickupDateTime,
                              //Booked_On = d.BookingDate,
                              PickupDate = d.PickupDate.ToString("dd/MM/yyyy"),
                              PickupTime = d.PickupTime.ToString("dd\\:hh\\:mm"),
                              Booked_On = d.BookingDate.ToString("dd/MM/yyyy"),

                              PickupAddress = d.PickupAddress + "\r\n" + d.PickupLandMark,
                              DropAddress = d.DropAddress + "\r\n" + d.DropLandmark,
                              Organization = d.CompanyName,
                              Booked_By = d.BookedBy,
                              DesiredCar = d.DesiredCar,
                              Vehicle_Chauffeur = "Vehicle :" + d.VehicleName + "\r\n" + "Vehicle No:" + d.VehicleNo + "\r\n" + "Driver :" + d.DriverName + "\r\n" + "Contact No :" + d.DriverContactNo + "",
                              RentalTypeName = "(" + d.PackageTypeName + ")" + d.RentalTypeName,
                              UpdatedBy = d.UserRole,
                              UpdateDate = d.UpdateDate.Date.ToString("dd/MM/yyyy"),
                              Status = d.BookingStatusName,
                              RouteNo=d.RouteNo
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =Unbilled_Bookings.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            // pagination
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);

            model.Bookings = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            model.sDate = sDate;
            model.eDate = eDate;
            model.routeno = String.IsNullOrEmpty(routeno) ? 0 : Convert.ToInt32(routeno);
            return View(model);
        }


        private IEnumerable<VehicleModel> GetVehicleModelListFromPackage(int customerId)
        {
            var client = ent.Customers.Find(customerId);

            //find parent client 
            if (client.ParentCustomer_Id != null)
            {
                var parentClient = ent.Customers.Find(client.ParentCustomer_Id);
                if (parentClient != null)
                {
                    customerId = parentClient.Id;
                }
            }

            string vehicleModelQuery = @"select * from VehicleModel where Id in (select distinct vehiclemodel_Id from ClientPackage where customer_id=" + customerId + ")";
            var vehicles = ent.Database.SqlQuery<VehicleModel>(vehicleModelQuery).ToList();
            return vehicles;
        }

        private IEnumerable<VehicleModel> GetVehicleModelListFromNRGPackage()
        {
            string vehicleModelQuery = @"select * from VehicleModel where Id in (select distinct vehiclemodel_Id from corporatepackage)";
            var vehicles = ent.Database.SqlQuery<VehicleModel>(vehicleModelQuery).ToList();
            return vehicles;
        }

        public ActionResult CabBook(int menuId = 0)
        {
            var model = new BookingDTO();
            model.OrganizationList = new SelectList(ent.Customers.Where(x=>x.IsActive==true).ToList().OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
            model.VehicleModels = new SelectList(Enumerable.Empty<SelectListItem>());
            model.StateList = new SelectList(ent.StateMasters.OrderBy(e => e.StateName).ToList(), "Id", "StateName");
            model.PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType");
            model.Cities = new SelectList(ent.CityMasters.OrderBy(e => e.CityName).ToList(), "Id", "CityName");
            //  ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CabBook(BookingDTO model)
        {
            if (model.BookingType == "nrd")
            {
                ModelState.Remove("Client_Id");
            }
            if (model.PickupDateTimeList.Count() < 1)
            {
                TempData["msg"] = "Please select pickup dates";
                return RedirectToAction("CabBook", new { menuId = model.MenuId });
            }
           

            model.StateList = new SelectList(ent.StateMasters.OrderBy(a=>a.StateName).ToList(), "Id", "StateName");
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.Cities = new SelectList(ent.CityMasters.OrderBy(a=>a.CityName).ToList(), "Id", "CityName");
            model.PacakgeTypes = new SelectList(ent.PackageTypes, "Id", "PType");
            // Create Rental List
            if (model.PackageType_Id > 0)
            {
                model.RentalList = new SelectList(ent.RentalTypes.Where(a => a.PackageType_Id == model.PackageType_Id).ToList(), "Id", "RentalTypeName");
            }
            else
            {
                model.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            // Create Vehicle Model
            if (model.BookingType == "nrd" && model.NrgType == "corporate" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName");
            }
            else if (model.BookingType == "regular" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName");
            }
            else if (model.BookingType == "nrd")
            {
                var data = GetVehicleModelListFromNRGPackage();
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName");
            }
            else
            {
                model.VehicleModels = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            //Checking validation
            if (!ModelState.IsValid)
                return View(model);
            if (model.City_Id == null || model.City_Id == 0)
            {
                TempData["msg"] = "Please select city from Service City Dropdown,do not type manually.";
                return View(model);
            }

            //rental type
            var rentalType = ent.RentalTypes.Find(model.RentalType);
            //package type
            var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);

            // check whether package exist or not
            if (model.BookingType == "nrd" && model.NrgType == "walkin")
            {
                var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                if (nrgPackage == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                {
                    var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (nrgPackage8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }
            }
            else
            {
                // start:  fetching parent id for sister company
                int compareClientId = model.Client_Id;
                var currentClient = ent.Customers.AsNoTracking().FirstOrDefault(a => a.Id == model.Client_Id);
                if (currentClient != null)
                {
                    if (currentClient.ParentCustomer_Id != null)
                        compareClientId = (int)currentClient.ParentCustomer_Id;
                }
                // End: fetching parent id for sister company
                var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                if (package == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && package.MinHrs == 4)
                {
                    var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (package8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }
            }

           
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var dt = cr.GetISTDate();
                    if (model.PickupDateTimeList.Count() == 1)
                    {
                        model.CompanyName = model.BookingType == "nrd" ? "NRD" : ent.Customers.Find(model.Client_Id).CompanyName;
                        model.BookingDate = dt;                       
                        model.BookingId = cr.GenerateBookingId(model.BookingType);
                        model.FinYear = Convert.ToInt32(model.BookingId.Substring(3, 2));
                        model.UpdatedBy = cr.GetUserDetailId();
                        var user = ent.UserLogins.Find(cr.GetUserDetailId());
                        model.UpdateDate = dt;
                        model.BookedBy = user.Email;
                        model.BookingStatus = (int)BookingStatus.Pending;

                        var data = Mapper.Map<Booking>(model);
                        data.CreateBy = user.Email;
                        data.CreateDate = cr.GetISTDate();
                        if (model.BookingType == "nrd" && model.NrgType == "walkin")
                        {
                            data.Client_Id = 0;
                        }
                        data.PickupDate = model.PickupDateTimeList[0].PickupDate;
                        data.PickupTime = model.PickupDateTimeList[0].PickupTime;
                        data.ReportingTime = model.PickupDateTimeList[0].ReportingTime;
                        ent.Bookings.Add(data);
                        ent.SaveChanges();
                        var log = new Log
                        {
                            Booking_Id = data.Id,
                            UpdateDate = cr.GetISTDate(),
                            UserLogin_Id = user.Id,
                            Description = "Booking with Id " + data.BookingId + " is Created by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                        };
                        ent.Logs.Add(log);
                        ent.SaveChanges();
                        tran.Commit();
                        TempData["msg"] = "Successfully booked";
                        var cab = ent.Cabs.Find(data.Cab_Id);
                        return RedirectToAction("BookingConfirmationInvoice1", new { id = data.Id, menuId = model.MenuId });
                    }
                    else
                    {                        
                        string bookingId = cr.GenerateBookingId(model.BookingType);
                        int i = 0;
                        int length = bookingId.Length;
                        //string sPart = bookingId.Substring(0, 3);
                        string sPart = bookingId.Substring(0, 6);
                        // string iPart = bookingId.Substring(3, length - 3);
                        string iPart = bookingId.Substring(6, length - 6);

                        string pickupLoc = model.PickupAddress;
                        string pickupLandmark = model.PickupLandMark;
                        string dropLoc = model.DropAddress;
                        string dropLandmark = model.DropLandmark;
                        foreach (var dts in model.PickupDateTimeList)
                        {
                            if (dts.Type != null && dts.Type.ToLower() == "drop")
                            {
                                model.PickupAddress = dropLoc;
                                model.PickupLandMark = dropLandmark;
                                model.DropAddress = pickupLoc;
                                model.DropLandmark = pickupLandmark;
                            }

                            int new_i = int.Parse(iPart) + i;
                            bookingId = sPart + new_i;
                            model.CompanyName = model.BookingType == "nrd" ? "NRD" : ent.Customers.Find(model.Client_Id).CompanyName;
                            model.BookingDate = dt;
                            model.BookingId = bookingId;
                            model.FinYear = Convert.ToInt32(model.BookingId.Substring(3, 2));
                            model.UpdatedBy = cr.GetUserDetailId();
                            var user = ent.UserLogins.Find(cr.GetUserDetailId());
                            model.UpdateDate = dt;
                            model.BookedBy = user.Email;
                            model.BookingStatus = (int)BookingStatus.Pending;
                            var data = Mapper.Map<Booking>(model);
                            data.CreateBy = user.Email;
                            data.CreateDate = cr.GetISTDate();
                            if (model.BookingType == "nrd" && model.NrgType == "walkin")
                            {
                                data.Client_Id = 0;
                            }
                            data.PickupDate = model.PickupDateTimeList[i].PickupDate;
                            data.PickupTime = model.PickupDateTimeList[i].PickupTime;
                            data.ReportingTime = model.PickupDateTimeList[i].ReportingTime;
                            ent.Bookings.Add(data);
                            ent.SaveChanges();
                            var log = new Log
                            {
                                Booking_Id = data.Id,
                                UpdateDate = cr.GetISTDate(),
                                UserLogin_Id = user.Id,
                                Description = "Booking with Id " + data.BookingId + " is Created by " + user.Email + " on" + cr.GetISTDate()
                            };
                            ent.Logs.Add(log);
                            i++;
                            ent.SaveChanges();
                        }
                        tran.Commit();
                        TempData["msg"] = "Successfully booked";
                        return RedirectToAction("CabBook", new { menuId = model.MenuId });
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Some error occurred";
                }
            }
            return View(model);
        }


        public ActionResult ScheduleCabBook(int menuId = 0)
        {
            var model = new ScheduleBookingDTO();
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.StateList = new SelectList(ent.StateMasters.OrderBy(x=>x.StateName).ToList(), "Id", "StateName");
            model.Cities = new SelectList(ent.CityMasters.OrderBy(a=>a.CityName).ToList(), "Id", "CityName");
            model.BookingRecords = new List<BookingRecordModel>
            {
              new BookingRecordModel
              {
                     PickupAddress="",
                     DropAddress="",
                     RentalList = new SelectList(Enumerable.Empty<SelectListItem>()),
                     PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType"),
                     VehicleModels = new SelectList(ent.VehicleModels.ToList(),"Id","ModelName")
              }
             };
            //  ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult ScheduleCabBook(ScheduleBookingDTO model)
        {
            int customerId = 0;
            if (model.BookingType == "nrd")
            {
                ModelState.Remove("Client_Id");
            }
            if (model.BookingRecords.Count() < 1)
            {
                TempData["msg"] = "Please select pickup dates";
                return RedirectToAction("CabBook", new { menuId = model.MenuId });
            }
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.Cities = new SelectList(ent.CityMasters.ToList(), "Id", "CityName");
            model.RouteNo = 0;
            bool isPackageAvailable = true;
            string pckgMsg = "";
            foreach (var r in model.BookingRecords)
            {
                r.PacakgeTypes = new SelectList(ent.PackageTypes, "Id", "PType");
                r.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
                // Create Rental List
                if (r.PackageType_Id > 0)
                {
                    r.RentalList = new SelectList(ent.RentalTypes.Where(a => a.PackageType_Id == r.PackageType_Id).ToList(), "Id", "RentalTypeName");
                }
                else
                {
                    r.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
                }
                //rental type
                var rentalType = ent.RentalTypes.Find(r.RentalType);
                //package type
                var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
                //vehicle model
                var vehicleModel = ent.VehicleModels.Find(r.VehicleModel_Id);
                // check whether package exist or not
                if (model.BookingType == "nrd" && model.NrgType == "walkin")
                {
                    var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == r.VehicleModel_Id && a.RentalType_Id == r.RentalType);
                    if (nrgPackage == null)
                    {
                        isPackageAvailable = false;
                        pckgMsg += $@" || Package of {vehicleModel.ModelName}/{rentalType.RentalTypeName} is not available";

                    }
                    if (nrgPackage != null && packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                    {
                        var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == r.VehicleModel_Id && a.MinHrs == 8);
                        if (nrgPackage8Hour == null)
                        {
                            isPackageAvailable = false;
                            pckgMsg += $@" || Package of {vehicleModel.ModelName} - 8/80 is not available";

                        }
                    }
                }
                else
                {
                    // customerId = model.Client_Id;
                    // start:  fetching parent id for sister company
                    int compareClientId = model.Client_Id;
                    var currentClient = ent.Customers.AsNoTracking().FirstOrDefault(a => a.Id == model.Client_Id);
                    if (currentClient != null)
                    {
                        if (currentClient.ParentCustomer_Id != null)
                            compareClientId = (int)currentClient.ParentCustomer_Id;
                    }
                    // End: fetching parent id for sister company

                    var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == r.VehicleModel_Id && a.RentalType_Id == r.RentalType);
                    if (package == null)
                    {
                        isPackageAvailable = false;
                        pckgMsg += $@" || Package of {vehicleModel.ModelName}/{rentalType.RentalTypeName} is not available";

                    }
                    if (package != null && packageType.PType == "Local Run" && package.MinHrs == 4)
                    {
                        var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == r.VehicleModel_Id && a.MinHrs == 8);
                        if (package8Hour == null)
                        {
                            isPackageAvailable = false;
                            pckgMsg += $@" || Package of {vehicleModel.ModelName} - 8/80 is not available";
                        }
                    }
                }
            }

            if (!isPackageAvailable)
            {
                TempData["msg"] = pckgMsg;
                return View(model);
            }
            //Checking validation
            if (!ModelState.IsValid)
                return View(model);
            if (model.City_Id == null || model.City_Id == 0)
            {
                TempData["msg"] = "Please select city from Service City Dropdown,do not type manually.";
                return View(model);
            }
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var billedIds = new List<int>();
                    var dt = cr.GetISTDate();
                    string bookingId = cr.GenerateBookingId(model.BookingType);
                    int i = 0;
                    foreach (var d in model.BookingRecords)
                    {
                        int length = bookingId.Length;

                        //string sPart = bookingId.Substring(0, 3);
                        //string iPart = bookingId.Substring(3, length - 3);

                        string sPart = bookingId.Substring(0, 6);
                        string iPart = bookingId.Substring(6, length - 6);

                        int new_i = int.Parse(iPart) + i;
                        bookingId = sPart + new_i;
                        model.CompanyName = model.BookingType == "nrd" ? "NRD" : ent.Customers.Find(model.Client_Id).CompanyName;
                        model.BookingDate = dt;
                        model.BookingId = bookingId;
                        model.FinYear = Convert.ToInt32(model.BookingId.Substring(3, 2));

                        model.UpdatedBy = cr.GetUserDetailId();
                        var user = ent.UserLogins.Find(cr.GetUserDetailId());
                        model.UpdateDate = dt;
                        model.BookedBy = user.Email;
                        model.BookingStatus = (int)BookingStatus.Pending;
                        var data = Mapper.Map<Booking>(model);
                        data.CreateBy = user.Email;
                        data.CreateDate = cr.GetISTDate();
                        if (model.BookingType == "nrd" && model.NrgType == "walkin")
                        {
                            data.Client_Id = 0;
                        }
                        data.PickupAddress = d.PickupAddress;
                        data.PickupLandMark = d.PickupLandMark;
                        data.DropAddress = d.DropAddress;
                        data.DropLandmark = d.DropLandmark;
                        data.PickupDate = d.PickupDate;
                        data.PickupTime = d.PickupTime;
                        data.ReportingTime = d.ReportingTime;
                        data.VehicleModel_Id = d.VehicleModel_Id;
                        data.RentalType = d.RentalType;
                        data.PackageType_Id = d.PackageType_Id;
                        ent.Bookings.Add(data);
                        ent.SaveChanges();
                        billedIds.Add(data.Id);
                        var log = new Log
                        {
                            Booking_Id = data.Id,
                            UpdateDate = cr.GetISTDate(),
                            UserLogin_Id = user.Id,
                            Description = "Booking with Id " + data.BookingId + " is Created by " + user.Email + " on" + cr.GetISTDate()
                        };
                        ent.Logs.Add(log);
                        i=1;
                    }
                    ent.SaveChanges();
                    var multipleBooking = new MultipleBookingFile { Customer_Id = customerId, CreateDate = DateTime.Now };
                    ent.MultipleBookingFiles.Add(multipleBooking);
                    ent.SaveChanges();
                    foreach (var id in billedIds)
                    {
                        var detail = new MultipleBookingFileDetail
                        {
                            Booking_Id = id,
                            MultipleBookingFile_Id = multipleBooking.Id
                        };
                        ent.MultipleBookingFileDetails.Add(detail);
                    }
                    ent.SaveChanges();
                    tran.Commit();
                    //  TempData["msg"] = "Successfully booked";
                    return RedirectToAction("ShowMultipleBookingConfirmation", new { menuId = model.MenuId, multipleBookingFileId = multipleBooking.Id });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Some error occurred";
                }
            }
            return View(model);
        }

        public ActionResult ShowMultipleBookingConfirmation(int multipleBookingFileId, int menuId = 0, bool isPdf = false)
        {
            var model = new MultipleBookingVm();

            var bookingIds = ent.MultipleBookingFileDetails.Where(a => a.MultipleBookingFile_Id == multipleBookingFileId).Select(a => a.Booking_Id).ToList();
            List<BookingDTO> bookings = new List<BookingDTO>();
            foreach (var id in bookingIds)
            {
                var data = (from b in ent.Bookings
                            join c in ent.CityMasters on b.City_Id equals c.Id
                            join r in ent.RentalTypes on b.RentalType equals r.Id
                            join vm in ent.VehicleModels on b.VehicleModel_Id equals vm.Id
                            where b.Id == id

                            select new BookingDTO
                            {
                                Id = b.Id,
                                CustomerName = b.CustomerName,
                                CityName = c.CityName,
                                PickupAddress = b.PickupAddress,
                                Email = b.Email,
                                ContactNo = b.ContactNo,
                                BookingDate = b.BookingDate,
                                CompanyName = b.CompanyName,
                                BookingId = b.BookingId,
                                PickupLandMark = b.PickupLandMark,
                                DropAddress = b.DropAddress,
                                DropLandmark = b.DropLandmark,
                                PickupDate = b.PickupDate,
                                PickupTime = b.PickupTime,
                                BookedBy = b.BookedBy,
                                CarModelName = vm.ModelName,
                                RentalTypeName = r.RentalTypeName,
                                ReportingTime = b.ReportingTime,
                                BookingConfirmFile = b.BookingConfirmFile,
                                BookingInstruction = b.BookingInstruction,
                                BookedByPerson = b.BookedByPerson,
                                IsVIP = b.IsVIP
                            }).FirstOrDefault();
                bookings.Add(data);
            }
            model.MenuId = menuId;
            model.MultipleBookingId = multipleBookingFileId;
            model.IsPdf = isPdf;
            var fileData = ent.MultipleBookingFiles.Find(multipleBookingFileId);
            model.MultipleBookingFile = fileData.ConfirmationFile;
            model.Booking = bookings;
            return View(model);
        }

        public ActionResult GenerateMultiplePdfBookingInvoice(int id)
        {
            try
            {
                var mulipleConfirm = ent.MultipleBookingFiles.Find(id);
                if (string.IsNullOrEmpty(mulipleConfirm.ConfirmationFile))
                {

                    var dt = DateTime.Now;
                    string filename = "BookingConfirmation-" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + ".pdf";
                    string path = Server.MapPath("/Files/") + filename;
                    mulipleConfirm.ConfirmationFile = filename;
                    ent.SaveChanges();
                    return new ActionAsPdf("ShowMultipleBookingConfirmation", new { multipleBookingFileId = id, isPdf = true }) { FileName = filename, SaveOnServerPath = path };
                }
                return Content("already added");
            }
            catch (Exception ex)
            {
                return Content("error");
            }
            //return View(data);
        }

        public ActionResult SendMultipleBookConfirmInvoice(int id, int menuId = 0)
        {
            try
            {
                var mulipleConfirm = ent.MultipleBookingFiles.Find(id);
                var detail = ent.MultipleBookingFileDetails.Where(a => a.MultipleBookingFile_Id == id);
                if (detail.Count() < 1)
                    return Content("Server error.");
                var data = ent.Bookings.Find(detail.FirstOrDefault().Booking_Id);
                var vehicle = ent.VehicleModels.Find(data.VehicleModel_Id);
                var path = Server.MapPath("~/Files/" + mulipleConfirm.ConfirmationFile);
                string msg = $@"Dear Sir / Madam,<br/><br/>

Greetings from VARDAAN Car Rental Services !!!! <br/><br/>

We are pleased to confirm the booking of {vehicle.ModelName}. The booking ID is {data.BookingId} dated {data.PickupDate.ToString("dd-MMM-yyyy")}.<br/>
Chauffeur and cab details will be sent prior 2 hours of pickup date and time.<br/>
For more detail please check the attachment.
<br/><br/>
For any further clarifications, write to us or call us at 24x7 Helpline numbers, mentioned below.<br/>
LL: 0120 - 4204668; Mo: 08130874555; Email: reservation @vardaanrentacar.com
<br/><br/>
--
<br/><br/>
For any further information or clarification please feel free to get in touch with undersigned.
<br/><br/>
Thanks & Regards<br/>
VRC Team";
                EmailOperation.SendEmail(data.Email, "Vardaan Car Rental Booking Confirmation", msg, true, path);
                return Content("Invoice has sent.");
            }
            catch (Exception ex)
            {
                return Content("Server error.");
            }
        }

        public ActionResult Edit(int id, DateTime? sDate, DateTime? eDate, int bookingStatus = 0, string term = "", int page = 1, int menuId = 0, int pbookingCat = 2, bool fromUnbilled = false)
        {
            var data = ent.Bookings.Find(id);
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;
            var model = Mapper.Map<BookingDTO>(data);
            model.PreviousBookingType = data.BookingType;
            model.OrganizationList = new SelectList(ent.Customers.Where(x=>x.IsActive==true).ToList().OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName", data.Client_Id);
            model.StateList = new SelectList(ent.StateMasters.OrderBy(e => e.StateName).ToList(), "Id", "StateName", data.NrdStateId);
            model.Cities = new SelectList(ent.CityMasters.OrderBy(e => e.CityName).ToList(), "Id", "CityName");
            var city = ent.CityMasters.Find(data.City_Id);
            if (city != null)
                model.CityName = city.CityName;
            List<VehicleModel> vehicles = new List<VehicleModel>();
            if (data.BookingType == "regular")
            {
                int clientId = data.Client_Id;
                // var client = ent.Customers.Find(data.Client_Id);

                //find parent client 
                //if (client.ParentCustomer_Id != null)
                //{
                //    var parentClient = ent.Customers.Find(client.ParentCustomer_Id);
                //    if (parentClient != null)
                //    {
                //        clientId = parentClient.Id;
                //    }
                //}
                vehicles = GetVehicleModelListFromPackage(clientId).ToList();
            }
            else
            {
                vehicles = GetVehicleModelListFromNRGPackage().ToList();
            }
            model.VehicleModels = new SelectList(vehicles, "Id", "ModelName", data.VehicleModel_Id);
            model.PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", data.PackageType_Id);
            model.RentalList = new SelectList(ent.RentalTypes.Where(a=>a.PackageType_Id==data.PackageType_Id).ToList(), "Id", "RentalTypeName", data.RentalType);
            ViewBag.pendingCat = pbookingCat;
            model.MenuId = menuId;
            model.Term = term;
            model.Page = page;
            model.sDate = sDate;
            model.eDate = eDate;
            model.bStatus = bookingStatus==5?2: bookingStatus;
            model.FromUnbilled = fromUnbilled;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BookingDTO model)
        {
            if (model.BookingType == "nrd")
            {
                ModelState.Remove("Client_Id");
            }
            string pCategory = Request["pbookingCat"].ToString();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", model.NrdStateId);
            model.Cities = new SelectList(ent.CityMasters.ToList(), "Id", "CityName");
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName", model.Client_Id);
            List<VehicleModel> vehicles = new List<VehicleModel>();
            if (model.PreviousBookingType.ToLower() != model.BookingType.ToLower())
            {
                model.BookingId = cr.GenerateBookingId(model.BookingType);
               
            }
            if (model.BookingType == "regular")
            {
                int clientId = model.Client_Id;
                var client = ent.Customers.Find(clientId);
                model.NrdBookingMode = null;
                //model.NrdStateId = null;
                model.NrgType = null;

                //find parent client 
                if (client.ParentCustomer_Id != null)
                {
                    var parentClient = ent.Customers.Find(client.ParentCustomer_Id);
                    if (parentClient != null)
                    {
                        clientId = parentClient.Id;
                    }
                }
                vehicles = GetVehicleModelListFromPackage(clientId).ToList();
            }
            else
            {
                vehicles = GetVehicleModelListFromNRGPackage().ToList();
            }
            //  model.VehicleModels = new SelectList(vehicles, "Id", "ModelName", model.VehicleModel_Id);
            model.PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", model.PackageType_Id);
            //  model.RentalList = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName", model.RentalType);
            // Create Rental List
            if (model.PackageType_Id > 0)
            {
                model.RentalList = new SelectList(ent.RentalTypes.Where(a => a.PackageType_Id == model.PackageType_Id).ToList(), "Id", "RentalTypeName", model.RentalType);
            }
            else
            {
                model.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            // Create Vehicle Model
            if (model.BookingType == "nrd" && model.NrgType == "corporate" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else if (model.BookingType == "regular" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else if (model.BookingType == "nrd")
            {
                var data = GetVehicleModelListFromNRGPackage();
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else
            {
                model.VehicleModels = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            //Checking validation
            if (!ModelState.IsValid)
                return View(model);

            //rental type
            var rentalType = ent.RentalTypes.Find(model.RentalType);
            //package type
            var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
            // check whether package exist or not
            if (model.BookingType == "nrd" && model.NrgType == "walkin")
            {
                var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                if (nrgPackage == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                {
                    var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (nrgPackage8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }
            }
            else
            {
                // start:  fetching parent id for sister company
                int compareClientId = model.Client_Id;
                var currentClient = ent.Customers.AsNoTracking().FirstOrDefault(a => a.Id == model.Client_Id);
                if (currentClient != null)
                {
                    if (currentClient.ParentCustomer_Id != null)
                        compareClientId = (int)currentClient.ParentCustomer_Id;
                }
                // End: fetching parent id for sister company
                var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                if (package == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && package.MinHrs == 4)
                {
                    var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (package8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }
            }

            using (var tran = ent.Database.BeginTransaction())
            {
                var user = ent.UserLogins.Find(cr.GetUserDetailId());
                try
                {
                    model.CompanyName = model.BookingType == "nrd" && model.NrgType == "walkin" ? "NRD" : ent.Customers.Find(model.Client_Id).CompanyName;
                    model.FinYear = Convert.ToInt32(model.BookingId.Substring(3, 2));
                    model.UpdatedBy = user.Id;
                    model.UpdateDate = cr.GetISTDate();
                    model.BookingConfirmFile = null;
                    if (model.BookingType == "nrd" && model.NrgType == "walkin")
                    {
                        model.Client_Id = 0;
                    }
                    var vendor = Mapper.Map<Booking>(model);
                    ent.Entry(vendor).State = System.Data.Entity.EntityState.Modified;

                    var log = new Log
                    {
                        Booking_Id = model.Id,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "Booking with Id " + model.BookingId + " is Updated by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                    tran.Commit();
                    if (model.FromUnbilled)
                    {
                        if (model.BookingType == "regular")
                        {
                           
                            if (model.PackageType_Id == 1 || model.PackageType_Id == 2 || model.PackageType_Id == 3)
                            {
                                return RedirectToAction("UnbilledBookingBypType", new { page = model.Page, term = model.Term, pType = model.PackageType_Id, bookingType = model.BookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            }
                            else
                            {
                                return RedirectToAction("UnbilledBookingBypType", new { page = model.Page, term = model.Term, pType = model.PackageType_Id, bookingType = model.BookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            }
                            //UnbilledBookingBypType?pType=5&bookingType=regular&menuId=120
                            //return RedirectToAction("UnbilledBooking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, menuId = model.MenuId });
                        }
                        else if (model.BookingType == "nrd")
                        {
                            return RedirectToAction("UnbilledBookingBypType", new { pType = 0, bookingType = model.BookingType,sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, menuId = model.MenuId });
                        }else
                        {
                            return RedirectToAction("UnbilledBooking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, menuId = model.MenuId });
                        }
                    }
                    else
                        return RedirectToAction("ShowBooking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, bookingStatus = model.BookingStatus, pbookingCat = pCategory, menuId = model.MenuId });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Server error";
                    return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });
                }
            }
        }

        public ActionResult BookingConfirmationInvoice1(int id, int menuId = 0, bool isPdf = false)
        {
            var data = (from b in ent.Bookings
                        join c in ent.CityMasters on b.City_Id equals c.Id
                        join r in ent.RentalTypes on b.RentalType equals r.Id
                        join vm in ent.VehicleModels on b.VehicleModel_Id equals vm.Id
                        where b.Id == id

                        select new BookingDTO
                        {
                            Id = b.Id,
                            CustomerName = b.CustomerName,
                            CityName = c.CityName,
                            PickupAddress = b.PickupAddress,
                            Email = b.Email,
                            ContactNo = b.ContactNo,
                            BookingDate = b.BookingDate,
                            CompanyName = b.CompanyName,
                            BookingId = b.BookingId,
                            PickupLandMark = b.PickupLandMark,
                            DropAddress = b.DropAddress,
                            DropLandmark = b.DropLandmark,
                            PickupDate = b.PickupDate,
                            PickupTime = b.PickupTime,
                            BookedBy = b.BookedBy,
                            CarModelName = vm.ModelName,
                            RentalTypeName = r.RentalTypeName,
                            ReportingTime = b.ReportingTime,
                            BookingConfirmFile = b.BookingConfirmFile,
                            BookingStatus = b.BookingStatus,
                            BookedByPerson = b.BookedByPerson,
                            BookingInstruction = b.BookingInstruction,
                            IsVIP = b.IsVIP
                        }).FirstOrDefault();
            ViewBag.menuId = menuId;
            ViewBag.isPdf = isPdf;
            return View(data);
        }

        //public ActionResult BookingConfirmationInvoice(int id)
        //{
        //    var data = (from b in ent.Bookings
        //                join c in ent.CityMasters on b.City_Id equals c.Id
        //                join r in ent.RentalTypes on b.RentalType equals r.Id
        //                join vm in ent.VehicleModels on b.VehicleModel_Id equals vm.Id
        //                where b.Id == id

        //                select new BookingDTO
        //                {
        //                    Id = b.Id,
        //                    CustomerName = b.CustomerName,
        //                    CityName = c.CityName,
        //                    PickupAddress = b.PickupAddress,
        //                    Email = b.Email,
        //                    ContactNo = b.ContactNo,
        //                    BookingDate = b.BookingDate,
        //                    CompanyName = b.CompanyName,
        //                    BookingId = b.BookingId,
        //                    PickupLandMark = b.PickupLandMark,
        //                    DropAddress = b.DropAddress,
        //                    DropLandmark = b.DropLandmark,
        //                    PickupDate = b.PickupDate,
        //                    PickupTime = b.PickupTime,
        //                    BookedBy = b.BookedBy,
        //                    CarModelName = vm.ModelName,
        //                    RentalTypeName = r.RentalTypeName,
        //                    ReportingTime = b.ReportingTime,
        //                    BookingConfirmFile = b.BookingConfirmFile,
        //                    BookingInstruction = b.BookingInstruction,
        //                    IsVIP = b.IsVIP
        //                }).FirstOrDefault();
        //    return View(data);
        //}

        public ActionResult GeneratePdfBookingInvoice(int id)
        {
            try
            {
                var booking = ent.Bookings.Find(id);
                if (string.IsNullOrEmpty(booking.BookingConfirmFile))
                {

                    var dt = DateTime.Now;
                    string filename = booking.BookingId + "-" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + ".pdf";
                    string path = Server.MapPath("/Files/") + filename;
                    booking.BookingConfirmFile = filename;
                    ent.SaveChanges();
                    return new ActionAsPdf("BookingConfirmationInvoice1", new { id = booking.Id, isPdf = true }) { FileName = filename, SaveOnServerPath = path };
                }
                return Content("already added");
            }
            catch (Exception ex)
            {
                return Content("error"+ex.Message);
            }
            //return View(data);
        }

        public ActionResult SendBookConfirmInvoice(int id, int menuId = 0)
        {
            try
            {
                var data = ent.Bookings.Find(id);
                var vehicle = ent.VehicleModels.Find(data.VehicleModel_Id);
                var path = Server.MapPath("~/Files/" + data.BookingConfirmFile);

                
                DateTime time = DateTime.Today.Add(data.PickupTime);
                string PickupTime = data.PickupTime.ToString();

                string msg = $@"Dear Sir / Madam,<br/><br/>

Greetings from VARDAAN Car Rental Services !!!! <br/><br/>

We are pleased to confirm the booking of {vehicle.ModelName} for Mr/Ms {data.CustomerName}. The booking ID is {data.BookingId} dated {data.PickupDate.ToString("dd-MMM-yyyy")} Time {PickupTime}.<br/>
Chauffeur and cab details will be sent prior 2 hours of pickup date and time.<br/><br/>

For any further clarifications, write to us or call us at 24x7 Helpline numbers, mentioned below.<br/>
LL: 0120 - 4204668; Mo: 08130874555; Email: reservation@vardaanrentacar.com
<br/><br/>
--
<br/><br/>
For any further information or clarification please feel free to get in touch with undersigned.
<br/><br/>
Thanks & Regards<br/>
VRC Team";
                EmailOperation.SendEmail(data.Email, "Vardaan Car Rental Booking Confirmation", msg, true, path);
                return Content("Invoice has sent.");
            }
            catch (Exception ex)
            {
                return Content("Server error.");
            }
        }

        //corporate performa invoice generate and send method
        public ActionResult GeneratePdfTaxInvoice(int id, bool IsTaxInvoice)
        {
            var cbill = ent.CorporateBills.Find(id);
            var booking = ent.Bookings.Find(cbill.Booking_Id);
            if (string.IsNullOrEmpty(cbill.BillFile))
            {
                var dt = DateTime.Now;

                string filename = booking.BookingId + "-" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + ".pdf";

                string path = Server.MapPath("/Files/") + filename;
                cbill.BillFile = filename;
                ent.SaveChanges();
               // return new ViewAsPdf("TaxInvoice", new { id = cbill.Id, IsTaxInvoice = IsTaxInvoice, isPdf = true }) { FileName = filename };
                return new ActionAsPdf("TaxInvoice", new { id = cbill.Id, IsTaxInvoice = IsTaxInvoice, isPdf = true }) { FileName = filename, SaveOnServerPath = path };
            }
            return Content("already added");
        }

        public ActionResult SendTaxInvoice(int id)
        {
            try
            {
                var data = ent.CorporateBills.Find(id);
                var client = ent.Customers.Find(data.Company_Id);
                var path = Server.MapPath("~/Files/" + data.BillFile);
                EmailOperation.SendEmail(client.Email, "Vardaan Car Rental Performa Invoice", "Please find the Attachements", true, path);
                return Content("Invoice has sent.");
            }
            catch (Exception ex)
            {
                return Content("Server error.");
            }
        }

        public ActionResult GeneratePdfVendorTaxInvoice(int id, bool IsTaxInvoice)
        {
            var vbill = ent.VendorBills.Find(id);
            if (string.IsNullOrEmpty(vbill.BillFile))
            {
                var dt = DateTime.Now;
                string filename = vbill.DsNo + "-" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + ".pdf";
                string path = Server.MapPath("/Files/") + filename;
                vbill.BillFile = filename;
                ent.SaveChanges();
                return new ActionAsPdf("VendorTaxInvoice", new { id = vbill.Id, IsTaxInvoice = IsTaxInvoice,IsPdf=true }) { FileName = filename, SaveOnServerPath = path };
            }
            return Content("already added");
        }

        public ActionResult SendVendorTaxInvoice(int id)
        {
            try
            {
                var data = ent.VendorBills.Find(id);
                var vendor = ent.Vendors.Find(data.Company_Id);
                var path = Server.MapPath("~/Files/" + data.BillFile);
                EmailOperation.SendEmail(vendor.Email, "Vardaan Car Rental Performa Invoice", "Please find the Attachements", true, path);
                return Content("Invoice has sent.");
            }
            catch (Exception ex)
            {
                return Content("Server error.");
            }
        }

        public ActionResult GetCityList(string term)
        {
            var model = new CitywithSatateDTO();

            var data = (from c in ent.CityMasters
                        join s in ent.StateMasters on c.StateMaster_Id equals s.Id
                        where c.CityName.ToLower().StartsWith(term.ToLower())
                        select new CitywithSatateDTO
                        {
                            Id = c.Id,
                            StateMaster_Id = c.StateMaster_Id,
                            StateName = s.StateName,
                            CityName = c.CityName
                        }
                        ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CloseBooking(DateTime? sDate, DateTime? eDate, int id, int page = 1, int menuId = 0, string term = "")
        {
            var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
            var model = new CorporateBillingDTO();
            model.DiscountTypes = new SelectList(cr.GetDiscountTypes(), "Text", "Value");
            model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "DisplayName");
            var booking = ent.Bookings.Find(id);
            var rentalType = ent.RentalTypes.Find(booking.RentalType);
            TempData["priviousURL"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            TempData.Keep("priviousURL");
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;
            ViewBag.bookingId = booking.BookingId;
           
            if (booking.BookingType == "nrd" && booking.NrgType == "walkin")
            {
                ViewBag.companyName = "NRD Invoice";
            }
            else
            {
                ViewBag.companyName = ent.Customers.Find(booking.Client_Id).CompanyName;
            }

            model.DesiredCar = ent.VehicleModels.Find(booking.VehicleModel_Id).ModelName;
            ///package type
            var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
            model.PackageType = packageType.PackageTypeName;
            var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
            ///package type

            model.JourneyStartDate = booking.PickupDate;
            // find vehicle model
            var cab = ent.Cabs.Find(booking.Cab_Id);
            ViewBag.CabNo = cab.VehicleNumber;
            var vehicleModel = ent.VehicleModels.Find(booking.VehicleModel_Id);
            double package8HourFare = 0;
            if (booking.BookingType == "nrd" && booking.NrgType == "walkin")
            {
                var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
                if (nrgPackage == null)
                    throw new Exception("package does not found");
                if (packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                {
                    var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.MinHrs == 8);
                    if (nrgPackage8Hour == null)
                        throw new Exception("package for 8 hour does not found");
                    package8HourFare = nrgPackage8Hour.Fare;
                }
                model.ExtraHrsRate = nrgPackage.ExtraPerHour;
                model.ExtraKmsRate = nrgPackage.ExtraPerKm;
                model.NightRate = nrgPackage.NightCharges;
                model.Fare = nrgPackage.Fare;
                model.Total = nrgPackage.Fare;
                model.MinHrs = nrgPackage.MinHrs;
                model.MinKm = nrgPackage.MinKm;
                model.OutStationChargeRate = nrgPackage.OutStationCharge;
            }
            else
            {
                int clientId = booking.Client_Id;
                var client = ent.Customers.Find(clientId);

                //find parent client 
                if (client.ParentCustomer_Id != null)
                {
                    clientId = client.ParentCustomer_Id ?? 0;
                }
                var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == vehicleModel.Id && a.RentalType_Id == booking.RentalType);
                if (package == null)
                    throw new Exception("package does not found");
                if (packageType.PType == "Local Run" && package.MinHrs == 4)
                {
                    var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == vehicleModel.Id && a.MinHrs == 8);
                    if (package8Hour == null)
                        throw new Exception("package for 8 hour does not found");
                    package8HourFare = package8Hour.Fare;
                }
                model.ExtraHrsRate = package.ExtraPerHour;
                model.ExtraKmsRate = package.ExtraPerKm;
                model.NightRate = package.NightCharges;
                model.Fare = package.Fare;
                model.Total = package.Fare;
                model.MinHrs = package.MinHrs;
                model.MinKm = package.MinKm;
                model.OutStationChargeRate = package.OutStationCharge;
            }
            if (packageArray.Contains(model.PackageType))
            {
                model.Fare = Math.Round(model.Fare / booking.DaysInMonth, 3);
                model.Total = model.Fare;
            }
            if (model.PackageType == "monthly")
            {
                model.MinKm = Math.Round(model.MinKm / booking.DaysInMonth, 3);
            }
            model.Package8HourFare = package8HourFare;
            model.Booking_Id = booking.Id;
            model.GuestName = booking.CustomerName;
            model.DutyAddress = booking.PickupAddress;
            model.Company_Id = booking.Client_Id;
            model.StartHour = (TimeSpan)booking.StartHour;
            model.StartKms = booking.StartKms;
            //model.EndKms = 2000;
            //model.EndHour= new TimeSpan(12, 00, 00);
            model.FuelEfficiency = cab.FuelEfficiency;
            model.InterStateCharge = 0; //package.InterStateCharge
            model.Term = term;
            model.Page = page;
            model.sDate = sDate;
            model.eDate = eDate;
            model.MenuId = menuId;
            //**************Previous CBill *************
            List<CorporateBill> cpbillPre=this.GetPreCorporateBillByBookingID(booking.Id).ToList();
            if (cpbillPre.Count>0)
            {
                model.StartKms = cpbillPre[0].StartKms;
                model.EndKms = cpbillPre[0].EndKms;
                model.JourneyStartDate = cpbillPre[0].JourneyStartDate;
                model.ExtraKms = cpbillPre[0].ExtraKms;
                model.ExtraKmsCharge = cpbillPre[0].ExtraKmsCharge;
                // model.JourneyClosingDate = cpbillPre[0].JourneyClosingDate;
                model.StartHour = cpbillPre[0].StartHour;
                model.EndHour = cpbillPre[0].EndHour;
                model.TotalNight = cpbillPre[0].TotalNight;
                model.MiscCharge = cpbillPre[0].MiscCharge;
                model.ParkCharge = cpbillPre[0].ParkCharge;
                model.TollCharge = cpbillPre[0].TollCharge;
                model.MCD = cpbillPre[0].MCD;
                model.InterStateCharge = cpbillPre[0].InterStateCharge;
                model.DiscountType = cpbillPre[0].DiscountType;
                model.DiscountValue = cpbillPre[0].DiscountValue;
                model.IncreaseDecreaseInFuel = cpbillPre[0].IncreaseDecreaseInFuel;
                model.NetAmount = cpbillPre[0].NetAmount;
                model.VisitedPlace = cpbillPre[0].VisitedPlace;
                model.Total = cpbillPre[0].NetAmount-Convert.ToDouble(cpbillPre[0].DiscountValue);
                var venBill= this.GetPreVendorBillByBookingID(booking.Id).ToList()[0];              
                model.VendorCommision = venBill.VendorCommision;

                model.TotalKms= cpbillPre[0].TotalKms;
                model.TotalHrs= cpbillPre[0].TotalHrs;
                model.OutStationDays= cpbillPre[0].OutStationDays;
                model.OutStationCharge = cpbillPre[0].OutStationCharge;
                model.OutStationDays= cpbillPre[0].OutStationDays;
                model.StateGstin_Id = cpbillPre[0].StateGstin_Id;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CloseBooking(CorporateBillingDTO model)
        {
            int pType = 0;
            string previousUrl = string.Empty;
            string bookingType= string.Empty;
            if (TempData["priviousURL"] != null)
            {
                previousUrl = Convert.ToString(TempData["priviousURL"]);
            }
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ent.CorporateBills.Any(x => x.Booking_Id == model.Booking_Id && x.IsCancelled == false))
                    {
                        var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
                        model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
                        model.DiscountTypes = new SelectList(cr.GetDiscountTypes(), "Text", "Value");
                        if (!ModelState.IsValid)
                        {
                            return View(model);
                        }
                        var booking = ent.Bookings.Find(model.Booking_Id);
                        bookingType = booking.BookingType;
                        if (booking == null)
                            throw new FileNotFoundException("This booking has deleted.");
                        model.DesiredCar = ent.VehicleModels.Find(booking.VehicleModel_Id).ModelName;
                        int clientId = booking.Client_Id;
                        var client = ent.Customers.Find(clientId);
                        //if(client==null)
                        //    throw new FileNotFoundException("This client has deleted.");
                        var rentalType = ent.RentalTypes.Find(booking.RentalType);
                        if (rentalType == null)
                            throw new FileNotFoundException("This rental type has deleted.");


                        ///package type
                        pType = rentalType.PackageType_Id;
                        var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
                        if (packageType == null)
                            throw new FileNotFoundException("This package type has deleted.");
                        model.PackageType = packageType.PackageTypeName;
                        var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
                        ///package type

                        var corporateBill = Mapper.Map<CorporateBill>(model);
                        var user = ent.UserLogins.Find(cr.GetUserDetailId());
                        #region corporate billing 
                        if (booking.BookingType == "nrd" && booking.NrgType == "walkin")
                        {
                            var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
                            if (nrgPackage == null)
                                throw new FileNotFoundException("nrd package does not found");
                            if (packageType.PType == "Local Run" && (model.MinHrs == 8 || model.MinKm == 80) && nrgPackage.MinHrs == 4)
                            {
                                var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.MinHrs == 8);
                                if (nrgPackage8Hour == null)
                                    throw new FileNotFoundException("nrd package for 8 hour does not found");
                                booking.RentalType = nrgPackage8Hour.RentalType_Id;
                                ent.SaveChanges();
                            }
                        }
                        else
                        {

                            //find parent client 
                            if (client.ParentCustomer_Id != null)
                            {
                                clientId = client.ParentCustomer_Id ?? 0;
                            }
                            var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
                            if (package == null)
                                throw new FileNotFoundException("package does not found");
                            if (packageType.PType == "Local Run" && (model.MinHrs == 8 || model.MinKm == 80) && package.MinHrs == 4)
                            {
                                ClientPackage package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == booking.VehicleModel_Id && a.MinHrs == 8);
                                if (package8Hour == null)
                                    throw new FileNotFoundException("package for 8 hour does not found");
                                booking.RentalType = package8Hour.RentalType_Id;
                                ent.SaveChanges();
                            }
                        }

                        corporateBill.UpdatedBy = user.Id;
                        corporateBill.UpdateDate = cr.GetISTDate();
                        corporateBill.DsDate = cr.GetISTDate();
                        corporateBill.IsCancelled = false;
                        var taxInPercent = ent.Taxes.FirstOrDefault().Amount;
                        double taxableAmount = model.Total;
                        //double taxableAmount = model.Total - (model.TollCharge + model.InterStateCharge);
                        double tax = Math.Round((taxableAmount * taxInPercent) / 100, 2);
                        corporateBill.Total += tax;
                        int reglarStateId = 0;
                        if (booking.BookingType == "regular")
                        {
                            if (ent.StateMasters.Any(x => x.StateCode == client.GSTIN.Substring(0, 2)))
                            {
                                reglarStateId = ent.StateMasters.Where(x => x.StateCode == client.GSTIN.Substring(0, 2)).FirstOrDefault().Id;
                            }
                            else
                            {
                                reglarStateId = client.State_Id;
                            }
                        }
                        int stateId = booking.BookingType == "regular" ? reglarStateId : (int)booking.NrdStateId;
                        // if (stateId == model.StateGstin_Id) updated by bhupesh
                        if (stateId == model.StateGstin_Id)
                        {
                            corporateBill.CgstPercent = Math.Round(taxInPercent / 2, 2);
                            corporateBill.SgstPercent = Math.Round(taxInPercent / 2, 2);
                            corporateBill.SGST = Math.Round(tax / 2, 2);
                            corporateBill.CGST = Math.Round(tax / 2, 2);
                        }
                        else
                        {
                            corporateBill.IgstPercent = Math.Round(taxInPercent, 2);
                            corporateBill.IGST = Math.Round(tax, 2);
                        }
                        if (booking.BookingType == "regular")
                        {
                            corporateBill.IsNrg = false;
                            var dsNo = "VRC100";
                            var lastRecord = ent.CorporateBills.OrderByDescending(a => a.Id).FirstOrDefault(a => a.IsNrg == false);
                            if (lastRecord != null)
                            {
                                var len = lastRecord.DsNo.Length;
                                var iPart = lastRecord.DsNo.Substring(3, len - 3);
                                var next = int.Parse(iPart) + 1;
                                dsNo = "VRC" + next;
                            }
                            corporateBill.DsNo = dsNo;

                        }
                        else
                        {
                            corporateBill.IsNrg = true;
                            var dsNo = "NRD-1";
                            var lastRecord = ent.CorporateBills.OrderByDescending(a => a.Id).FirstOrDefault(a => a.IsNrg == true);
                            if (lastRecord != null)
                            {
                                var len = lastRecord.DsNo.Length;
                                var iPart = lastRecord.DsNo.Substring(4, len - 4);
                                var next = int.Parse(iPart) + 1;
                                dsNo = "NRD-" + next;
                            }
                            corporateBill.DsNo = dsNo;
                        }
                        if (model.MinKm == 0)
                        {
                            corporateBill.ExtraKms = 0;
                        }
                        if (model.MinHrs == 0)
                        {
                            corporateBill.ExtraHrs = 0;
                        }
                        //by bhupesh
                        corporateBill.ProformNo = this.getAutoProformaeNo();
                        ent.CorporateBills.Add(corporateBill);
                        ent.SaveChanges();
                        var driver = ent.Drivers.Find(booking.DriverId);
                        //driver.IsAvailable = true;
                        var cab = ent.Cabs.Find(booking.Cab_Id);
                        // cab.IsAvailable = true;
                        booking.BookingStatus = (int)BookingStatus.Completed;
                        booking.ClosedBy = user.Email;
                        booking.ClosingDate = cr.GetISTDate();
                        ent.SaveChanges();
                        #endregion
                        //vendor billing section start

                        #region vendor billing

                        var vendor = ent.Vendors.Find(cab.Vendor_Id);
                        if (vendor == null)
                            throw new FileNotFoundException("This cab does not have any vendor.");
                        //find vendor package
                        var vendor_package = ent.VendorPersonalPackages.FirstOrDefault(a => a.Vendor_Id == vendor.Id && a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
                        if (model.CommisionType != "Percent" && vendor_package == null)
                            throw new FileNotFoundException("Vendor personal package does not found");
                        var vBill = Mapper.Map<VendorBill>(model);
                        vBill.Company_Id = vendor.Id;
                        vBill.UpdatedBy = user.Id;
                        vBill.UpdateDate = cr.GetISTDate();
                        vBill.DsDate = cr.GetISTDate();

                        // calculate fare and TA for outstation package
                        if (model.PackageType == "outstation")
                        {
                            //vBill.Fare = vendor_package.Fare * model.OutStationDays;
                            //vBill.OutStationDays = model.OutStationDays;
                            //if (model.CommisionType == "Percent")
                            //    vBill.OutStationChargeRate = (model.OutStationChargeRate * (100 - model.VendorCommision)) / 100;
                            //else
                            //    vBill.OutStationChargeRate = vendor_package.OutStationCharge;
                            //vBill.OutStationCharge = vBill.OutStationChargeRate * model.OutStationDays;

                            double fre = 0;
                            double otChrgeRate = 0;
                            if (model.CommisionType == "Percent")
                            {
                                fre = model.Fare * (100 - model.VendorCommision) / 100;
                                // otChrgeRate = (model.OutStationChargeRate * (100 - model.VendorCommision)) / 100;
                            }
                            else
                            {
                                fre = model.Fare;
                                //otChrgeRate = model.OutStationChargeRate;
                            }
                            otChrgeRate = model.OutStationChargeRate;
                            vBill.Fare = fre;
                            vBill.OutStationDays = model.OutStationDays;
                            vBill.OutStationCharge = otChrgeRate * model.OutStationDays;

                        }
                        else
                        {
                            if (model.CommisionType == "Percent")
                            {
                                vBill.Fare = model.Fare * (100 - model.VendorCommision) / 100;
                            }
                            else
                            {
                                if (packageArray.Contains(model.PackageType))
                                {
                                    vBill.Fare = Math.Round(vendor_package.Fare / booking.DaysInMonth, 2);
                                }
                                else
                                {
                                    vBill.Fare = vendor_package.Fare;
                                }
                            }
                        }

                        //if (model.PackageType == "monthly" || model.PackageType == "monthly-route" || model.PackageType == "monthly-trip")
                        //{
                        //    vBill.Fare = 0;
                        //}
                        if (model.CommisionType != "Percent")
                        {
                            vBill.ExtraHrsRate = vendor_package.ExtraPerHour;
                            vBill.ExtraKmsRate = vendor_package.ExtraPerKm;
                            vBill.NightRate = vendor_package.NightCharges;

                            vBill.MinHrs = vendor_package.MinHrs;
                            if (model.PackageType == "monthly")
                            {
                                vBill.MinKm = vendor_package.MinKm / booking.DaysInMonth;
                            }
                            else
                            {
                                vBill.MinKm = vendor_package.MinKm;
                            }
                        }


                        //if (vBill.ExtraKms > 0)
                        //{
                        //    if (model.CommisionType == "Percent")
                        //    {
                        //        vBill.ExtraKmsRate = Convert.ToInt32(model.ExtraKmsRate * (100 - model.VendorCommision) / 100);
                        //    }
                        //    vBill.ExtraKmsCharge = vBill.ExtraKms * vBill.ExtraKmsRate;
                        //}
                        //updated by bhupesh
                        if (vBill.ExtraKms > 0)
                        {
                            double extraKmsChargeB = vBill.ExtraKms * model.ExtraKmsRate;
                            if (model.CommisionType == "Percent")
                            {
                                vBill.ExtraKmsCharge = Convert.ToDouble(extraKmsChargeB * (100 - model.VendorCommision) / 100);
                            }
                            vBill.ExtraKmsRate = model.ExtraKmsRate;
                        }
                        //**End
                        //if (vBill.ExtraHrs > 0)
                        //{
                        //    if (model.CommisionType == "Percent")
                        //    {
                        //        vBill.ExtraHrsRate = Convert.ToInt32(model.ExtraHrsRate * (100 - model.VendorCommision) / 100);
                        //    }
                        //    vBill.ExtraHrsCharge = vBill.ExtraHrs * vBill.ExtraHrsRate;
                        //}
                        //updated by bhupesh
                        if (vBill.ExtraHrs > 0)
                        {
                            double extraHrsChargeB = vBill.ExtraHrs * model.ExtraHrsRate;
                            if (model.CommisionType == "Percent")
                            {
                                vBill.ExtraHrsCharge = Convert.ToDouble(extraHrsChargeB * (100 - model.VendorCommision) / 100);
                            }
                            vBill.ExtraHrsRate = model.ExtraHrsRate;
                        }
                        //***End

                        if (vBill.TotalNight > 0)
                        {
                            vBill.NightCharge = vBill.TotalNight * vBill.NightRate;
                        }

                        //total amount calculation
                        //if (model.CommisionType == "Percent")
                        //{
                        //    double fareWithCommision = vBill.Fare * (100 - model.VendorCommision) / 100;
                        //    vBill.Fare = fareWithCommision;
                        //}

                        double taxableVendorAmount = vBill.Fare + vBill.ExtraKmsCharge +
                            vBill.ExtraHrsCharge + vBill.NightCharge + vBill.StateCharge
                            + vBill.ParkCharge + vBill.FuelCharge + vBill.OutStationCharge
                          + vBill.MCD + vBill.TollCharge + vBill.InterStateCharge + vBill.MiscCharge;

                        vBill.NetAmount = taxableVendorAmount;
                        double discountAmt = 0;
                        if (model.DiscountAmount > 0)
                        {
                            if (model.CommisionType == "Percent")
                            {
                                discountAmt = Convert.ToDouble(model.DiscountAmount * (100 - model.VendorCommision) / 100);
                            }

                        }
                        vBill.DiscountAmount = discountAmt;
                        double gst = Math.Round(((taxableVendorAmount - discountAmt) * taxInPercent) / 100, 2);
                        //vBill.Total = taxableVendorAmount + vBill.TollCharge + vBill.InterStateCharge;
                        vBill.Total = taxableVendorAmount - discountAmt + gst;
                        if (vendor.StateMaster_Id == model.StateGstin_Id)
                        {
                            vBill.CgstPercent = Math.Round(taxInPercent / 2, 2);
                            vBill.SgstPercent = Math.Round(taxInPercent / 2, 2);
                            vBill.SGST = Math.Round(gst / 2, 2);
                            vBill.CGST = Math.Round(gst / 2, 2);
                        }
                        else
                        {
                            vBill.IgstPercent = Math.Round(taxInPercent, 2);
                            vBill.IGST = Math.Round(gst, 2);
                        }
                        // vBill.Total += gst;
                        var vdsNo = "VRCV100";
                        var lRecord = ent.VendorBills.OrderByDescending(a => a.Id).FirstOrDefault();
                        if (lRecord != null)
                        {
                            var len = lRecord.DsNo.Length;
                            var iPart = lRecord.DsNo.Substring(4, len - 4);
                            var next = int.Parse(iPart) + 1;
                            vdsNo = "VRCV" + next;
                        }
                        vBill.DsNo = vdsNo;
                        vBill.VisitedPlace = model.VisitedPlace;
                        if (model.MinKm == 0)
                        {
                            vBill.ExtraKms = 0;
                        }
                        if (model.MinHrs == 0)
                        {
                            vBill.ExtraHrs = 0;
                        }
                        ent.VendorBills.Add(vBill);
                        ent.SaveChanges();
                        //vendor billing end
                        var log = new Log
                        {
                            Booking_Id = booking.Id,
                            UpdateDate = cr.GetISTDate(),
                            UserLogin_Id = user.Id,
                            Description = "Booking with Id " + booking.BookingId + " and Ds No.  " + corporateBill.DsNo + " is closed by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                        };
                        ent.Logs.Add(log);
                        ent.SaveChanges();
                        #endregion
                        tran.Commit();
                    }
                    var urlArray = previousUrl.Split(new Char[] { '/', '?' });
                    string returnURL = String.Empty;
                    foreach (var item in urlArray)
                    {
                        if(item== "UnbilledBookingBypType")
                        {
                            returnURL = "UnbilledBookingBypType";
                            break;
                        }
                    }
                    if (returnURL== "UnbilledBookingBypType")
                    {
                        if (bookingType == "regular")
                        {
                            //if(pType == 1||pType==2||pType==3)
                            //{
                            //    return RedirectToAction("UnbilledBookingBypType", new { page = model.Page, term = model.Term, pType = 2, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            //}
                            //else
                            //{ 
                            return RedirectToAction("UnbilledBookingBypType", new { page = model.Page, term = model.Term, pType = pType, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            //}
                        }
                        else
                        {
                            return RedirectToAction("UnbilledBookingBypType", new { page = model.Page, term = model.Term, pType = 0, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                        }
                    }
                    else 
                    {
                        return RedirectToAction("UnbilledBooking", new { page = model.Page, term = model.Term, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                    }
                    //pType=0&bookingType=nrd
                    


                        
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = ex.Message;
                    return View(model);
                    //return RedirectToAction("CloseBooking", new { id = model.Booking_Id, page = model.Page, menuId = model.MenuId, term = model.Term });
                }
            }
        }

        public ActionResult GetDays(DateTime startDays, DateTime endDays)
        {
            int diff = (endDays.Date - startDays.Date).Days + 1;
            return Json(diff <= 0 ? 1 : diff, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRemainingHour(RemainingHourModel model)
        {
            var sDate = new DateTime(model.JourneyStartDate.Year, model.JourneyStartDate.Month, model.JourneyStartDate.Day, model.JourneyStartTime.Hours, model.JourneyStartTime.Minutes, model.JourneyStartTime.Seconds);
            var eDate = new DateTime(model.JourneyEndDate.Year, model.JourneyEndDate.Month, model.JourneyEndDate.Day, model.JourneyEndTime.Hours, model.JourneyEndTime.Minutes, model.JourneyEndTime.Seconds);
            var diff = (eDate - sDate);
            decimal totalHours = Convert.ToDecimal(diff.TotalHours);
            var total = decimal.Round(totalHours, 3);
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        //        public ActionResult CorporateInvoiceList(DateTime? sDate, DateTime? eDate, string isNrg = "false", string bookingId = "", string billNo = "", string companyName = "", string mobile = "", string cabNo="", int page = 1, int menuId = 0)
        //        {
        //            int pos = isNrg.IndexOf('?');
        //            if (pos >= 0)
        //            {
        //                isNrg = isNrg.Remove(pos, 1);
        //            }
        //            bool nrg = Convert.ToBoolean(isNrg);

        //            int nrgType = 0;
        //            if(nrg)
        //            {
        //                nrgType = 1;
        //            }
        //            var model = new CIBillingListVm();

        //            string query = @"Select cb.Id,b.BookedBy,b.BookingId,rt.RentalTypeName as BookingType,vh.ModelName as CabAlloted,
        //c.VehicleNumber as CabNo, city.CityName as City,cb.ParkCharge as ParkingChage,cb.TollCharge,cb.StateCharge,
        //cb.CGST,cb.SGST,cb.IGST,cb.EndHour as ClosingTime, cb.StartHour as StartTime, cb.JourneyStartDate as JourneyDate,
        //cb.JourneyClosingDate as JourneyEndDate,b.ClosingDate, b.ClosedBy,cl.CompanyName as Company,cb.ExtraHrs as ExtraHours,
        //cb.Total as TotalAmount,cb.ExtraKms as ExtraKm,cb.Fare,cb.DsDate as InvoiceDate,cb.DsNo as InvoiceNo,cb.NightCharge as NightCharges,
        //(select case when cb.IsCancelled=1 then 'Cancelled' else 'ok' end) as [Status],cb.TotalHrs as TotalHours,cb.TotalKms as TotalKm,
        //cb.Booking_Id as CabBooking_Id,cb.IsCancelled,(select case when cb.IsCancelled=1 then 'red' else 'green' end) as Color,
        //cb.DiscountType,cb.DiscountValue,cb.DiscountAmount,cb.NetAmount,b.ContactNo as Mobile,b.NrgType,b.NrdBookingMode as PaymentType,
        //pType.PType as PackageTypeName,cb.GuestName,cb.TaxInvoiceDate,cb.TaxInvoiceNumber,cb.TotalNight as TotalNights,cb.StartKms as StartKm,
        //cb.EndKms as EndKm,cb.MCD,[dbo].[getUpdateDescription](cb.Booking_Id) as UpdateDescription from CorporateBill cb
        //                        join Booking b on cb.Booking_Id = b.Id
        //                        join Cab c on b.Cab_Id = c.Id
        //                        join  RentalType rt on b.RentalType = rt.Id
        //                        join  VehicleModel vh on c.VehicleModel_Id = vh.Id
        //                        join  CityMaster city on b.City_Id = city.Id
        //                        left join  Customer cl on b.Client_Id = cl.Id 
        //                        left join PackageType pType on b.PackageType_Id = pType.Id
        //                        where cb.IsNrg =" + nrgType + " and cb.IsCancelled=0 order by cb.Id desc";

        //            var data = ent.Database.SqlQuery<CIData>(query).ToList();


        //            if (sDate != null && eDate != null)
        //            {
        //                data = data.Where(a => a.JourneyDate.Date >= sDate.Value.Date && a.JourneyDate.Date <= eDate.Value.Date).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(bookingId))
        //            {
        //                data = data.Where(a => a.BookingId.ToLower().Contains(bookingId.ToLower().Trim())).ToList();
        //            }

        //            if (!string.IsNullOrEmpty(cabNo))
        //            {
        //                data = data.Where(a => a.CabNo.ToLower().Contains(cabNo.ToLower().Trim())).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(billNo))
        //            {
        //                data = data.Where(a => a.InvoiceNo.ToLower().Contains(billNo.ToLower().Trim())).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(companyName))
        //            {
        //               // data = data.Where(a => !string.IsNullOrEmpty(a.Company)).ToList().Where(a=> a.Company.ToLower().Contains(companyName.ToLower().Trim())).ToList();
        //                data = data.Where(a=> !string.IsNullOrEmpty(a.Company) && a.Company.ToLower().Contains(companyName.ToLower().Trim())).ToList();
        //            }

        //            if (!string.IsNullOrEmpty(mobile))
        //            {
        //                data = data.Where(a => a.Mobile.StartsWith(mobile.Trim())).ToList();
        //            }
        //            // pagination

        //            int total = data.Count;
        //            int pageSize = 100;
        //            double totalPages = Math.Ceiling(total / (double)pageSize);
        //            int skip = pageSize * (page - 1);
        //            data = data.Skip(skip).Take(pageSize).ToList();
        //            model.Bills = data;
        //            model.sDate = sDate;
        //            model.eDate = eDate;
        //            model.BookingId = bookingId;
        //            model.InvoiceNo = billNo;
        //            model.CompanyName = companyName;
        //            model.Page = page;
        //            model.NumberOfPages = (int)totalPages;
        //            model.IsNrg = nrg;
        //            model.Mobile = mobile;
        //            model.CabNo = cabNo;
        //            ViewBag.menuId = menuId;
        //            return View(model);
        //        }


        public ActionResult CorporateInvoiceList(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", int page = 1, string routeNo = "", int menuId = 0, bool export = false, string Display = "d")
        {
            int pageSize =50;
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;
            string usern = Convert.ToString(Session["uEmail"]);
            string sesid = Convert.ToString(Session.SessionID);
            List<tempUserBooking> lst = new List<tempUserBooking>();
            if (ent.tempUserBookings.Any(x => x.Username == usern && x.SessionId == sesid))
            {
                lst = ent.tempUserBookings.Where(x => x.Username == usern && x.SessionId == sesid).ToList();
                ViewBag.bookinglst = lst;
            }
            string cityId = string.Empty;
            string routeno = String.Empty;
            routeno = String.IsNullOrEmpty(routeNo) ? "0" : routeNo;
            var model = new CIBillingListVm();
            if (Request["City_Id"] != null)
            {
                cityId = Request["City_Id"].ToString();              
            }
            else 
            {
                cityId = model.cityid==0?"": model.cityid.ToString();
            }           
            
            int pos = isNrg.IndexOf('?');
            if (pos >= 0)
            {
                isNrg = isNrg.Remove(pos, 1);
            }
            bool nrg = Convert.ToBoolean(isNrg);

            int nrgType = 0;
            if (nrg)
            {
                nrgType = 1;
            }
          
            model.Display = Display;

            bool dispalyAll = Display == "d" ? export : true;
            string query = @"execute GetCorporateDsNew @isNrg,@term,@sDate,@eDate,@page,@pageSize,@isExport,@cityId,@routeNo";
            var data = ent.Database.SqlQuery<CIData>(query,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", dispalyAll),
                new SqlParameter("@cityId",String.IsNullOrEmpty(cityId)? (object)DBNull.Value:Convert.ToInt32(cityId)),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).ToList();

            // for getting all records,so that we can do pagination
            string query2 = @"execute GetCorporateDsCountNew @isNrg,@term,@sDate,@eDate,@page,@pageSize,@isExport,@cityId,@routeNo";
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true),
                new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).FirstOrDefault();

            // var total = data2.Count;
            if (export)
            {
                var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
                var ed = (from d in data
                          select new DsExcelModel
                          {
                              SlNo = data.IndexOf(d) + 1,
                              BookedBy = d.BookedBy,
                              JourneyEndDate = d.JourneyEndDate.ToString("dd/MM/yyyy"),
                              BookingId = d.BookingId,
                              NetAmount = d.NetAmount - d.DiscountAmount,
                              StartKm = d.StartKm,
                              NightCharges = d.NightCharges,
                              StateTax = d.InterStateCharge,
                              StartTime = d.StartTime,
                              BookingType = d.BookingType,
                              PackageType = d.PackageTypeName,
                              EndKm = d.EndKm,
                              Fare = d.Fare,
                              CabBooked = d.CabBooked,
                              City = d.City,
                              ClosingDate = d.ClosingDate,
                              ClosingTime = d.ClosingTime,
                              JourneyStartDate = d.JourneyDate.ToString("dd/MM/yyyy"),
                              CabNo = d.CabNo,
                              CGST =isNrg== "True" ? 0.0: d.CGST,
                              SGST = isNrg == "True" ? 0.0 : d.SGST,
                              IGST = isNrg == "True" ? 0.0 : d.IGST,
                              ParkingChage = d.ParkingChage,
                              TotalAmount = isNrg == "True" ? Math.Round(d.TotalAmount - (d.CGST + d.SGST + d.IGST)) : packageArray.Contains(d.PackageTypeName) ? d.TotalAmount : Math.Round(d.TotalAmount, MidpointRounding.AwayFromZero),
                              MCD = d.MCD,
                              GuestName = d.GuestName,
                              Mobile = d.Mobile,
                              Organization = d.Company,
                              TollTax = d.TollCharge,
                              TotalHours = d.TotalHours,
                              TotalKm = d.TotalKm,
                              TotalNights = d.TotalNights,
                              ExtraKm = d.ExtraKm,
                              ExtraKmsRate = d.ExtraKmsRate,
                              ExtraKmsCharge = d.ExtraKmsCharge,
                              ExtraHours = d.ExtraHours,
                              ExtraHourCharge = d.ExtraHrsCharge,
                              ExtraHourRate = d.ExtraHrsRate,
                              TACharge = d.OutStationCharge,
                              MiscellaneousCharge = d.MiscCharge,
                              DiscountAmount = d.DiscountAmount,
                              SubTotal = d.NetAmount,
                              RouteNo=d.RouteNo
                          }
                        ).ToList();
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =corporateDutySlips.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            double totalPages = Display == "a" ? 1 : Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            // data = data.Skip(skip).Take(pageSize).ToList();

            //if (Request["City_Id"] != null)
            //{
            //    cityId = Request["City_Id"].ToString();
            //    if(!String.IsNullOrEmpty(cityId))
            //    {
            //        data = data.Where(x => x.City == cityId).ToList();
            //    }
            //}
            model.SrNo = skip;
            model.Bills = data;
            model.sDate = sDate;
            model.eDate = eDate;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.IsNrg = nrg;
           
            model.Cities = new SelectList(ent.CityMasters.OrderBy(a=>a.CityName).ToList(), "Id", "CityName", cityId);
            model.cityid = String.IsNullOrEmpty(cityId) ? 0 : Convert.ToInt32(cityId);
            model.routeno = String.IsNullOrEmpty(routeno) ? 0 : Convert.ToInt32(routeno); 
            ViewBag.menuId = menuId;
            ViewBag.total = total;
            return View(model);
        }

        public ActionResult CorporateInvoiceListEditInv(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", int page = 1, int menuId = 0, int TinvID = 0, string Display = "d")
        {
            int pageSize = 40;

            int pos = isNrg.IndexOf('?');
            if (pos >= 0)
            {
                isNrg = isNrg.Remove(pos, 1);
            }
            bool nrg = Convert.ToBoolean(isNrg);           
            var model = new CIBillingListVmEdit();
            model.Display = Display;
            string query = @"execute GetCorporateDsEdit @isNrg,@term,@sDate,@eDate,@page,@pageSize,@TInvID";
            var data = ent.Database.SqlQuery<CIDataEdit>(query,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@TInvID", TinvID)
                ).ToList();
            var taxInvObj= ent.CorporateInvoiceFiles.Where(x => x.Id == TinvID).FirstOrDefault();
            model.TaxInvoiceNumber = taxInvObj.InvoiceNumber;
            model.TaxInvoiceDate = taxInvObj.InvoiceDate.ToString("dd/MM/yyyy");
            // for getting all records,so that we can do pagination
            var total = 100;
            double totalPages = Display == "a" ? 1 : Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            // data = data.Skip(skip).Take(pageSize).ToList();
            model.SrNo = skip;
            model.Bills = data;
            model.sDate = sDate;
            model.eDate = eDate;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.IsNrg = nrg;
            model.taxInvID = TinvID;
            ViewBag.menuId = menuId;
            return View(model);
        }
        public ActionResult CorporateInvoiceListBypType(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "",  int page = 1, int pType = 0, string bookingType = "", string routeNo = "", int menuId = 0, bool export = false, string Display = "d")
        {
            int pageSize = 40;
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;
            string cityId = string.Empty;
            string routeno = String.Empty;
            routeno = String.IsNullOrEmpty(routeNo) ? "0" : routeNo;
            int pos = isNrg.IndexOf('?');
            if (pos >= 0)
            {
                isNrg = isNrg.Remove(pos, 1);
            }
            bool nrg = Convert.ToBoolean(isNrg);

            int nrgType = 0;
            if (nrg)
            {
                nrgType = 1;
            }
            var model = new CIBillingListVm();
            if (Request["City_Id"] != null)
            {
                cityId = Request["City_Id"].ToString();
            }
            else
            {
                cityId = model.cityid == 0 ? "" : model.cityid.ToString();
            }
            
            model.Display = Display;

            bool dispalyAll = Display == "d" ? export : true;
            string query = @"execute GetCorporateDsByPType @isNrg,@term,@sDate,@eDate,@page,@pageSize,@pType,@bType,@isExport,@cityId,@routeNo";
            var data = ent.Database.SqlQuery<CIData>(query,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                 new SqlParameter("@pType", pType),
                new SqlParameter("@bType", bookingType),
                new SqlParameter("@isExport", dispalyAll),
                new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).ToList();

            //for getting all records, so that we can do pagination
            string query2 = @"execute GetCorporateDsCountByPType @isNrg,@term,@sDate,@eDate,@page,@pageSize,@pType,@bType,@isExport,@cityId,@routeNo";
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                 new SqlParameter("@pType", pType),
                new SqlParameter("@bType", bookingType),
                new SqlParameter("@isExport", true),
                new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno))
                ).FirstOrDefault();

            //var total = data.Count;
         
            if (pType == 0)
            { ViewBag.PackageType = "NRD";
                ViewBag.PackageTypeID = 0;
            }
            else
            {
                ViewBag.PackageType = ent.PackageTypes.Where(x => x.Id == pType).FirstOrDefault().PType;
                ViewBag.PackageTypeID = pType;
            }
            if (export)
            {
                var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
                var ed = (from d in data
                          select new DsExcelModel
                          {
                              SlNo = data.IndexOf(d) + 1,
                              BookedBy = d.BookedBy,
                              JourneyEndDate = d.JourneyEndDate.ToString("dd/MM/yyyy"),
                              BookingId = d.BookingId,
                              NetAmount = d.NetAmount - d.DiscountAmount,
                              StartKm = d.StartKm,
                              NightCharges = d.NightCharges,
                              StateTax = d.InterStateCharge,
                              StartTime = d.StartTime,
                              BookingType = d.BookingType,
                              PackageType = d.PackageTypeName,
                              EndKm = d.EndKm,
                              Fare = d.Fare,
                              CabBooked = d.CabBooked,
                              City = d.City,
                              ClosingDate = d.ClosingDate,
                              ClosingTime = d.ClosingTime,
                              JourneyStartDate = d.JourneyDate.ToString("dd/MM/yyyy"),
                              CabNo = d.CabNo,
                              CGST = isNrg == "True" ? 0.0 : d.CGST,
                              SGST = isNrg == "True" ? 0.0 : d.SGST,
                              IGST = isNrg == "True" ? 0.0 : d.IGST,
                              ParkingChage = d.ParkingChage,
                              TotalAmount = isNrg == "True" ? d.TotalAmount- (d.CGST+ d.SGST+ d.IGST) : packageArray.Contains(d.PackageTypeName) ? d.TotalAmount : Math.Round(d.TotalAmount, MidpointRounding.AwayFromZero),
                              MCD = d.MCD,
                              GuestName = d.GuestName,
                              Mobile = d.Mobile,
                              Organization = d.Company,
                              TollTax = d.TollCharge,
                              TotalHours = d.TotalHours,
                              TotalKm = d.TotalKm,
                              TotalNights = d.TotalNights,
                              ExtraKm = d.ExtraKm,
                              ExtraKmsRate = d.ExtraKmsRate,
                              ExtraKmsCharge = d.ExtraKmsCharge,
                              ExtraHours = d.ExtraHours,
                              ExtraHourCharge = d.ExtraHrsCharge,
                              ExtraHourRate = d.ExtraHrsRate,
                              TACharge = d.OutStationCharge,
                              MiscellaneousCharge = d.MiscCharge,
                              DiscountAmount = d.DiscountAmount,
                              SubTotal = d.NetAmount,
                              RouteNo=d.RouteNo
                          }
                        ).ToList();
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =corporateDutySlips.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            double totalPages = Display == "a" ? 1 : Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            // data = data.Skip(skip).Take(pageSize).ToList();
            //if (Request["City_Id"] != null)
            //{                
            //    cityId = Request["City_Id"].ToString();
            //    if (!String.IsNullOrEmpty(cityId))
            //    {
            //        data = data.Where(x => x.City == cityId).ToList();
            //    }
            //}
            model.SrNo = skip;
            model.Bills = data;
            model.sDate = sDate;
            model.eDate = eDate;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.IsNrg = nrg;
            model.Cities = new SelectList(ent.CityMasters.ToList(), "Id", "CityName");
            ViewBag.menuId = menuId;
            ViewBag.total = total;
            return View(model);
        }

        public ActionResult CancelBooking(int bookingId, int? pDriver_Id, int? pCab_Id, int bStatusId, int menuId = 0)
        {
            //var model = new CancelBookingDTO();
            var data = ent.Bookings.Find(bookingId);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var userId = cr.GetUserDetailId();
                    var user = ent.UserLogins.FirstOrDefault(a => a.Id == userId);
                    if (data != null)
                    {
                        //update driver id and cab id

                        data.UpdatedBy = user.Id;
                        data.UpdateDate = cr.GetISTDate();
                        data.BookingStatus = bStatusId;
                        ent.SaveChanges();

                        //send sms to customer with car & driver details.
                        if (pDriver_Id != null && pCab_Id != null)
                        {
                            var pvehicle = ent.Cabs.Find(pCab_Id);
                            var pdriver = ent.Drivers.Find(pDriver_Id);
                            pdriver.IsAvailable = true;
                            ent.SaveChanges();
                            pvehicle.IsAvailable = true;
                            ent.SaveChanges();
                        }
                        var log = new Log
                        {
                            Booking_Id = data.Id,
                            UpdateDate = cr.GetISTDate(),
                            UserLogin_Id = user.Id,
                            Description = "Booking with Id " + data.BookingId + " is Cancelled by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                        };
                        ent.Logs.Add(log);
                        ent.SaveChanges();
                    }
                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("Server error");
                }
            }
        }

        public TaxInvoiceViewModel GetTaxInvoiceData(int id, bool IsTaxInvoice)
        {
            var data = (from cbill in ent.CorporateBills
                        join booking in ent.Bookings on cbill.Booking_Id equals booking.Id
                        join swg in ent.StateWiseGSTINs on cbill.StateGstin_Id equals swg.State_Id
                        join rental in ent.RentalTypes on booking.RentalType equals rental.Id
                        join pt in ent.PackageTypes on rental.PackageType_Id equals pt.Id
                        join company in ent.Customers on cbill.Company_Id equals company.Id into bill_customer
                        from client in bill_customer.DefaultIfEmpty()
                        join serviceState in ent.StateMasters on client.State_Id equals serviceState.Id into serviceState_client
                        from ss in serviceState_client.DefaultIfEmpty()
                        join cab in ent.Cabs on booking.Cab_Id equals cab.Id
                        join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                        join desiredVm in ent.VehicleModels on booking.VehicleModel_Id equals desiredVm.Id
                        join fromState in ent.StateMasters on swg.State_Id equals fromState.Id
                        join nrdPlaceOfServie in ent.StateMasters on booking.NrdStateId equals nrdPlaceOfServie.Id into xyz
                        from nrdServiceState in xyz.DefaultIfEmpty()
                        where cbill.Id == id
                        select new TaxInvoiceViewModel
                        {
                            Id = cbill.Id,
                            GuestName = cbill.GuestName,
                            DutyAddress = cbill.DutyAddress,
                            DsDate = cbill.DsDate,
                            DsNo = cbill.DsNo,
                            RentalTypeName = rental.RentalTypeName,
                            Fare = cbill.Fare,
                            ExtraHrsRate = cbill.ExtraHrsRate,
                            ExtraKmsRate = cbill.ExtraKmsRate,
                            ExtraKms = cbill.ExtraKms,
                            ExtraHrs = cbill.ExtraHrs,
                            ExtraHrsCharge = cbill.ExtraHrsCharge,
                            ExtraKmsCharge = cbill.ExtraKmsCharge,
                            NightCharge = cbill.NightCharge,
                            NightRate = cbill.NightRate,
                            TotalNight = cbill.TotalNight,
                            TotalHrs = cbill.TotalHrs,
                            TotalKms = cbill.TotalKms,
                            Total = cbill.Total,
                            ParkCharge = cbill.ParkCharge,
                            TollCharge = cbill.TollCharge,
                            StateCharge = cbill.StateCharge,
                            InterStateCharge = cbill.InterStateCharge,
                            OutStationDays = cbill.OutStationDays,
                            OutStationChargeRate = cbill.OutStationChargeRate,
                            OutStationCharge = cbill.OutStationCharge,
                            VehicleName = desiredVm.ModelName,
                            VehicleNo = cab.VehicleNumber,
                            FromCompanyName=swg.CompanyName,
                            FromEmail=swg.EmailId,
                            FromAddress = swg.Address,
                            FromGSTIN = swg.Gstin,
                            FromStateName = fromState.StateName,

                            FromBankName=swg.BankName,
                            FromAC_No=swg.AC_No,
                            FromBranchAddress=swg.BranchAddress,
                            FromIFS_Code=swg.IFS_Code,
                            CompanyRegAdd = swg.CompanyRegAdd,
                            CompanyRegState = swg.CompanyRegState,
                            BilledAddress = client.FullAddress,
                            CompanyName = client != null ? client.CompanyName : booking.CustomerName,
                            CompanyGSTIN = client.GSTIN,
                            CompanyState= client!=null? ent.StateMasters.Where(x => x.StateCode == "9").FirstOrDefault().StateName:"UP",
                            //PlaceOfService = ss != null ? ss.StateName : nrdServiceState != null ? nrdServiceState.StateName : "",
                            //PlaceOfService = ss != null ? ent.StateMasters.Where(x => x.StateCode == client.GSTIN.Substring(0, 2)).FirstOrDefault().StateName : nrdServiceState != null ? nrdServiceState.StateName : "",
                            PlaceOfService = booking.City_Id != null? ent.CityMasters.Where(x => x.Id==booking.City_Id).FirstOrDefault().CityName:"",
                            BookingId = booking.BookingId,
                            CGST = cbill.CGST,
                            SGST = cbill.SGST,
                            IGST = cbill.IGST,
                            PickupDate = booking.PickupDate,
                            ParkTollStateCharge = cbill.ParkCharge + cbill.TollCharge + cbill.StateCharge,
                            IgstPercent = cbill.IgstPercent,
                            CgstPercent = cbill.CgstPercent,
                            SgstPercent = cbill.SgstPercent,
                            IsTaxInvoice = IsTaxInvoice,
                            BillFile = cbill.BillFile,
                            DiscountType = cbill.DiscountType,
                            DiscountValue = cbill.DiscountValue,
                            DiscountAmount = cbill.DiscountAmount,
                            MCD = cbill.MCD,
                            NetAmount = cbill.NetAmount,
                            PackageTypeName = pt.PType,
                            TaxInvoiceNumber = cbill.TaxInvoiceNumber,
                            TaxInvoiceDate = cbill.TaxInvoiceDate,
                            PickupAddress = booking.PickupAddress,
                            DropAddress = booking.DropAddress,
                            VisitedPlace = cbill.VisitedPlace,
                            JourneyDate = cbill.JourneyStartDate,
                            JourneyEndDate = cbill.JourneyClosingDate,
                            MiscCharge= cbill.MiscCharge,
                            IsNrg=cbill.IsNrg? "True" : "False",
                            ProformNo =cbill.ProformNo
                        }).FirstOrDefault();
           
            var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
            data.ShouldRoundOff = !packageArray.Contains(data.PackageTypeName);
            return data;
        }

        public ActionResult TaxInvoice(int id, DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", int page = 1, bool export = false, string Display = "d", bool IsTaxInvoice = false, int menuId = 0, bool isPdf = false)
        {
            var data = GetTaxInvoiceData(id, IsTaxInvoice);            
            string previousUrl = string.Empty;            
            previousUrl =Convert.ToString(System.Web.HttpContext.Current.Request.UrlReferrer);
            ViewBag.priviousURL = previousUrl;
            ViewBag.menuId = menuId;
            ViewBag.isPdf = isPdf;
            data.sDate = sDate;
            data.eDate = eDate;
            //data.IsNrg = isNrg;
            data.Term = term;
            data.Page = page;
            data.Export = export;
            data.Display = Display;
            return View(data);
        }

        //public ActionResult TaxInvoice1(int id, bool IsTaxInvoice = false, int menuId = 0)
        //{
        //    var data = GetTaxInvoiceData(id, IsTaxInvoice);
        //    ViewBag.menuId = menuId;
        //    return View(data);
        //}

        public ActionResult CancelInvoice(int Bill_Id, int menuId = 0)
        {
            var data = ent.CorporateBills.Find(Bill_Id);
            string boType = string.Empty;
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (data != null)
                    {
                        var booking = ent.Bookings.Find(data.Booking_Id);
                        boType = booking.BookingType;
                        //cancel invoice wrong data fill
                        var user = ent.UserLogins.Find(cr.GetUserDetailId());
                        var b = ent.Bookings.Find(data.Booking_Id);
                        data.CancelledBy = user.Email;
                        data.CancellationDate = cr.GetISTDate();
                        data.IsCancelled = true;
                        //change booking status and closed by, closing date after booking invoice is cancelled
                        booking.BookingStatus = 1;
                        booking.ClosedBy = null;
                        booking.ClosingDate = null;

                        var vendorBill = ent.VendorBills.OrderByDescending(a => a.Id).FirstOrDefault(a => a.Booking_Id == b.Id);
                        vendorBill.CancelledBy = user.Email;
                        vendorBill.CancellationDate = cr.GetISTDate();
                        vendorBill.IsCancelled = true;

                        var log = new Log
                        {
                            Booking_Id = booking.Id,
                            UpdateDate = cr.GetISTDate(),
                            UserLogin_Id = user.Id,
                            Description = "Booking with Id " + b.BookingId + " and DS No. " + data.DsNo + " is Cancelled by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                        };
                        ent.Logs.Add(log);
                        ent.SaveChanges();
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                }
                if (boType == "nrd")
                { return RedirectToAction("CorporateInvoiceList", new { isNrg = true, menuId = menuId }); }
                else
                { return RedirectToAction("CorporateInvoiceList", new { isNrg = false, menuId = menuId }); }
            }

        }

        public ActionResult VendorInvoiceList(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0, bool export = false)
        {
            int pageSize = 100;
            double totalPages = 0;
            int skip = 0;
            term = term.ToLower();
            var model = new CIBillingListVm();
            var data = new List<CIData>();
            if (!export)
            {

                double total = (from vb in ent.VendorBills
                                join b in ent.Bookings on vb.Booking_Id equals b.Id
                                join c in ent.Cabs on b.Cab_Id equals c.Id
                                join rt in ent.RentalTypes on b.RentalType equals rt.Id
                                join pk in ent.PackageTypes on rt.PackageType_Id equals pk.Id
                                join vh in ent.VehicleModels on c.VehicleModel_Id equals vh.Id
                                join city in ent.CityMasters on b.City_Id equals city.Id
                                join vendor in ent.Vendors on vb.Company_Id equals vendor.Id
                                where !vb.IsCancelled
                                &&
                                (
                                (term == "" && sDate == null && eDate == null)
                                ||
                                (
                                term == "" && (sDate != null && eDate != null) && (DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                                )
                                ||
                                (
                                term != "" && sDate != null && eDate != null && ((DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                                                                        && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term))
                                )
                                ||
                                (term != "" && sDate == null && eDate == null && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term)))
                                )
                                )
                                orderby vb.Id descending
                                select new CIData
                                {
                                    Id = vb.Id
                                }
                       ).Count();


                totalPages = Math.Ceiling(total / (double)pageSize);
                skip = pageSize * (page - 1);

                data = (from vb in ent.VendorBills
                        join b in ent.Bookings on vb.Booking_Id equals b.Id
                        join c in ent.Cabs on b.Cab_Id equals c.Id
                        join rt in ent.RentalTypes on b.RentalType equals rt.Id
                        join pk in ent.PackageTypes on rt.PackageType_Id equals pk.Id
                        join vh in ent.VehicleModels on c.VehicleModel_Id equals vh.Id
                        join city in ent.CityMasters on b.City_Id equals city.Id
                        join vendor in ent.Vendors on vb.Company_Id equals vendor.Id
                        where !vb.IsCancelled
                        &&
                        (
                        (term == "" && sDate == null && eDate == null)
                        ||
                        (
                        term == "" && (sDate != null && eDate != null) && (DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                        )
                        ||
                        (
                        term != "" && sDate != null && eDate != null && ((DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                                                                && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term))
                        )
                        ||
                        (term != "" && sDate == null && eDate == null && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term)))
                        )
                        )
                        orderby vb.JourneyClosingDate descending
                        select new CIData
                        {
                            Id = vb.Id,
                            BookedBy = b.BookedBy,
                            BookingId = b.BookingId,
                            BookingType = rt.RentalTypeName,
                            CabAlloted = vh.ModelName,
                            CabNo = c.VehicleNumber,
                            City = city.CityName,
                            ParkingChage = vb.ParkCharge,
                            TollCharge = vb.TollCharge,
                            InterStateCharge = vb.StateCharge,
                            CGST = vb.CGST,
                            SGST = vb.SGST,
                            IGST = vb.IGST,
                            ClosingTime = vb.EndHour,
                            StartTime = vb.StartHour,
                            JourneyDate = (DateTime)vb.JourneyStartDate,
                            JourneyEndDate = (DateTime)vb.JourneyClosingDate,
                            ClosingDate = (DateTime)b.ClosingDate,
                            ClosedBy = b.ClosedBy,
                            Company = vendor.CompanyName,
                            ExtraHours = vb.ExtraHrs,
                            TotalAmount = vb.Total,
                            ExtraKm = vb.ExtraKms,
                            Fare = vb.Fare,
                            InvoiceDate = vb.DsDate,
                            InvoiceNo = vb.DsNo,
                            NightCharges = vb.NightCharge,
                            Status = vb.IsCancelled ? "Cancelled" : "Ok",
                            TotalHours = vb.TotalHrs,
                            TotalKm = vb.TotalKms,
                            CabBooking_Id = vb.Booking_Id,
                            IsCancelled = vb.IsCancelled,
                            VendorCommission = vb.VendorCommision,
                            Mobile = b.ContactNo,
                            TotalNights = vb.TotalNight,
                            StartKm = vb.StartKms,
                            EndKm = vb.EndKms,
                            MCD = vb.MCD,
                            ExtraHrsCharge = vb.ExtraHrsCharge,
                            ExtraKmsCharge = vb.ExtraKmsCharge,
                            ExtraHrsRate = vb.ExtraHrsRate,
                            ExtraKmsRate = vb.ExtraKmsRate,
                            PackageTypeName = pk.PType,
                            MiscCharge=vb.MiscCharge
                        }
                       ).Skip(skip).Take(pageSize).ToList();
            }

            // export to excel
            else
            {
                var data2 = (from vb in ent.VendorBills
                             join b in ent.Bookings on vb.Booking_Id equals b.Id
                             join c in ent.Cabs on b.Cab_Id equals c.Id
                             join rt in ent.RentalTypes on b.RentalType equals rt.Id
                             join pk in ent.PackageTypes on rt.PackageType_Id equals pk.Id
                             join vh in ent.VehicleModels on c.VehicleModel_Id equals vh.Id
                             join city in ent.CityMasters on b.City_Id equals city.Id
                             join vendor in ent.Vendors on vb.Company_Id equals vendor.Id
                             where !vb.IsCancelled
                             &&
                             (
                             (term == "" && sDate == null && eDate == null)
                             ||
                             (
                             term == "" && (sDate != null && eDate != null) && (DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                             )
                             ||
                             (
                             term != "" && sDate != null && eDate != null && ((DbFunctions.TruncateTime(vb.JourneyClosingDate) >= sDate.Value && DbFunctions.TruncateTime(vb.JourneyClosingDate) <= eDate.Value)
                                                                     && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term))
                             )
                             ||
                             (term != "" && sDate == null && eDate == null && (vb.DsNo.ToLower().Contains(term) || b.ContactNo.Contains(term) || c.VehicleNumber.Contains(term) || b.BookingId.Contains(term) || vendor.CompanyName.ToLower().Contains(term)))
                             )
                             )
                             orderby vb.JourneyClosingDate descending
                             select new CIData
                             {
                                 Id = vb.Id,
                                 BookedBy = b.BookedBy,
                                 BookingId = b.BookingId,
                                 BookingType = rt.RentalTypeName,
                                 CabAlloted = vh.ModelName,
                                 CabBooked=vh.ModelName,
                                 CabNo = c.VehicleNumber,
                                 City = city.CityName,
                                 ParkingChage = vb.ParkCharge,
                                 TollCharge = vb.TollCharge,
                                 InterStateCharge = vb.InterStateCharge,
                                 CGST = vb.CGST,
                                 SGST = vb.SGST,
                                 IGST = vb.IGST,
                                 ClosingTime = vb.EndHour,
                                 StartTime = vb.StartHour,
                                 JourneyDate = (DateTime)vb.JourneyStartDate,
                                 JourneyEndDate = (DateTime)vb.JourneyClosingDate,
                                 ClosingDate = (DateTime)b.ClosingDate,
                                 ClosedBy = b.ClosedBy,
                                 Company = vendor.CompanyName,
                                 ExtraHours = vb.ExtraHrs,
                                 TotalAmount = vb.Total,
                                 ExtraKm = vb.ExtraKms,
                                 Fare = vb.Fare,
                                 InvoiceDate = vb.DsDate,
                                 InvoiceNo = vb.DsNo,
                                 NightCharges = vb.NightCharge,
                                 Status = vb.IsCancelled ? "Cancelled" : "Ok",
                                 TotalHours = vb.TotalHrs,
                                 TotalKm = vb.TotalKms,
                                 CabBooking_Id = vb.Booking_Id,
                                 IsCancelled = vb.IsCancelled,
                                 VendorCommission = vb.VendorCommision,
                                 Mobile = b.ContactNo,
                                 TotalNights = vb.TotalNight,
                                 StartKm = vb.StartKms,
                                 EndKm = vb.EndKms,
                                 MCD = vb.MCD,
                                 ExtraHrsCharge = vb.ExtraHrsCharge,
                                 ExtraKmsCharge = vb.ExtraKmsCharge,
                                 ExtraHrsRate = vb.ExtraHrsRate,
                                 ExtraKmsRate = vb.ExtraKmsRate,
                                 PackageTypeName = pk.PType,
                                 MiscCharge=vb.MiscCharge,
                                 OutStationCharge = vb.OutStationCharge,
                                 DiscountAmount=vb.DiscountAmount.Value,
                                 NetAmount=vb.NetAmount.Value,
                                 GuestName=vb.GuestName

                             }
                       ).ToList();
                var ed = (from d in data2
                          select new VendorDsExcelModel
                          {
                              BookingId = d.BookingId,
                             // BookedBy = d.BookedBy,
                              CabBooked = d.CabBooked,
                              //DiscountValue = d.DiscountValue,
                              TaxInvoiceNumber = d.TaxInvoiceNumber,
                              //DutyDate = d.InvoiceDate,
                              JourneyEndDate = d.JourneyEndDate,
                              GuestName=d.GuestName,


                              StartKm = d.StartKm,
                              NightCharges = d.NightCharges,
                              StateCharge = d.InterStateCharge,
                              StartTime = d.StartTime,
                            //  BookingType = d.BookingType,
                              PackageType = d.PackageTypeName,
                              ExtraKm = d.StartKm,
                              EndKm = d.EndKm,
                              Fare = d.Fare,
                              TaxInvoiceDate = d.InvoiceDate,
                             
                             // City = d.City,
                             // ClosedBy = d.ClosedBy,
                            //  ClosingDate = d.ClosingDate,
                              ClosingTime = d.ClosingTime,
                              //CabAlloted = d.CabAlloted,
                              JourneyStartDate = d.JourneyDate,
                              ExtraHours = d.ExtraHours,
                             // DiscountType = d.DiscountType,
                              CabNo = d.CabNo,
                              CGST = d.CGST,
                              SGST = d.SGST,
                              IGST = d.IGST,
                              ParkingChage = d.ParkingChage,
                              SubTotal=d.NetAmount,
                              DiscountAmount = d.DiscountAmount,
                              NetAmount = d.NetAmount - d.DiscountAmount,
                              TotalAmount = d.TotalAmount,
                              MCD = d.MCD,
                            
                              //Mobile = d.Mobile,
                             // NrgType = d.NrgType,
                              Vendor = d.Company,
                            
                              TollCharge = d.TollCharge,
                              TotalHours = d.TotalHours,
                              TotalKm = d.TotalKm,
                              TotalNights = d.TotalNights,
                             // VendorCommission = d.VendorCommission,
                              ExtraHourCharge = d.ExtraHrsCharge,
                              ExtraKmCharge = d.ExtraKmsCharge,
                              ExtraHourRate = d.ExtraHrsRate,
                              ExtraKmRate = d.ExtraKmsRate,
                              MiscellaneousCharge=d.MiscCharge,
                              TACharge=d.OutStationCharge

                          }
                            ).ToList();
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =vendorDutySlips.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            model.SrNo = skip;
            model.Bills = data;
            model.sDate = sDate;
            model.eDate = eDate;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult VendorTaxInvoice(int id, bool IsTaxInvoice = false, int menuId = 0,bool IsPdf=false)
        {
            var data = (from vbill in ent.VendorBills
                        join booking in ent.Bookings on vbill.Booking_Id equals booking.Id
                        join swg in ent.StateWiseGSTINs on vbill.StateGstin_Id equals swg.State_Id
                        join rental in ent.RentalTypes on booking.RentalType equals rental.Id
                        join pt in ent.PackageTypes on rental.PackageType_Id equals pt.Id
                        join vendor in ent.Vendors on vbill.Company_Id equals vendor.Id
                        join serviceState in ent.StateMasters on vendor.StateMaster_Id equals serviceState.Id
                        join cab in ent.Cabs on booking.Cab_Id equals cab.Id
                        join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                        join fromState in ent.StateMasters on swg.State_Id equals fromState.Id
                        where vbill.Id == id
                        select new TaxInvoiceViewModel
                        {
                            Id = vbill.Id,
                            GuestName = vbill.GuestName,
                            DutyAddress = vbill.DutyAddress,
                            DsDate = vbill.DsDate,
                            DsNo = vbill.DsNo,
                            RentalTypeName = rental.RentalTypeName,
                            Fare = vbill.Fare,
                            ExtraHrsRate = vbill.ExtraHrsRate,
                            ExtraKmsRate = vbill.ExtraKmsRate,
                            ExtraKms = vbill.ExtraKms,
                            ExtraHrs = vbill.ExtraHrs,
                            ExtraHrsCharge = vbill.ExtraHrsCharge,
                            ExtraKmsCharge = vbill.ExtraKmsCharge,
                            NightCharge = vbill.NightCharge,
                            NightRate = vbill.NightRate,
                            TotalNight = vbill.TotalNight,
                            TotalHrs = vbill.TotalHrs,
                            TotalKms = vbill.TotalKms,
                            Total = vbill.Total,
                            ParkCharge = vbill.ParkCharge,
                            TollCharge = vbill.TollCharge,
                            StateCharge = vbill.StateCharge,
                            InterStateCharge = vbill.InterStateCharge,
                            OutStationDays = vbill.OutStationDays,
                            OutStationChargeRate = vbill.OutStationChargeRate,
                            OutStationCharge = vbill.OutStationCharge,
                            VehicleName = vm.ModelName,
                            VehicleNo = cab.VehicleNumber,
                            FromAddress = swg.Address,
                            FromGSTIN = swg.Gstin,
                            FromStateName = fromState.StateName,
                            BilledAddress = vendor.FullAddress,
                            CompanyName = vendor.CompanyName,
                            CompanyGSTIN = vendor.GSTIN,
                            PlaceOfService = serviceState.StateName,
                            BookingId = booking.BookingId,
                            DiscountAmount=vbill.DiscountAmount.Value,
                            NetAmount=vbill.NetAmount.Value,
                            CGST = vbill.CGST,
                            SGST = vbill.SGST,
                            IGST = vbill.IGST,
                            PickupDate = booking.PickupDate,
                            ParkTollStateCharge = vbill.ParkCharge + vbill.TollCharge + vbill.StateCharge,
                            IgstPercent = vbill.IgstPercent,
                            CgstPercent = vbill.CgstPercent,
                            SgstPercent = vbill.SgstPercent,
                            IsTaxInvoice = IsTaxInvoice,
                            MCD = vbill.MCD,
                            BillFile = vbill.BillFile,
                            PackageTypeName = pt.PType,
                            MiscCharge=vbill.MiscCharge,
                            JourneyEndDate=vbill.JourneyClosingDate
                            
                        }).FirstOrDefault();
            ViewBag.menuId = menuId;
            var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
            data.ShouldRoundOff = !packageArray.Contains(data.PackageTypeName);
            data.isPdf = IsPdf;
            return View(data);
        }

      

        public ActionResult ChangeToDispatch(int bookingId, string bookingType = "regular", int menuId = 0)
        {

            var model = new DispatchBookingModel();
            model.BookingId = bookingId;
            model.BookingType = bookingType;
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeToDispatch(DispatchBookingModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var b = ent.Bookings.Find(model.BookingId);
                    b.BookingStatus = (int)BookingStatus.Dispatched;
                    b.StartHour = model.StartHour;
                    b.StartKms = model.StartKms;
                    b.UpdateDate = cr.GetISTDate();
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    b.UpdatedBy = user.Id;
                    ent.SaveChanges();
                    var log = new Log
                    {
                        Booking_Id = b.Id,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "Booking with Id " + b.BookingId + " is Dispatched by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("ShowBooking", new { bookingType = model.BookingType, menuId = model.MenuId });
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    tran.Rollback();
                    return View(model);
                }
            }
        }
            
        //[AllowAnonymous]
        //public ActionResult table()
        //{
        //    return View();
        //}

        public JsonResult getLocs(int rentalTypeId, int clientId, int vModelId)
        {
            var data = ent.ClientPackages.FirstOrDefault(a => a.RentalType_Id == rentalTypeId
&& a.Customer_Id == clientId && a.VehicleModel_Id == vModelId);
            string pickup = "";
            string drop = "";
            int noOfDays = 0;
            if (data != null)
            {
                pickup = data.PickupLocation;
                drop = data.DropLocation;
                noOfDays = data.NoOfDays;
            }
            var rm = new { pickupLocation = pickup, dropLocation = drop, noOfDays = noOfDays };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReleaseCab(int id,int pbookingCat, DateTime? sDate, DateTime? eDate, int page = 1, string term = "", string bookingStatus = "1", int menuId = 0)
        {
            int pendingBookingType = pbookingCat;
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var booking = ent.Bookings.Find(id);
                    if (booking.DriverId == null || booking.Cab_Id == null)
                    {
                        TempData["mesg"] = "Cab not release, driver or cab not assign";
                        return RedirectToAction("ShowBooking", new { sDate = sDate, eDate = eDate, page = page, term = term, bookingStatus = bookingStatus, pbookingCat = pendingBookingType, menuId = menuId });
                    }
                    var driver = ent.Drivers.Find(booking.DriverId);
                    driver.IsAvailable = true;
                    var cab = ent.Cabs.Find(booking.Cab_Id);
                    cab.IsAvailable = true;
                    booking.IsReleasedCab = true;
                    booking.UpdateDate = cr.GetISTDate();
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    booking.UpdatedBy = user.Id;
                    var log = new Log
                    {
                        Booking_Id = booking.Id,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "Booking with Id " + booking.BookingId + " is release for bill by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
                return RedirectToAction("ShowBooking", new { sDate = sDate, eDate = eDate, page = page, term = term, bookingStatus = bookingStatus, pbookingCat = pendingBookingType, menuId = menuId });
            }
        }


        public ActionResult AvailableDrivers(string term = "", int page = 1, int menuId = 0)
        {
            var model = new DriverListVm();
            var data = cr.GetAvailableDrivers();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.DriverName.ToLower().Contains(term) || (a.MobileNumber != null && a.MobileNumber.Contains(term))).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 300;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Drivers = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult AvailableCabs(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CabListVm();
            var data = cr.GetAvailableCabs();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.Company.ToLower().Contains(term) || a.VehicleNumber.ToLower().Contains(term) || a.ModelName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 300;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Cabs = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ContentResult GenerateTaxInvoice(int invoiceId, string invoiceNo, DateTime invoiceDate)
        {
            if (ent.CorporateBills.Any(a => a.TaxInvoiceNumber == invoiceNo))
                return Content("This invoice number has already used.");
            var bill = ent.CorporateBills.Find(invoiceId);
            try
            {
                bill.TaxInvoiceDate = invoiceDate;
                bill.TaxInvoiceNumber = invoiceNo;
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server Error");
            }
        }

        public ActionResult DeleteBooking(int id, DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0)
        {
            try
            {
                ent.Database.ExecuteSqlCommand(@"delete from  booking where id=" + id);

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ShowBooking", new { sDate = sDate, eDate = eDate, page = page, term = term, menuId = menuId });
        }


        public JsonResult VehicleForClient(int clientId)
        {
            var data = GetVehicleModelListFromPackage(clientId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VehicleForNrg()
        {
            var data = GetVehicleModelListFromNRGPackage();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //        public JsonResult LoadPackageType(bool nrgWalkin = false, int clientId = 0)
        //        {
        //            string query = "";
        //            if (nrgWalkin)
        //            {

        //            }
        //            else
        //            {
        //                // if client is child

        //                var client = ent.Customers.Find(clientId);
        //                if (client != null)
        //                {
        //                    if (client.ParentCustomer_Id != null)
        //                    {
        //                        var parent = ent.Customers.Find(client.ParentCustomer_Id);
        //                        clientId = parent == null ? 0 : parent.Id;
        //                    }
        //                }
        //                 query = @"select * from PackageType where id in(
        //select distinct pt.Id from ClientPackage cp
        //join  RentalType rt on cp.RentalType_Id=rt.Id
        //join PackageType pt on rt.PackageType_Id=pt.Id
        //where cp.Customer_Id="+clientId+")";

        //            }
        //            var data = ent.Database.SqlQuery<PackageType>(query).ToList();
        //        }

        [HttpPost]
        public ActionResult CreateInvoice(List<int> ids, string invoiceNo, string invoiceDate,string PONo,string PODate)
        {
            string InvNo = invoiceNo; //getAutoGenratedTaxInvoiceNo();
            //var vv=getAutoNPreviousTaxInvoiceNo();
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (ent.CorporateInvoiceFiles.Any(a => a.InvoiceNumber == InvNo))
                        return Content("This invoice number has already used.");
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    var inv = new CorporateInvoiceFile
                    {
                        InvoiceDate = Convert.ToDateTime(invoiceDate),
                        InvoiceFile = "",
                        CreatedBy = user.Email,
                        InvoiceNumber = InvNo,
                        PONo= PONo,
                        PODate=Convert.ToDateTime(PODate),
                        CreatedDate=DateTime.Now,
                        IsCancel=false
                    };
                    ent.CorporateInvoiceFiles.Add(inv);
                    ent.SaveChanges();
                    int cinvid = inv.Id;
                    int companyId = 0;

                    List<tempUserBooking> lst = new List<tempUserBooking>();
                    string usern = Convert.ToString(Session["uEmail"]);
                    string sesid = Convert.ToString(Session.SessionID);

                    //List<tempUserBookingVM> lst = new List<tempUserBookingVM>();
                    //if (Session["bookinglst"] != null)
                    //{
                    //    lst = (List<tempUserBookingVM>)Session["bookinglst"];
                    //}
                    if (ent.tempUserBookings.Any(x => x.Username == usern && x.SessionId == sesid))
                    {
                        lst = ent.tempUserBookings.Where(x => x.Username == usern && x.SessionId == sesid).ToList();
                        if (lst.Count > 0)
                        {
                            foreach (var id in lst)
                            {
                                var ci = ent.CorporateBills.Find(id.BookingId);
                                if (companyId == 0 && companyId != ci.Company_Id)
                                    companyId = ci.Company_Id;
                                if (ci.Company_Id != companyId)
                                {
                                    tran.Rollback();
                                    return Content("You can not generate invoice of more than one company.");
                                }
                                ci.IsBilled = true;
                                ci.TaxInvoiceNumber = InvNo;
                                ci.TaxInvoiceDate = Convert.ToDateTime(invoiceDate);
                                var invoiceFileDetail = new CorporateInvoiceFileDetail
                                {
                                    CorporateBill_Id = id.BookingId,
                                    CorporateInvoiceFile_Id = inv.Id
                                };
                                ent.CorporateInvoiceFileDetails.Add(invoiceFileDetail);
                                ent.SaveChanges();
                                //delete temp data
                                //remove from db
                                if (ent.tempUserBookings.Any(x => x.BookingId ==id.BookingId))
                                {
                                    tempUserBooking obd = ent.tempUserBookings.Where(x => x.BookingId == id.BookingId).FirstOrDefault();
                                    ent.tempUserBookings.Remove(obd);
                                    ent.SaveChanges();
                                }
                            }
                        }
                    }
                   
                   

                    var log = new Log
                    {
                        Booking_Id = cinvid,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "TaxInvoice with Id " + cinvid.ToString() + " and TaxInvoice No. " + InvNo + " is Created by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                   // Session["bookinglst"] = null;
                   

                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("Server Error");
                }
            }
        }

        [HttpPost]
        public ActionResult CreateInvoiceAll(List<int> ids, string invoiceNo, string invoiceDate, string PONo, string PODate)
        {
            string InvNo = invoiceNo;
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (ent.CorporateInvoiceFiles.Any(a => a.InvoiceNumber == InvNo))
                        return Content("This invoice number has already used.");
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    var inv = new CorporateInvoiceFile
                    {
                        InvoiceDate = Convert.ToDateTime(invoiceDate),
                        InvoiceFile = "",
                        CreatedBy = user.Email,
                        InvoiceNumber = InvNo,
                        PONo = PONo,
                        PODate = Convert.ToDateTime(PODate),
                        CreatedDate = DateTime.Now,
                        IsCancel = false
                    };
                    ent.CorporateInvoiceFiles.Add(inv);
                    ent.SaveChanges();
                    int cinvid = inv.Id;
                    //int companyId = 0;   

                    //add procedure to create invoice
                    string sesid = Convert.ToString(Session.SessionID);
                    string cpbQuery = "exec CreateInvoiceUsingTempData @CopInvId,@username,@sessId";
                    var re = ent.Database.ExecuteSqlCommand(cpbQuery,
                       new SqlParameter("@CopInvId", cinvid),
                       new SqlParameter("@username", user.Email),
                       new SqlParameter("@sessId", sesid)
                       );

                    //foreach (var id in ids)
                    //{
                    //    var ci = ent.CorporateBills.Find(id);
                    //    if (companyId == 0 && companyId != ci.Company_Id)
                    //        companyId = ci.Company_Id;
                    //    if (ci.Company_Id != companyId)
                    //    {
                    //        tran.Rollback();
                    //        return Content("You can not generate invoice of more than one company.");
                    //    }
                    //    ci.IsBilled = true;
                    //    ci.TaxInvoiceNumber = InvNo;
                    //    ci.TaxInvoiceDate = Convert.ToDateTime(invoiceDate);
                    //    var invoiceFileDetail = new CorporateInvoiceFileDetail 
                    //    {
                    //        CorporateBill_Id = id,
                    //        CorporateInvoiceFile_Id = inv.Id
                    //    };
                    //    ent.CorporateInvoiceFileDetails.Add(invoiceFileDetail);
                    //    ent.SaveChanges();
                    //}


                    var log = new Log
                    {
                        Booking_Id = cinvid,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "TaxInvoice with Id " + cinvid.ToString() + " and TaxInvoice No. " + InvNo + " is Created by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                  //  Session["bookinglst"] = null;
                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("Server Error");
                }
            }
        }

        [HttpPost]
        public ActionResult EditTaxInvoice(List<int> ids, string invoiceNo, int taxID,string remark)
        {
            string InvNo = invoiceNo; 
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var CorInvFiles = ent.CorporateInvoiceFiles.Where(a => a.InvoiceNumber == InvNo).FirstOrDefault();
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    //var inv = new CorporateInvoiceFile
                    //{
                    //    InvoiceDate = DateTime.Now,
                    //    InvoiceFile = "",
                    //    CreatedBy = user.Email,
                    //    InvoiceNumber = InvNo,                     
                    //    PODate = DateTime.Now,
                    //    IsCancel = false
                    //};
                    //CorInvFiles.CreatedDate=
                    CorInvFiles.Remark = remark;
                    ent.Entry(CorInvFiles).State = System.Data.Entity.EntityState.Modified;
                    ent.SaveChanges();
                    int cinvid = taxID;
                    int companyId = 0;

                    var rows = ent.CorporateInvoiceFileDetails.Where(x=>x.CorporateInvoiceFile_Id==taxID).ToList();
                    foreach (var row in rows)
                    {
                        ent.CorporateInvoiceFileDetails.Remove(row);
                    }
                    ent.SaveChanges();

                    foreach (var id in ids)
                    {
                        var ci = ent.CorporateBills.Find(id);
                        if (companyId == 0 && companyId != ci.Company_Id)
                            companyId = ci.Company_Id;
                        if (ci.Company_Id != companyId)
                        {
                            tran.Rollback();
                            return Content("You can not generate invoice of more than one company.");
                        }
                        ci.IsBilled = true;
                        var invoiceFileDetail = new CorporateInvoiceFileDetail
                        {
                            CorporateBill_Id = id,
                            CorporateInvoiceFile_Id = taxID
                        };
                        ent.CorporateInvoiceFileDetails.Add(invoiceFileDetail);
                    }
                    ent.SaveChanges();

                    var log = new Log
                    {
                        Booking_Id = cinvid,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "TaxInvoice with Id " + cinvid.ToString() + " and TaxInvoice No. " + InvNo + " is updated by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();

                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("Server Error");
                }
            }
        }



        public ActionResult EditBookingBill(int id, int menuId = 0, string isNrg = "false", string term = "", int page = 1)
        {
            CorporateBill cpBill = new CorporateBill();
            TempData["priviousURL"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            TempData.Keep("priviousURL");
            if (term=="edit")
             cpBill = ent.CorporateBills.Where(x => x.Booking_Id == id && x.IsCancelled == false).FirstOrDefault();
            else
               cpBill = ent.CorporateBills.Where(x => x.Id == id && x.IsCancelled == false).FirstOrDefault();
            var booking = ent.Bookings.Find(cpBill.Booking_Id);
            var vendorBill=ent.VendorBills.Where(x => x.Booking_Id == cpBill.Booking_Id && x.IsCancelled == false).FirstOrDefault();
            var model = new CorporateBillingDTO();
            //display
                    
            model.DesiredCar = ent.VehicleModels.Find(booking.VehicleModel_Id).ModelName;
            var cab = ent.Cabs.Find(booking.Cab_Id);
           
            ViewBag.bookingId = booking.BookingId;
            ViewBag.CabNo = cab.VehicleNumber;
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;

            var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();           
            model.DiscountTypes = new SelectList(cr.GetDiscountTypes(), "Text", "Value",cpBill.DiscountType);
            
            model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "DisplayName", cpBill.StateGstin_Id);
            model.StateGstin_Id = cpBill.StateGstin_Id;
            ///package type
            var rentalType = ent.RentalTypes.Find(booking.RentalType);            
            var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
            model.PackageType = packageType.PackageTypeName;

            //package
            var vehicleModel = ent.VehicleModels.Find(booking.VehicleModel_Id);
            double package8HourFare = 0;
           
            //
            if (booking.BookingType == "nrd" && booking.NrgType == "walkin")
            {
                ViewBag.companyName = "NRD Invoice";
                var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
              
                if (packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                {
                    var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == booking.VehicleModel_Id && a.MinHrs == 8);
                    if (nrgPackage8Hour == null)
                        throw new Exception("package for 8 hour does not found");
                    package8HourFare = nrgPackage8Hour.Fare;
                }
                model.Package8HourFare = package8HourFare;
                model.ExtraHrsRate = nrgPackage.ExtraPerHour;
                model.ExtraKmsRate = nrgPackage.ExtraPerKm;
                model.NightRate = nrgPackage.NightCharges;
                model.Fare = nrgPackage.Fare;
                model.Total = nrgPackage.Fare;
                model.MinHrs = nrgPackage.MinHrs;
                model.MinKm = nrgPackage.MinKm;
                model.OutStationChargeRate = nrgPackage.OutStationCharge;
                model.IsNrg = true;
                if (packageType.PType == "OUTSTATION")
                {
                    model.OutStationDays = (Convert.ToDateTime(cpBill.JourneyClosingDate) - Convert.ToDateTime(cpBill.JourneyStartDate)).TotalDays + 1;
                    model.OutStationCharge = model.OutStationChargeRate * model.OutStationDays;
                }
            }
            else
            {
                int clientId = booking.Client_Id;
                var client = ent.Customers.Find(clientId);

                //find parent client 
                if (client.ParentCustomer_Id != null)
                {
                    clientId = client.ParentCustomer_Id ?? 0;
                }

                var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == vehicleModel.Id && a.RentalType_Id == booking.RentalType);
                if (packageType.PType == "Local Run" && package.MinHrs == 4)
                {
                    //var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == booking.Client_Id && a.VehicleModel_Id == vehicleModel.Id && a.MinHrs == 8);
                    var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == clientId && a.VehicleModel_Id == vehicleModel.Id && a.MinHrs == 8);
                    if (package8Hour == null)
                        throw new Exception("package for 8 hour does not found");
                    package8HourFare = package8Hour.Fare;
                }
                model.Package8HourFare = package8HourFare;
                ViewBag.companyName = ent.Customers.Find(booking.Client_Id).CompanyName;
              
                model.ExtraHrsRate = package.ExtraPerHour;
                model.ExtraKmsRate = package.ExtraPerKm;
                model.NightRate = package.NightCharges;
                model.Fare = package.Fare;
                model.Total = package.Fare;
                model.MinHrs = package.MinHrs;
                model.MinKm = package.MinKm;
                model.OutStationChargeRate = package.OutStationCharge;
                if (packageType.PType.Contains("monthly"))
                {
                    model.Fare = Math.Round(package.Fare / booking.DaysInMonth, 3);
                    model.Total = model.Fare;
                }
                if (packageType.PType == "OUTSTATION")
                {

                    model.OutStationDays = (Convert.ToDateTime(cpBill.JourneyClosingDate) - Convert.ToDateTime(cpBill.JourneyStartDate)).TotalDays + 1;
                    model.OutStationCharge = model.OutStationChargeRate * model.OutStationDays;
                    model.ExtraHrsCharge = 0;
                    model.Fare = package.Fare * model.OutStationDays;

                }
            }
            if (term == "Editbill")
            {
                model.ExtraKmsRate = cpBill.ExtraKmsRate;
                model.ExtraHrsRate = cpBill.ExtraHrsRate;
                model.Fare = cpBill.Fare;
                model.NightRate = cpBill.NightRate;
                model.MinHrs = cpBill.MinHrs;
                model.MinKm = cpBill.MinKm;
                model.OutStationChargeRate = cpBill.OutStationChargeRate;
                model.OutStationDays = cpBill.OutStationDays;
                model.OutStationCharge = cpBill.OutStationCharge;
                model.ExtraHrsCharge = cpBill.ExtraHrsCharge;

            }
            else
            {
                //if (packageType.PType == "OUTSTATION")
                //{
                //    model.OutStationDays = (Convert.ToDateTime(cpBill.JourneyClosingDate) - Convert.ToDateTime(cpBill.JourneyStartDate)).TotalDays + 1;
                //    model.OutStationCharge = model.OutStationChargeRate * model.OutStationDays;
                //    model.ExtraHrsCharge = 0;
                //}

            }
         //   model.IsNrg = Convert.ToBoolean(isNrg);
            model.JourneyStartDate = cpBill.JourneyStartDate;
            model.JourneyClosingDate = cpBill.JourneyClosingDate;
            
            model.Booking_Id = booking.Id;
            model.GuestName = booking.CustomerName;
            model.DutyAddress = booking.PickupAddress;
            model.Company_Id = booking.Client_Id;
          
            model.StartKms = cpBill.StartKms;
            model.EndKms = cpBill.EndKms;
            model.TotalKms = cpBill.TotalKms;
            model.ExtraKms = cpBill.ExtraKms;
            model.ExtraKmsCharge = cpBill.ExtraKmsCharge;
            

            model.StartHour = cpBill.StartHour;
            model.EndHour = cpBill.EndHour;
            model.TotalHrs = cpBill.TotalHrs;
            model.ExtraHrs = cpBill.ExtraHrs;
           
           
            model.TotalNight = cpBill.TotalNight;
            model.NightCharge = cpBill.NightCharge;
            
            model.MiscCharge = cpBill.MiscCharge;
            model.ParkCharge = cpBill.ParkCharge;
            model.TollCharge = cpBill.TollCharge;
            model.MCD = cpBill.MCD;
            model.InterStateCharge = cpBill.InterStateCharge;
            model.FuelEfficiency = cpBill.FuelEfficiency;
            model.DiscountType = cpBill.DiscountType;
            model.DiscountValue = cpBill.DiscountValue;
            model.DiscountAmount = cpBill.DiscountAmount;
            model.VisitedPlace = cpBill.VisitedPlace;

            model.NetAmount = cpBill.NetAmount;
            model.Total = cpBill.Total-(cpBill.CGST+cpBill.SGST+cpBill.IGST);

          

            if (vendorBill.VendorCommision >= 0)
            {
                model.CommisionType = "Percent";
                model.VendorCommision = vendorBill.VendorCommision;
            }
            else
            {
                model.CommisionType = "None";
            }

            model.Term = term;
            model.Page = page;
            //model.sDate = sDate;
            //model.eDate = eDate;
            model.MenuId = menuId;
            return View(model);
        }
        [HttpPost]
        public ActionResult EditBookingBill(CorporateBillingDTO model)
        {
            int pType = 0;
            string previousUrl = string.Empty;
            string bookingType = string.Empty;
            if (TempData["priviousURL"] != null)
            {
                previousUrl = Convert.ToString(TempData["priviousURL"]);
            }
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
                    model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
                    model.DiscountTypes = new SelectList(cr.GetDiscountTypes(), "Text", "Value");
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }
                    var booking = ent.Bookings.Find(model.Booking_Id);


                    var rentalType = ent.RentalTypes.Find(booking.RentalType);
                    if (rentalType == null)
                        throw new FileNotFoundException("This rental type has deleted.");
                    pType = rentalType.PackageType_Id;
                    var corporateBill = ent.CorporateBills.Where(x=>x.Booking_Id==model.Booking_Id && x.IsCancelled == false).FirstOrDefault();
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    bookingType = booking.BookingType;
                    #region corporate billing   
                    corporateBill.GuestName = booking.CustomerName;
                    corporateBill.UpdatedBy = user.Id;
                    corporateBill.UpdateDate = cr.GetISTDate();
                    corporateBill.DsDate = cr.GetISTDate();
                    corporateBill.IsCancelled = false;
                    corporateBill.IsNrg =Convert.ToBoolean(model.IsNrg);

                    corporateBill.Company_Id = model.Company_Id;
                    corporateBill.StateGstin_Id = model.StateGstin_Id;

                    corporateBill.Fare = model.Fare;
                    corporateBill.Total = model.Total;
                    corporateBill.MinHrs = model.MinHrs;
                    corporateBill.MinKm = model.MinKm;
                    corporateBill.OutStationDays = model.OutStationDays;
                    corporateBill.OutStationCharge = model.OutStationCharge;
                    corporateBill.OutStationChargeRate = model.OutStationChargeRate;

                    corporateBill.JourneyStartDate = model.JourneyStartDate;
                    corporateBill.JourneyClosingDate = model.JourneyClosingDate;

                    corporateBill.StartKms = model.StartKms;
                    corporateBill.EndKms = model.EndKms;
                    corporateBill.TotalKms = model.TotalKms;
                    corporateBill.ExtraKms = model.ExtraKms;
                    corporateBill.ExtraKmsCharge = model.ExtraKmsCharge;
                    corporateBill.ExtraKmsRate = model.ExtraKmsRate;

                    corporateBill.StartHour = model.StartHour;
                    corporateBill.EndHour = model.EndHour;
                    corporateBill.TotalHrs = model.TotalHrs;
                    corporateBill.ExtraHrs = model.ExtraHrs;
                    corporateBill.ExtraHrsCharge = model.ExtraHrsCharge;
                    corporateBill.ExtraHrsRate = model.ExtraHrsRate;

                   corporateBill.TotalNight = model.TotalNight;
                   corporateBill.NightCharge = model.NightCharge;
                    corporateBill.NightRate = model.NightRate;
                   corporateBill.MiscCharge= model.MiscCharge;
                   corporateBill.ParkCharge= model.ParkCharge;
                   corporateBill.TollCharge= model.TollCharge;
                   corporateBill.MCD= model.MCD;

                    corporateBill.FuelEfficiency = model.FuelEfficiency;
                    corporateBill.DiscountType = model.DiscountType;
                    corporateBill.DiscountValue = model.DiscountValue;
                    corporateBill.DiscountAmount = model.DiscountAmount;
                    corporateBill.NetAmount = model.NetAmount;
                    corporateBill.Total = model.Total;
                    corporateBill.InterStateCharge = model.InterStateCharge;

                    corporateBill.VisitedPlace = model.VisitedPlace;
                   

                    var taxInPercent = ent.Taxes.FirstOrDefault().Amount;
                    double taxableAmount = model.Total;
                    double tax = Math.Round((taxableAmount * taxInPercent) / 100, 2);
                    corporateBill.Total += tax;

                    var client = ent.Customers.Find(booking.Client_Id);
                    int reglarStateId = 0;
                    if (booking.BookingType == "regular")
                    {
                        if (ent.StateMasters.Any(x => x.StateCode == client.GSTIN.Substring(0, 2)))
                        {
                            reglarStateId = ent.StateMasters.Where(x => x.StateCode == client.GSTIN.Substring(0, 2)).FirstOrDefault().Id;
                        }
                        else
                        {
                            reglarStateId = client.State_Id;
                        }
                        corporateBill.IsNrg = false;
                    }
                    else 
                    {
                        corporateBill.IsNrg = true;
                    }

                    int stateId = booking.BookingType == "regular" ? reglarStateId : (int)booking.NrdStateId;
                    if (stateId == model.StateGstin_Id)
                    {
                        corporateBill.CgstPercent = Math.Round(taxInPercent / 2, 2);
                        corporateBill.SgstPercent = Math.Round(taxInPercent / 2, 2);
                        corporateBill.SGST = Math.Round(tax / 2, 2);
                        corporateBill.CGST = Math.Round(tax / 2, 2);

                        corporateBill.IgstPercent = 0;
                        corporateBill.IGST = 0;

                    }
                    else
                    {
                        corporateBill.IgstPercent = Math.Round(taxInPercent, 2);
                        corporateBill.IGST = Math.Round(tax, 2);

                        corporateBill.CgstPercent =0;
                        corporateBill.SgstPercent =0;
                        corporateBill.SGST = 0;
                        corporateBill.CGST = 0;
                    }
                    
                    if (model.MinKm == 0)
                    {
                        corporateBill.ExtraKms = 0;
                    }
                    if (model.MinHrs == 0)
                    {
                        corporateBill.ExtraHrs = 0;
                    }
                    corporateBill.BillFile = "";
                    ent.Entry(corporateBill).State = System.Data.Entity.EntityState.Modified;
                    ent.SaveChanges();
                  
                    //var driver = ent.Drivers.Find(booking.DriverId);
                    //driver.IsAvailable = true;                  
                    // cab.IsAvailable = true;
                    booking.BookingStatus = (int)BookingStatus.Completed;
                    booking.ClosedBy = user.Email;
                    booking.ClosingDate = cr.GetISTDate();
                    ent.SaveChanges();
                    #endregion

                    //vendor billing section start
                    #region vendor billing
                    var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
                    var cab = ent.Cabs.Find(booking.Cab_Id);
                    var vendor = ent.Vendors.Find(cab.Vendor_Id);
                    if (vendor == null)
                        throw new FileNotFoundException("This cab does not have any vendor.");
                   

                    var vBill = ent.VendorBills.Where(x => x.Booking_Id ==model.Booking_Id && x.IsCancelled == false).FirstOrDefault();// Mapper.Map<VendorBill>(model);
                    vBill.Company_Id = model.Company_Id;
                    vBill.UpdatedBy = user.Id;
                    vBill.UpdateDate = cr.GetISTDate();
                    vBill.DsDate = cr.GetISTDate();

                    vBill.JourneyStartDate = model.JourneyStartDate;
                    vBill.JourneyClosingDate = model.JourneyClosingDate;

                    vBill.StartKms = model.StartKms;
                    vBill.EndKms = model.EndKms;
                    vBill.TotalKms = model.TotalKms;
                    vBill.ExtraKms = model.ExtraKms;
                    vBill.ExtraKmsCharge = model.ExtraKmsCharge;
                    vBill.ExtraKmsRate = model.ExtraKmsRate;

                    vBill.StartHour = model.StartHour;
                    vBill.EndHour = model.EndHour;
                    vBill.TotalHrs = model.TotalHrs;
                    vBill.ExtraHrs = model.ExtraHrs;
                    vBill.ExtraHrsCharge = model.ExtraHrsCharge;
                    vBill.ExtraHrsRate = model.ExtraHrsRate;

                    vBill.TotalNight = model.TotalNight;                    
                    vBill.VendorCommision = model.VendorCommision;                    
                    vBill.NightCharge = model.NightCharge;
                    vBill.MiscCharge = model.MiscCharge;
                    vBill.ParkCharge = model.ParkCharge;
                    vBill.TollCharge = model.TollCharge;
                    vBill.MCD = model.MCD;


                    vBill.FuelEfficiency = model.FuelEfficiency;                   
                    vBill.DiscountAmount = model.DiscountAmount;
                    vBill.NetAmount = model.NetAmount;
                    vBill.Total = model.Total;
                    vBill.InterStateCharge = model.InterStateCharge;
                    //calcualte package
                    ////find vendor package
                    var vendor_package = ent.VendorPersonalPackages.FirstOrDefault(a => a.Vendor_Id == vendor.Id && a.VehicleModel_Id == booking.VehicleModel_Id && a.RentalType_Id == booking.RentalType);
                    if (model.CommisionType != "Percent"&& model.CommisionType != "None" && vendor_package == null)
                    //if (vendor_package == null)
                    {
                        throw new FileNotFoundException("Vendor personal package does not found");
                    }
                    else
                    {
                        if (model.CommisionType != "Percent")
                        {
                            vBill.ExtraHrsRate = model.ExtraHrsRate;
                            vBill.ExtraKmsRate = model.ExtraKmsRate;
                            vBill.NightRate = model.NightRate;

                            vBill.MinHrs = model.MinHrs;
                            if (model.PackageType == "monthly")
                            {
                                vBill.MinKm = model.MinKm / booking.DaysInMonth;
                            }
                            else
                            {
                                vBill.MinKm = model.MinKm;
                            }
                        }
                    }
                    //// calculate fare and TA for outstation package
                    if (model.PackageType == "outstation")
                    {
                        double fre = 0;
                        double otChrgeRate = 0;
                        if (model.CommisionType == "Percent")
                        {
                            fre = model.Fare * (100 - model.VendorCommision) / 100;
                        }
                        else
                        {
                            fre = model.Fare;
                        }
                        otChrgeRate = model.OutStationChargeRate;
                        vBill.Fare = fre;
                        vBill.OutStationDays = model.OutStationDays;
                        vBill.OutStationCharge = otChrgeRate * model.OutStationDays;
                    }
                    else
                    {
                        if (model.CommisionType == "Percent")
                        {
                            vBill.Fare = model.Fare * (100 - model.VendorCommision) / 100;
                        }
                        else
                        {
                            if (packageArray.Contains(model.PackageType))
                            {
                                vBill.Fare = Math.Round(vendor_package.Fare / booking.DaysInMonth, 2);
                            }
                            else
                            {
                                vBill.Fare = model.Fare;
                            }
                        }
                    }
                    

                    //updated by bhupesh
                   
                    if (vBill.ExtraKms > 0)
                    {
                        double extraKmsChargeB = vBill.ExtraKms * model.ExtraKmsRate;
                        if (model.CommisionType == "Percent")
                        {
                            vBill.ExtraKmsCharge = Convert.ToDouble(extraKmsChargeB * (100 - model.VendorCommision) / 100);
                        }
                        vBill.ExtraKmsRate = model.ExtraKmsRate;
                    }
                    //updated by bhupesh
                    if (vBill.ExtraHrs > 0)
                    {
                        double extraHrsChargeB = vBill.ExtraHrs * model.ExtraHrsRate;
                        if (model.CommisionType == "Percent")
                        {
                            vBill.ExtraHrsCharge = Convert.ToDouble(extraHrsChargeB * (100 - model.VendorCommision) / 100);
                        }
                        vBill.ExtraHrsRate = model.ExtraHrsRate;
                    }
                    //***End

                    if (vBill.TotalNight > 0)
                    {
                        vBill.NightCharge = vBill.TotalNight * vBill.NightRate;
                    }

                    double taxableVendorAmount = vBill.Fare + vBill.ExtraKmsCharge +
                        vBill.ExtraHrsCharge + vBill.NightCharge + vBill.StateCharge
                        + vBill.ParkCharge + vBill.FuelCharge + vBill.OutStationCharge
                      + vBill.MCD + vBill.TollCharge + vBill.InterStateCharge + vBill.MiscCharge;

                    vBill.NetAmount = taxableVendorAmount;
                    double discountAmt = 0;
                    if (model.DiscountAmount > 0)
                    {
                        if (model.CommisionType == "Percent")
                        {
                            discountAmt = Convert.ToDouble(model.DiscountAmount * (100 - model.VendorCommision) / 100);
                        }

                    }
                    vBill.DiscountAmount = discountAmt;
                    double gst = Math.Round(((taxableVendorAmount - discountAmt) * taxInPercent) / 100, 2);
                    //vBill.Total = taxableVendorAmount + vBill.TollCharge + vBill.InterStateCharge;
                    vBill.Total = taxableVendorAmount - discountAmt + gst;
                    if (vendor.StateMaster_Id == model.StateGstin_Id)
                    {
                        vBill.CgstPercent = Math.Round(taxInPercent / 2, 2);
                        vBill.SgstPercent = Math.Round(taxInPercent / 2, 2);
                        vBill.SGST = Math.Round(gst / 2, 2);
                        vBill.CGST = Math.Round(gst / 2, 2);
                    }
                    else
                    {
                        vBill.IgstPercent = Math.Round(taxInPercent, 2);
                        vBill.IGST = Math.Round(gst, 2);
                    }
                    //  vBill.Total += gst;
                    var vdsNo = "VRCV100";
                    var lRecord = ent.VendorBills.OrderByDescending(a => a.Id).FirstOrDefault();
                    if (lRecord != null)
                    {
                        var len = lRecord.DsNo.Length;
                        var iPart = lRecord.DsNo.Substring(4, len - 4);
                        var next = int.Parse(iPart) + 1;
                        vdsNo = "VRCV" + next;
                    }
                    vBill.DsNo = vdsNo;
                    vBill.VisitedPlace = model.VisitedPlace;
                    if (model.MinKm == 0)
                    {
                        vBill.ExtraKms = 0;
                    }
                    if (model.MinHrs == 0)
                    {
                        vBill.ExtraHrs = 0;
                    }
                    ent.Entry(vBill).State = System.Data.Entity.EntityState.Modified;
                    ent.SaveChanges();
                   
                    //vendor billing end
                    var log = new Log
                    {
                        Booking_Id = booking.Id,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "Booking with Id " + booking.BookingId + " and Ds No.  " + corporateBill.DsNo + " is Edit by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                    #endregion
                    tran.Commit();
                    // ViewBag["msg"] = "Record has updated.";
                    //if (booking.BookingType == "nrd")
                    //{ return RedirectToAction("CorporateInvoiceList", new { isNrg = true, menuId = model.MenuId }); }
                    //else
                    //{ return RedirectToAction("CorporateInvoiceList", new { isNrg = false, menuId = model.MenuId }); }
                    var urlArray = previousUrl.Split(new Char[] { '/', '?' });
                    string returnURL = String.Empty;
                    foreach (var item in urlArray)
                    {
                        if (item == "CorporateInvoiceListBypType")
                        {
                            returnURL = "CorporateInvoiceListBypType";
                            break;
                        }
                    }
                    if (returnURL == "CorporateInvoiceListBypType")
                    {
                        if (booking.BookingType == "regular")
                        {
                            if (pType == 1 || pType == 2 || pType == 3)
                            {
                                return RedirectToAction("CorporateInvoiceListBypType", new { isNrg = false, page = model.Page, pType = 2, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            }
                            else
                            {
                                return RedirectToAction("CorporateInvoiceListBypType", new {isNrg = false, page = model.Page, pType = pType, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                            }
                        }
                        else
                        {
                            return RedirectToAction("CorporateInvoiceListBypType", new { isNrg = true, page = model.Page, pType = 0, bookingType = bookingType, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                        }
                    }
                    else
                    {
                        return RedirectToAction("CorporateInvoiceList", new { page = model.Page, menuId = model.MenuId, sDate = model.sDate, eDate = model.eDate });
                    }

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                   // TempData["msg"] = ex.Message;
                    return View(model);
                    //return RedirectToAction("CloseBooking", new { id = model.Booking_Id, page = model.Page, menuId = model.MenuId, term = model.Term });
                }
            }
        }


       //*********** EditBooking
        public ActionResult EditBooking(int id, int bookingStatus = 0, string term = "", int page = 1, int menuId = 7, bool fromUnbilled = false)
        {
            var data = ent.Bookings.Find(id);
            ViewBag.priviousURL = System.Web.HttpContext.Current.Request.UrlReferrer;
            var model = Mapper.Map<BookingDTO>(data);
            model.PreviousBookingType = data.BookingType;
            model.OrganizationList = new SelectList(ent.Customers.Where(x=>x.IsActive==true).ToList().OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName", data.Client_Id);
            model.StateList = new SelectList(ent.StateMasters.OrderBy(e => e.StateName).ToList(), "Id", "StateName", data.NrdStateId);
            model.Cities = new SelectList(ent.CityMasters.OrderBy(e => e.CityName).ToList(), "Id", "CityName");
            var city = ent.CityMasters.Find(data.City_Id);
            if (city != null)
                model.CityName = city.CityName;
            List<VehicleModel> vehicles = new List<VehicleModel>();
            if (data.BookingType == "regular")
            {
                int clientId = data.Client_Id;
                // var client = ent.Customers.Find(data.Client_Id);

                //find parent client 
                //if (client.ParentCustomer_Id != null)
                //{
                //    var parentClient = ent.Customers.Find(client.ParentCustomer_Id);
                //    if (parentClient != null)
                //    {
                //        clientId = parentClient.Id;
                //    }
                //}
                vehicles = GetVehicleModelListFromPackage(clientId).ToList();
            }
            else
            {
                vehicles = GetVehicleModelListFromNRGPackage().ToList();
            }
            model.VehicleModels = new SelectList(vehicles, "Id", "ModelName", data.VehicleModel_Id);
            model.PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", data.PackageType_Id);
            model.RentalList = new SelectList(ent.RentalTypes.Where(a => a.PackageType_Id == data.PackageType_Id).ToList(), "Id", "RentalTypeName", data.RentalType);
            //ViewBag.menuId = menuId;
            model.MenuId = menuId;
            model.Term = term;
            model.Page = page;
            //model.sDate = sDate;
            //model.eDate = eDate;
            model.bStatus = bookingStatus;
            model.FromUnbilled = fromUnbilled;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditBooking(BookingDTO model)
        {
            string isNrg = string.Empty;
            if (model.BookingType == "nrd")
            {
                ModelState.Remove("Client_Id");
            }
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", model.NrdStateId);
            model.Cities = new SelectList(ent.CityMasters.ToList(), "Id", "CityName");
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName", model.Client_Id);
            List<VehicleModel> vehicles = new List<VehicleModel>();
            if (model.PreviousBookingType.ToLower() != model.BookingType.ToLower())
            {
                model.BookingId = cr.GenerateBookingId(model.BookingType);

            }
            if (model.BookingType == "regular")
            {
                int clientId = model.Client_Id;
                var client = ent.Customers.Find(clientId);
                model.NrdBookingMode = null;
                //model.NrdStateId = null;
                model.NrgType = null;
                isNrg = "false";
                //find parent client 
                if (client.ParentCustomer_Id != null)
                {
                    var parentClient = ent.Customers.Find(client.ParentCustomer_Id);
                    if (parentClient != null)
                    {
                        clientId = parentClient.Id;
                    }
                }
                vehicles = GetVehicleModelListFromPackage(clientId).ToList();
            }
            else
            {
                isNrg = "true";
                vehicles = GetVehicleModelListFromNRGPackage().ToList();
            }
            //  model.VehicleModels = new SelectList(vehicles, "Id", "ModelName", model.VehicleModel_Id);
            model.PacakgeTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", model.PackageType_Id);
            //  model.RentalList = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName", model.RentalType);
            // Create Rental List
            if (model.PackageType_Id > 0)
            {
                model.RentalList = new SelectList(ent.RentalTypes.Where(a => a.PackageType_Id == model.PackageType_Id).ToList(), "Id", "RentalTypeName", model.RentalType);
            }
            else
            {
                model.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            // Create Vehicle Model
            if (model.BookingType == "nrd" && model.NrgType == "corporate" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else if (model.BookingType == "regular" && model.Client_Id > 0)
            {
                var data = GetVehicleModelListFromPackage(model.Client_Id);
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else if (model.BookingType == "nrd")
            {
                var data = GetVehicleModelListFromNRGPackage();
                model.VehicleModels = new SelectList(data.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            }
            else
            {
                model.VehicleModels = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            //Checking validation
            if (!ModelState.IsValid)
                return View(model);

            //rental type
            var rentalType = ent.RentalTypes.Find(model.RentalType);
            //package type
            var packageType = ent.PackageTypes.Find(rentalType.PackageType_Id);
            // check whether package exist or not
            if (model.BookingType == "nrd" && model.NrgType == "walkin")
            {
                var nrgPackage = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                isNrg = "true";
                if (nrgPackage == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && nrgPackage.MinHrs == 4)
                {
                    var nrgPackage8Hour = ent.CorporatePackages.FirstOrDefault(a => a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (nrgPackage8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }

            }
            else
            {
                // start:  fetching parent id for sister company
                int compareClientId = model.Client_Id;
                var currentClient = ent.Customers.AsNoTracking().FirstOrDefault(a => a.Id == model.Client_Id);
                if (currentClient != null)
                {
                    if (currentClient.ParentCustomer_Id != null)
                        compareClientId = (int)currentClient.ParentCustomer_Id;
                }
                // End: fetching parent id for sister company
                var package = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.RentalType_Id == model.RentalType);
                if (package == null)
                {
                    TempData["msg"] = "package does not found";
                    return View(model);
                }
                if (packageType.PType == "Local Run" && package.MinHrs == 4)
                {
                    var package8Hour = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == compareClientId && a.VehicleModel_Id == model.VehicleModel_Id && a.MinHrs == 8);
                    if (package8Hour == null)
                    {
                        TempData["msg"] = "package for 8/80 does not found";
                        return View(model);
                    }
                }
            }

            using (var tran = ent.Database.BeginTransaction())
            {
                var user = ent.UserLogins.Find(cr.GetUserDetailId());
                try
                {
                    model.CompanyName = model.BookingType == "nrd" && model.NrgType == "walkin" ? "NRD" : ent.Customers.Find(model.Client_Id).CompanyName;
                    model.FinYear = Convert.ToInt32(model.BookingId.Substring(3, 2));
                    model.UpdatedBy = user.Id;
                    model.UpdateDate = cr.GetISTDate();
                    model.BookingConfirmFile = null;
                    if (model.BookingType == "nrd" && model.NrgType == "walkin")
                    {
                        model.Client_Id = 0;
                    }
                    var vendor = Mapper.Map<Booking>(model);
                    ent.Entry(vendor).State = System.Data.Entity.EntityState.Modified;

                    var log = new Log
                    {
                        Booking_Id = model.Id,
                        UpdateDate = cr.GetISTDate(),
                        UserLogin_Id = user.Id,
                        Description = "Booking with Id " + model.BookingId + " is Updated by " + user.Email + " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                    };
                    ent.Logs.Add(log);
                    ent.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("EditBookingBill", new { id = model.Id, page = 1, isNrg = isNrg, menuId = 7,term=model.Term });
                   
                    //if (model.FromUnbilled)
                    //    return RedirectToAction("UnbilledBooking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, menuId = model.MenuId });
                    //else
                    //    return RedirectToAction("ShowBooking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, bookingStatus = model.BookingStatus, menuId = model.MenuId });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Server error";
                    return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });
                }
            }
        }


        public ActionResult TaxInvoceList(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0, bool export = false)
        {
            TaxInvoiceFileListVM lst = new TaxInvoiceFileListVM();
            lst.SrNo = 0;
            int taxId = 0;
            string query = @"execute uspTaxInvoicelist @taxInvId";
            var data = ent.Database.SqlQuery<TaxInvoiceFileDetails>(query,
            new SqlParameter("@taxInvId", taxId)).ToList();
            lst.TaxInvoiceFileLists = data;
            if (export)
            {
                var tlist= (from d in data
                        select new TaxInvoiceListExcel
                        {
                            CompanyName = d.CompanyName,
                            City = d.City,
                            TaxInviceNo = d.TaxInviceNo,
                            TaxInvoiceDate = d.TaxInvoiceDate,
                            PONo = d.PONo,
                            PODate = d.PODate,
                            Amount = d.Amount,
                            CGST = d.CGST,
                            CGST_per = d.CGST_per,
                            SGST = d.SGST,
                            SGST_per = d.SGST_per,
                            IGST = d.IGST,
                            IGST_per = d.IGST_per,
                            GrandTotal = d.GrandTotal
                        }).ToList();
                var grid = new GridView();
                grid.DataSource = tlist;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =txtInvoiceList.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            ViewBag.menuId = menuId;
            return View(lst);
        }

        public ActionResult TaxInvoiceMerge(int id)
        {
            TaxInvoiceFileListVM lst = new TaxInvoiceFileListVM();           
            string query = @"execute uspTaxInvoicelist @taxInvId";
            var data = ent.Database.SqlQuery<TaxInvoiceFileDetails>(query,
            new SqlParameter("@taxInvId", id)).ToList();
            lst.TaxInvoiceFileLists = data;
            if(data!=null && data.Count>0)
            {
                int st = data[0].Company_Id;// ent.Customers.Where(x => x.Id == data[0].Company_Id).FirstOrDefault().State_Id;
                //int gststateId=data[0].StateGstin_Id;
                int gststateId = ent.Customers.Where(x => x.Id == st).FirstOrDefault().State_Id;
                lst.StateGSTNBankdetails = ent.StateWiseGSTINs.Where(x => x.State_Id == gststateId).FirstOrDefault();
            }
           
            return View(lst);           
        }
        //corporate Tax invoice generate and send method
        public ActionResult GeneratePdfTaxInvoiceMerge(int id)
        {
            TaxInvoiceFileListVM lst = new TaxInvoiceFileListVM();
            string query = @"execute uspTaxInvoicelist @taxInvId";
            var data = ent.Database.SqlQuery<TaxInvoiceFileDetails>(query,
            new SqlParameter("@taxInvId", id)).ToList();
            lst.TaxInvoiceFileLists = data;
            var cptaxinv = ent.CorporateInvoiceFiles.Where(x=>x.Id==id).FirstOrDefault();
            if (string.IsNullOrEmpty(data[0].InvoiceFile))
            {
                var dt = DateTime.Now;

                string filename = data[0].TaxInviceNo + "-" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + ".pdf";
                string path = Server.MapPath("/Files/") + filename;

                cptaxinv.InvoiceFile = filename;
                ent.SaveChanges();
               
                return new ActionAsPdf("TaxInvoiceMerge", new { id = data[0].Id, isPdf = true }) { FileName = filename, SaveOnServerPath = path };
            }
            return Content("already added");
        }
        public ActionResult MIS(int Id)
        {
            string query = @"execute GetMISVyTaxInvNo @taxInvId,@isExport";
            var data = ent.Database.SqlQuery<CIData>(query,
                new SqlParameter("@taxInvId", Id),
                new SqlParameter("@isExport",false)
                ).ToList();
            return View(data);
        }

        //********************Previous Canceled Bill details
        private IEnumerable<CorporateBill> GetPreCorporateBillByBookingID(int bookingId)
        {
            string cpbQuery = @"select top 1 * from [dbo].[CorporateBill] where Booking_Id= '"+ bookingId + "' and IsCancelled=1  order by UpdateDate desc";
            var cpbill = ent.Database.SqlQuery<CorporateBill>(cpbQuery).ToList();
            return cpbill;
        }

        private IEnumerable<VendorBill> GetPreVendorBillByBookingID(int bookingId)
        {
            string cpbQuery = @"select top 1 * from [dbo].[VendorBill] where Booking_Id= '" + bookingId + "' and IsCancelled=1  order by UpdateDate desc";
            var cpbill = ent.Database.SqlQuery<VendorBill>(cpbQuery).ToList();
            return cpbill;
        }

        public string getAutoGenratedTaxInvoiceNo()
        {
            string taxInvNo = String.Empty;
            string cpbQuery = "exec getAutoInvNo";//@"select max(LEFT(InvoiceNumber,CHARINDEX('-', InvoiceNumber, 0)-1))+1 from CorporateInvoiceFile";
            taxInvNo = ent.Database.SqlQuery<string>(cpbQuery).FirstOrDefault().ToString();
            return taxInvNo;   
        }
        public JsonResult getAutoNPreviousTaxInvoiceNo()
        {           
            AutoTaxInvoice invObj = new VardaanCab.Models.ViewModels.AutoTaxInvoice();
            string cpbQuery = "exec getAutoGenratedInvNo";
            invObj = ent.Database.SqlQuery<AutoTaxInvoice>(cpbQuery).FirstOrDefault();
            return Json(invObj, JsonRequestBehavior.AllowGet);           
        }

        [HttpPost]
        public ActionResult CancelTaxInvoice(string TaxInvid, string Remark)
        {
            int id =Convert.ToInt32(TaxInvid);
            string remark = Remark;
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var taxInv = ent.CorporateInvoiceFiles.Where(x => x.Id == id).FirstOrDefault();
                    taxInv.IsCancel= true;
                    taxInv.Remark = Remark;                  
                    ent.SaveChanges();                 
                    foreach (var item in ent.CorporateInvoiceFileDetails.Where(x=>x.CorporateInvoiceFile_Id==id).ToList())
                    {
                        var ci = ent.CorporateBills.Find(item.CorporateBill_Id);
                        ci.IsBilled = false;                      
                    }
                    ent.SaveChanges();
                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("Server Error");
                }
            }

        }

        public long getAutoProformaeNo()
        {
            long propormaNo =0;
            string cpbQuery = "exec getAutoProformaNo";
            propormaNo = ent.Database.SqlQuery<long>(cpbQuery).FirstOrDefault();
            return propormaNo;
        }
        //add booking id for tax invoice
        [HttpPost]
        public ActionResult addBookingId(string bookingNo,bool chkValue)
        {
            try
            {           
            int count = 0;
            bool ischecked = chkValue;
            string id = bookingNo;
            List<tempUserBooking> lst = new List<tempUserBooking>();           
                string usern = Convert.ToString(Session["uEmail"]);
                string sesid = Convert.ToString(Session.SessionID);
                if (ent.tempUserBookings.Any(x => x.Username ==usern&&x.SessionId==sesid))
                {
                    lst = ent.tempUserBookings.Where(x => x.Username == usern && x.SessionId == sesid).ToList();
                    if (lst.Count > 0)
                    {
                        if (chkValue)
                        {
                                try
                                {                                   
                                    //************add in database
                                    tempUserBooking obd = new tempUserBooking();
                                    obd.BookingId = Convert.ToInt32(bookingNo);
                                    obd.Username = Convert.ToString(Session["uEmail"]);
                                    obd.SessionId = Session.SessionID;
                                    obd.UpdateDate = DateTime.Now;
                                    ent.tempUserBookings.Add(obd);
                                    ent.SaveChanges();
                                    lst.Add(obd);
                                }
                                catch (Exception ex1)
                                {
                                    throw;
                                }
                          
                        }
                        else
                        {
                            int bookid = Convert.ToInt32(bookingNo);                           
                            //remove from db
                            if (ent.tempUserBookings.Any(x => x.BookingId == bookid))
                            {
                                tempUserBooking obd = ent.tempUserBookings.Where(x => x.BookingId == bookid).FirstOrDefault();
                                ent.tempUserBookings.Remove(obd);
                                ent.SaveChanges();
                                lst.Remove(obd);
                            }
                        }
                    }
                    else
                    {
                            try
                            {   
                                // add on db
                                tempUserBooking obd = new tempUserBooking();
                                obd.BookingId = Convert.ToInt32(bookingNo);
                                obd.Username = Convert.ToString(Session["uEmail"]);
                                obd.SessionId = Session.SessionID;
                                obd.UpdateDate = DateTime.Now;
                                ent.tempUserBookings.Add(obd);
                                ent.SaveChanges();
                                lst.Add(obd);
                            }
                            catch (Exception ex2)
                            {
                                throw;
                            }
                        }
                }           
                else
                {
                    try
                    {                       
                        // add on db
                        tempUserBooking obd = new tempUserBooking();
                        obd.BookingId = Convert.ToInt32(bookingNo);
                        obd.Username = Convert.ToString(Session["uEmail"]);
                        obd.SessionId = Session.SessionID;
                        obd.UpdateDate = DateTime.Now;
                        ent.tempUserBookings.Add(obd);
                        ent.SaveChanges();
                        lst.Add(obd);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                
                }
            count = lst.Count;
            return Content(count.ToString());
            }
            catch (Exception ex)
            {
                return Content("0");
            }
        }

        public ActionResult BookingCart(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "",string routeNo = "", string pType = "", int menuId = 0)
        {
            bool nrg = Convert.ToBoolean(isNrg);
            string cityId = string.Empty;
            string routeno = String.Empty;
            routeno = routeNo;
            string userName=Convert.ToString(Session["uEmail"]);
            string sess = Convert.ToString(Session.SessionID);
            //string query = @"execute GetBookingCartData @isNrg,@term,@sDate,@eDate,@cityId,@routeNo";
            string query = @"execute GetBookingCartDataTemp @isNrg,@term,@sDate,@eDate,@cityId,@routeNo,@username,@sessId,@pType"; 
            var data = ent.Database.SqlQuery<BookingCartVM>(query,
                new SqlParameter("@isNrg", nrg),
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
                new SqlParameter("@routeNo", String.IsNullOrEmpty(routeno) ? (object)DBNull.Value : Convert.ToInt32(routeno)),
                new SqlParameter("@username", String.IsNullOrEmpty(userName) ? (object)DBNull.Value : userName),
                new SqlParameter("@sessId", String.IsNullOrEmpty(sess) ? (object)DBNull.Value : sess),
                 new SqlParameter("@pType", String.IsNullOrEmpty(pType) ? (object)DBNull.Value : Convert.ToInt32(pType))
                ).ToList();
            var total = data.Count;
            ViewBag.total = total;
            if(total > 0)
            { return View(data); }
            else
            {
                return RedirectToAction("CorporateInvoiceList", new { sDate = sDate, eDate = eDate, term=term, });
            }
        }
        [HttpPost]
        public int checkOneCompanySelected(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", string routeNo = "")
        {
            bool nrg = Convert.ToBoolean(isNrg);
            string cityId = string.Empty;
            string cpbQuery = "exec checkOneCompanyBookingSelected @isNrg,@term,@sDate,@eDate,@cityId,@routeNo";            
            var total = ent.Database.SqlQuery<int>(cpbQuery,
               new SqlParameter("@isNrg", nrg),
               new SqlParameter("@term", term),
               new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
               new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),             
               new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
               new SqlParameter("@routeNo", String.IsNullOrEmpty(routeNo) ? (object)DBNull.Value : Convert.ToInt32(routeNo))
               ).FirstOrDefault();
            return total;
        }

        [HttpPost]
        public int checkOneCompanySelectedByPType(DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", string routeNo = "", string pType ="")
        {
            bool nrg = Convert.ToBoolean(isNrg);
            string cityId = string.Empty;
            string cpbQuery = "exec checkOneCompanyBookingSelectedByPtype @isNrg,@term,@sDate,@eDate,@cityId,@routeNo,@pType";
            var total = ent.Database.SqlQuery<int>(cpbQuery,
               new SqlParameter("@isNrg", nrg),
               new SqlParameter("@term", term),
               new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
               new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
               new SqlParameter("@cityId", String.IsNullOrEmpty(cityId) ? (object)DBNull.Value : Convert.ToInt32(cityId)),
               new SqlParameter("@routeNo", String.IsNullOrEmpty(routeNo) ? (object)DBNull.Value : Convert.ToInt32(routeNo)),
                new SqlParameter("@pType", String.IsNullOrEmpty(pType) ? (object)DBNull.Value : Convert.ToInt32(pType))
               ).FirstOrDefault();
            return total;
        }
    }
}