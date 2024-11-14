using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Data.SqlClient;
using AutoMapper;
using Rotativa;
using System.IO;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Areas.Clint.Controllers
{
    public class CabBookingController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository cr = new CommonRepository();
        // GET: Clint/CabBooking
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CabBook(int menuId = 0)
        {
            var model = new BookingDTO();
            model.OrganizationList = new SelectList(Enumerable.Empty<SelectListItem>());//new SelectList(ent.Customers.Where(x => x.IsActive == true).ToList().OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.RentalList = new SelectList(Enumerable.Empty<SelectListItem>());
            model.VehicleModels = new SelectList(Enumerable.Empty<SelectListItem>());
            model.StateList = new SelectList(ent.StateMasters.OrderBy(e => e.StateName).ToList(), "Id", "StateName");
            model.PacakgeTypes = new SelectList(ent.PackageTypes.Where(x=>x.Id<4).ToList(), "Id", "PType");
            model.Cities = new SelectList(ent.CityMasters.OrderBy(e => e.CityName).ToList(), "Id", "CityName");
            ViewBag.cuserid = Session["CUID"];
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

            int cuid =Convert.ToInt32(Session["CUID"]);

            model.StateList = new SelectList(ent.StateMasters.OrderBy(a => a.StateName).ToList(), "Id", "StateName");
            model.OrganizationList = new SelectList(ent.Customers.OrderBy(a => a.CompanyName).ToList(), "Id", "CompanyName");
            model.Cities = new SelectList(ent.CityMasters.OrderBy(a => a.CityName).ToList(), "Id", "CityName");
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
            var user = ent.ClientUsers.Find(cuid);
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
                        model.UpdatedBy = cuid;
                        
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
                        //Add client and booking mapping
                        var cbmapping = new ClientUserBookingMapping
                        {
                            CUserId = cuid,
                            BookingId = data.Id
                        };
                        ent.ClientUserBookingMappings.Add(cbmapping);
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
                            model.UpdatedBy = user.Id;
                           
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
                            //Add client and booking mapping
                            var cbmapping = new ClientUserBookingMapping
                            {
                            CUserId= cuid,
                            BookingId=data.Id
                            };
                            ent.ClientUserBookingMappings.Add(cbmapping);
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

        public ActionResult ShowBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", string bookingStatus = "2", int menuId = 0,bool export = false)
        {
            int clientuserid =Convert.ToInt32(Session["CUID"]);
            ViewBag.pendingCat = clientuserid;
            //ViewBag.pendingCat = pendingBookingType;// int pbookingCat = 2
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
            string query = @"execute GetBookingByClintUser @term,@sDate,@eDate,@page,@pageSize,@isExport,@clintUID";

            var data = ent.Database.SqlQuery<BookingDTO>(query,
                
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", export),//Add new parameter
                new SqlParameter("@clintUID", clientuserid)
                ).ToList();
          

            //get count for  pagination
            string query2 = string.Format("execute GetBookingCountByClientUser @term,@sDate,@eDate,@page,@pageSize,@isExport,@clintUID");
            var total = ent.Database.SqlQuery<int>(query2,                
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true),//Add new parameter
                new SqlParameter("@clintUID", clientuserid)
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
                              RouteNo = d.BookingNo
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
        public ActionResult PendingBooking(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", string bookingStatus = "2", int menuId = 0, bool export = false)
        {
            int clientuserid = Convert.ToInt32(Session["CUID"]);
            ViewBag.pendingCat = clientuserid;
            //ViewBag.pendingCat = pendingBookingType;// int pbookingCat = 2
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
            string query = @"execute GetPendingBookingByClintUser @term,@sDate,@eDate,@page,@pageSize,@isExport,@clintUID";

            var data = ent.Database.SqlQuery<BookingDTO>(query,

                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", export),//Add new parameter
                new SqlParameter("@clintUID", clientuserid)
                ).ToList();


            //get count for  pagination
            string query2 = string.Format("execute GetPendingBookingCountByClientUser @term,@sDate,@eDate,@page,@pageSize,@isExport,@clintUID");
            var total = ent.Database.SqlQuery<int>(query2,
                new SqlParameter("@term", term),
                new SqlParameter("@sDate", sDate == null ? (object)DBNull.Value : sDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@eDate", eDate == null ? (object)DBNull.Value : eDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@page", page),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@isExport", true),//Add new parameter
                new SqlParameter("@clintUID", clientuserid)
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
                              RouteNo = d.BookingNo
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
        public ActionResult TaxInvoice(int id, DateTime? sDate, DateTime? eDate, string isNrg = "false", string term = "", int page = 1, bool export = false, string Display = "d", bool IsTaxInvoice = false, int menuId = 0, bool isPdf = false)
        {
            var data = GetTaxInvoiceData(id, IsTaxInvoice);
            string previousUrl = string.Empty;
            previousUrl = Convert.ToString(System.Web.HttpContext.Current.Request.UrlReferrer);
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
        public TaxInvoiceViewModel GetTaxInvoiceData(int id, bool IsTaxInvoice)
        {
            int cbillid = ent.CorporateBills.Where(x => x.Booking_Id == id &&(x.IsCancelled==false)).Select(x => x.Id).FirstOrDefault();

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
                        where cbill.Id == cbillid
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
                            FromCompanyName = swg.CompanyName,
                            FromEmail = swg.EmailId,
                            FromAddress = swg.Address,
                            FromGSTIN = swg.Gstin,
                            FromStateName = fromState.StateName,

                            FromBankName = swg.BankName,
                            FromAC_No = swg.AC_No,
                            FromBranchAddress = swg.BranchAddress,
                            FromIFS_Code = swg.IFS_Code,
                            CompanyRegAdd = swg.CompanyRegAdd,
                            CompanyRegState = swg.CompanyRegState,
                            BilledAddress = client.FullAddress,
                            CompanyName = client != null ? client.CompanyName : booking.CustomerName,
                            CompanyGSTIN = client.GSTIN,
                            // PlaceOfService = ss != null ? ss.StateName : nrdServiceState != null ? nrdServiceState.StateName : "",
                            PlaceOfService = ss != null ? ent.StateMasters.Where(x => x.StateCode == client.GSTIN.Substring(0, 2)).FirstOrDefault().StateName : nrdServiceState != null ? nrdServiceState.StateName : "",
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
                            MiscCharge = cbill.MiscCharge,
                            IsNrg = cbill.IsNrg ? "True" : "False",
                            ProformNo = cbill.ProformNo
                        }).FirstOrDefault();

            var packageArray = new string[] { "monthly", "monthly-route", "monthly-trip" };
            data.ShouldRoundOff = !packageArray.Contains(data.PackageTypeName);
            return data;
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
                return Content("error" + ex.Message);
            }
            //return View(data);
        }

        public ActionResult TaxInvoceList(DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0, bool export = false)
        {
            int clientuserid = Convert.ToInt32(Session["CUID"]);
            TaxInvoiceFileListVM lst = new TaxInvoiceFileListVM();
            lst.SrNo = 0;
            int taxId = 0;
            string query = @"execute uspTaxInvoicelistbyClintUser @taxInvId,@clintUID";
            var data = ent.Database.SqlQuery<TaxInvoiceFileDetails>(query,
            new SqlParameter("@taxInvId", taxId),
             new SqlParameter("@clintUID", clientuserid)).ToList();
            lst.TaxInvoiceFileLists = data;
            if (export)
            {
                var tlist = (from d in data
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

        public ActionResult MIS(int Id)
        {
            string query = @"execute GetMISVyTaxInvNo @taxInvId,@isExport";
            var data = ent.Database.SqlQuery<CIData>(query,
                new SqlParameter("@taxInvId", Id),
                new SqlParameter("@isExport", false)
                ).ToList();
            return View(data);
        }

        public ActionResult TaxInvoiceMerge(int id)
        {
            TaxInvoiceFileListVM lst = new TaxInvoiceFileListVM();
            string query = @"execute uspTaxInvoicelist @taxInvId";
            var data = ent.Database.SqlQuery<TaxInvoiceFileDetails>(query,
            new SqlParameter("@taxInvId", id)).ToList();
            lst.TaxInvoiceFileLists = data;
            if (data != null && data.Count > 0)
            {
                int gststateId = data[0].StateGstin_Id;
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
            var cptaxinv = ent.CorporateInvoiceFiles.Where(x => x.Id == id).FirstOrDefault();
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
    }
}