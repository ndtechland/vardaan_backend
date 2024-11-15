 using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();

        public ActionResult Sale(DateTime? sDate, DateTime? eDate, string term = "",bool export=false,int menuId=0)
        {
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(term))
                term = term.Trim().ToLower();
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            string startDate = sDate.Value.ToString("yyyy-MM-dd");
            string endDate = eDate.Value.ToString("yyyy-MM-dd");
            var query = @"select c.Id as CompanyId,IsNull(c.CompanyName,'walkin') as CompanyName,sub.Total as Amount from 
(select cb.Company_Id,round(sum(cb.total),2) as total from CorporateBill cb
join booking b on cb.Booking_Id=b.Id
left join PackageType pt on b.PackageType_Id = pt.Id
where cb.Iscancelled=0 and b.BookingType='regular' and 
convert(date,cb.JourneyClosingDate) between '" + startDate+"' and '"+endDate+ "' group by cb.Company_Id) sub left join Customer c on sub.Company_Id = c.Id order by CompanyName";
            var data = ent.Database.SqlQuery<SaleReportData>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data=data.Where(a => a.CompanyName.ToLower().Contains(term.ToLower())).ToList();
            }
            if (export)
            {
                var grid = new GridView();
                grid.DataSource = data; ;//data.Select(a=> new { a.CompanyName, a.Amount });
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =customer billing report ("+startDate+" to "+endDate+").xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new SaleReportVm();
            rm.SaleData = data;
            rm.Total = data.Sum(a => a.Amount);
            rm.sDate = sDate.Value;
            rm.eDate = eDate.Value;
            rm.Term = term;
            ViewBag.menuId = menuId;
            rm.StartDate = startDate;
            rm.EndDate = endDate;
            return View(rm);
        }

        public ActionResult SaleDetail(int companyId,string sDate,string eDate,int menuId, string term = "")
        {
//            var query = @"select pt.PType as PackageTypeName,rt.RentalTypeName,cb.*  from CorporateBill cb
//join booking b on cb.Booking_Id=b.Id
//left join PackageType pt on b.PackageType_Id = pt.Id
//left join RentalType rt on b.RentalType =rt.Id
//where cb.Iscancelled=0 and b.BookingType='regular' and CONVERT(datetime,dsdate) between '" + sDate+"' and '"+eDate+ "'and cb.Company_Id='"+ companyId + "'";


            var query = @"select  b.CompanyName,
b.BookingId,vm.ModelName,c.VehicleNumber,cb.TaxInvoiceNumber,cb.TaxInvoiceDate,cb.JourneyStartDate,cb.JourneyClosingDate,cb.GuestName,cb.VisitedPlace,pt.PType as PackageTypeName,rt.RentalTypeName,
cb.Fare,cb.ExtraHrs,cb.ExtraHrsRate,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsRate,cb.ExtraKmsCharge,cb.TotalNight,cb.NightRate,cb.NightCharge,cb.OutStationDays,
cb.OutStationChargeRate,cb.OutStationCharge,cb.DiscountAmount,cb.ParkCharge,cb.MCD,
cb.TollCharge,cb.InterStateCharge as StateCharge,cb.MiscCharge,cb.NetAmount as SubTotal,(cb.NetAmount-isnull(cb.DiscountAmount,0)) as NetAmount,CgstPercent,cb.CGST,cb.SgstPercent,cb.SGST,cb.IgstPercent,cb.IGST,cb.Total
from booking b
left join PackageType pt on b.PackageType_Id = pt.Id
left join RentalType rt on b.RentalType =rt.Id
join cab c on b.Cab_Id=c.Id
join CorporateBill cb on b.Id=cb.Booking_Id
join VehicleModel vm on b.VehicleModel_Id=vm.Id
where cb.Iscancelled=0 and b.BookingType='regular' and CONVERT(datetime,cb.JourneyClosingDate) between '" + sDate + "' and '" + eDate + "' and cb.Company_Id='" + companyId + "'";
            
            var data = ent.Database.SqlQuery<CorporateBillReportModel>(query).ToList();
            var model = new SaleDetailReportModel();
            model.MenuId = menuId;
            model.Data = data;
            model.sMonth = Convert.ToDateTime(sDate).ToString("MMMM");
            if (data.Count > 0)
                model.CompanyName = data[0].CompanyName;
            return View(model);
        }

        public ActionResult NRDSale(string nrdType, DateTime? sDate, DateTime? eDate, string term = "", bool export = false, int menuId = 0)
        {
            int pos = nrdType.IndexOf('?');
            if (pos >= 0)
            {
                nrdType = nrdType.Remove(pos, 1);
            }
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(term))
                term = term.Trim().ToLower();
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            string startDate = sDate.Value.ToString("yyyy-MM-dd");
            string endDate = eDate.Value.ToString("yyyy-MM-dd");
            var query = @"select IsNull(c.CompanyName,'walkin') as CompanyName,sub.Company_Id as CompanyId,sub.Total as Amount from 
(select cb.Company_Id,round(sum(cb.total),2) as total from CorporateBill cb
join booking b on cb.Booking_Id=b.Id
where cb.Iscancelled=0 and b.BookingType='nrd'  
and  convert(date,cb.JourneyClosingDate) between '" + startDate + "' and '" + endDate + "' group by cb.Company_Id) sub left join Customer c on sub.Company_Id = c.Id order by CompanyName";
            var data = ent.Database.SqlQuery<SaleReportData>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.CompanyName.ToLower().Contains(term.ToLower())).ToList();
            }
            if (export)
            {
                var grid = new GridView();
                grid.DataSource = data;// data.Select(a => new { a.CompanyName, a.Amount });
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =customer billing report (" + startDate + " to " + endDate + ").xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";

                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new SaleReportVm();
            rm.SaleData = data;
            rm.Total = data.Sum(a => a.Amount);
            rm.sDate = sDate.Value;
            rm.eDate = eDate.Value;
            rm.Term = term;
            rm.NRDType = nrdType;
            ViewBag.menuId = menuId;
            rm.StartDate = startDate;
            rm.EndDate = endDate;
            return View(rm);
        }

        public ActionResult NRDSaleDetail(string nrdType, int companyId, string sDate, string eDate, int menuId)
        {
            var query = @"select  b.CompanyName,
b.BookingId,vm.ModelName,c.VehicleNumber,cb.TaxInvoiceNumber,cb.TaxInvoiceDate,cb.JourneyStartDate,cb.JourneyClosingDate,cb.GuestName,cb.VisitedPlace,pt.PType as PackageTypeName,rt.RentalTypeName,
cb.Fare,cb.ExtraHrs,cb.ExtraHrsRate,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsRate,cb.ExtraKmsCharge,cb.TotalNight,cb.NightRate,cb.NightCharge,cb.OutStationDays,
cb.OutStationChargeRate,cb.OutStationCharge,cb.DiscountAmount,cb.ParkCharge,cb.MCD,
cb.TollCharge,cb.InterStateCharge as StateCharge,cb.MiscCharge,cb.NetAmount as SubTotal,(cb.NetAmount-isnull(cb.DiscountAmount,0)) as NetAmount,CgstPercent,cb.CGST,cb.SgstPercent,cb.SGST,cb.IgstPercent,cb.IGST,cb.Total
from booking b
left join PackageType pt on b.PackageType_Id = pt.Id
left join RentalType rt on b.RentalType =rt.Id
join cab c on b.Cab_Id=c.Id
join CorporateBill cb on b.Id=cb.Booking_Id
join VehicleModel vm on b.VehicleModel_Id=vm.Id
where cb.Iscancelled=0 and b.BookingType='nrd' and CONVERT(datetime,cb.JourneyClosingDate) between '" + sDate + "' and '" + eDate + "'and cb.Company_Id='"+ companyId + "'";
            var data = ent.Database.SqlQuery<CorporateBillReportModel>(query).ToList();
            var model = new SaleDetailReportModel();
            model.MenuId = menuId;
            model.NRDType = nrdType;
            model.Data = data;
            model.sMonth = Convert.ToDateTime(sDate).ToString("MMMM");
            if (data.Count > 0)
                model.CompanyName = data[0].CompanyName;
            return View(model);
        }

        public ActionResult VendorCommisionReport(DateTime? sDate, DateTime? eDate, string term = "", bool export = false, int menuId = 0)
        {
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(term))
                term = term.Trim().ToLower();
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            string startDate = sDate.Value.ToString("yyyy-MM-dd");
            string endDate = eDate.Value.ToString("yyyy-MM-dd");
            string query = @"select v.Id as VendorId,v.CompanyName,t2.Total from vendor v
join (
select c.Vendor_Id,round(Sum(vb.Total),2) as Total from vendorbill vb
join Booking b on
vb.Booking_id =b.id
join cab c on b.Cab_Id=c.Id
join Vendor v on  c.Vendor_Id=v.Id
where vb.iscancelled=0 and convert(date,vb.JourneyClosingDate) between convert(date,'" + startDate + "') and convert(date,'"+ endDate + "')group by Vendor_Id) t2 on v.Id=t2.Vendor_Id order by CompanyName";
            var data = ent.Database.SqlQuery<VendorCommisionData>(query).ToList();
            if(!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.CompanyName.ToLower().Contains(term.ToLower())).ToList();
            }
            if (export)
            {
                var grid = new GridView();
                grid.DataSource = data.Select(a=> new {a.CompanyName,a.Total });
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =vendor billing report (" + startDate + " to " + endDate + ").xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new VendorCommisionReportVm();
            rm.Data = data;
            rm.Total = data.Sum(a => a.Total);
            rm.sDate = sDate.Value;
            rm.eDate = eDate.Value;
            rm.StartDate = startDate;
            rm.EndDate = endDate;
            rm.Term = term;
            ViewBag.menuId = menuId;
            return View(rm);
        }

        public ActionResult VendorSaleDetail(int vendorId, string sDate, string eDate, int menuId)
        {
            var rm = new VendorReportDetailModel();
            //            string query = @"select v.CompanyName as VendorName,t2.* from vendor v
            //join (select vb.*,b.BookingId,c.Vendor_Id from vendorbill vb
            //join Booking b on vb.Booking_id =b.id
            //join cab c on b.Cab_Id=c.Id
            //join Vendor v on  c.Vendor_Id=v.Id
            //where vb.iscancelled=0 and convert(date,dsdate) between convert(date,'" + sDate+"') and convert(date,'"+eDate+"')) t2 on v.Id=t2.Vendor_Id where v.Id="+vendorId;

                string query = @"select v.VendorName,v.CompanyName,b.BookingId,cb.TaxInvoiceNumber,cb.TaxInvoiceDate,c.VehicleNumber,b.CompanyName as ClientName,vb.JourneyStartDate,vb.JourneyClosingDate,
(cb.Fare+cb.ExtraHrsCharge+cb.ExtraKmsCharge) as CommisionableAmt,vb.VendorCommision,(((cb.Fare+cb.ExtraHrsCharge+cb.ExtraKmsCharge)*(100-vb.VendorCommision))/100) as payble,vb.Fare,vb.ExtraHrs,(cb.ExtraHrsCharge*(100-vb.VendorCommision)/100) as ExtraHrsCharge,vb.ExtraKms,(cb.ExtraKmsCharge*(100-vb.VendorCommision)/100) as ExtraKmsCharge,(vb.Fare+vb.ExtraHrsCharge+vb.ExtraKmsCharge) as BasicPayble,vb.TotalNight,vb.NightCharge,vb.OutStationDays,vb.OutStationCharge,vb.ParkCharge,vb.MCD,
vb.TollCharge,vb.InterStateCharge as StateCharge,vb.MiscCharge,vb.NetAmount as SubTotal,vb.DiscountAmount,(vb.NetAmount-vb.DiscountAmount) as NetAmount,vb.CgstPercent,vb.CGST,vb.SgstPercent,vb.SGST,vb.IgstPercent,vb.IGST,vb.Total
 from vendorbill vb
join Booking b on vb.Booking_id =b.id
join CorporateBill cb on b.Id=cb.Booking_Id and cb.iscancelled=0 
join cab c on b.Cab_Id=c.Id
join Vendor v on  c.Vendor_Id=v.Id
where vb.iscancelled=0 and v.Id=" + vendorId+ " and convert(date,cb.JourneyClosingDate) between convert(date,'" + sDate + "') and convert(date,'" + eDate + "')";
            
            rm.Data = ent.Database.SqlQuery<VendorBillModel>(query).ToList();
            rm.MenuId = menuId;
            rm.sMonth = Convert.ToDateTime(sDate).ToString("MMMM");
                if (rm.Data.Count() > 0)
                rm.VendorName = rm.Data.ToList()[0].VendorName;
            return View(rm);
        }

        public ActionResult BookingReport(DateTime? sDate, DateTime? eDate, bool export = false, int menuId = 0)
        {
            var currentDate = DateTime.Now;
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            var bookingStatus = cr.GetBookingStatus().ToList();
            var booking = cr.GetAllBookingWithPType();
            var bookingData = (from b in booking
                               join bs in bookingStatus
                                on b.BookingStatus equals bs.Id
                               where b.BookingDate.Date>=sDate.Value
                               && b.BookingDate.Date <= eDate.Value
                               group new { bs.StatusName, bs.Id,b.BookingDate }
                               by new { bs.Id }
                                into g
                               select new BookingStatusCount
                               {
                                   StatusName = g.FirstOrDefault().StatusName,
                                   Count = g.Count(),
                                   BookingDate=g.FirstOrDefault().BookingDate,
                                   BookingStatusId = g.FirstOrDefault().Id
                               }
                               ).ToList();
            
            var monthlyBooking = booking.Where(a => a.PackageTypeName== "monthly" && a.CreateDate.Date >=sDate.Value 
            && a.CreateDate.Date <= eDate.Value 
            ).Count();
            var monthlyRouteBooking = booking.Where(a => a.PackageTypeName == "monthly-route" && a.CreateDate.Date >= sDate.Value
             && a.CreateDate.Date <= eDate.Value
             ).Count();
            var monthlyRouteTripBooking = booking.Where(a => a.PackageTypeName == "monthly-trip" && a.CreateDate.Date >= sDate.Value
             && a.CreateDate.Date <= eDate.Value
             ).Count();
            bookingData.Add(new BookingStatusCount { BookingStatusId = 0, StatusName = "MonthlyFix", Count = monthlyBooking });
            bookingData.Add(new BookingStatusCount { BookingStatusId = 0, StatusName = "MonthlyRoute", Count = monthlyRouteBooking });
            bookingData.Add(new BookingStatusCount { BookingStatusId = 0, StatusName = "MonthlyTrip", Count = monthlyRouteTripBooking });
            if (export)
            {
                string stDate = sDate.Value.ToString("dd-MMM-yyyy");
                string enDate = eDate.Value.ToString("dd-MMM-yyyy");
                var grid = new GridView();
                grid.DataSource = bookingData.Select(a => new { a.StatusName, a.Count });
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =booking status report (" + stDate + " to " + enDate + ").xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new BookingReportVm();
            rm.BookingStatusData = bookingData;
            rm.TotalBooking = bookingData.Sum(a => a.Count);
            rm.eDate = eDate.Value;
            rm.sDate=sDate.Value;
            ViewBag.menuId = menuId;
            return View(rm);
        }

        //        public ActionResult MonthlyRouteBookingReport( DateTime ? sDate, DateTime ? eDate, int menuId=0, int companyId = 0)
        //        {
        //            var model = new MonthlyBookingDetail();
        //            model.MenuId = menuId;
        //            model.CompanyId = companyId;
        //            model.SDate = sDate;
        //            model.EDate = eDate;
        //            model.Heading = "Monthly Route Booking";
        //            model.CompanyList = new SelectList(ent.Customers.ToList(), "Id", "CompanyName"); 
        //            if (model.SDate != null && model.EDate != null && companyId > 0)
        //            {
        //                var query = @"select vm.Id as VehicleModel_Id,vm.ModelName,b.PickupAddress +' - '+ b.DropAddress as PickupDropAddress,pt.PackageTypeName,cb.*  from CorporateBill cb
        //join booking b on cb.Booking_Id=b.Id
        //join VehicleModel vm on b.VehicleModel_Id=vm.Id
        //left join PackageType pt on b.PackageType_Id = pt.Id 
        //where cb.Company_Id=" + companyId+" and cb.Iscancelled=0 and b.BookingType='regular' and pt.PackageTypeName='monthly-route' and CONVERT(datetime,dsdate) between '" + sDate.Value.ToString("yyyy-MM-dd") + "' and '" + eDate.Value.ToString("yyyy-MM-dd") + "'";
        //                var data = ent.Database.SqlQuery<CorporateBillModel>(query).ToList();
        //                if(data.Count>0)
        //                {
        //                    var record = data.FirstOrDefault();
        //                    var booking = ent.Bookings.Find(record.Booking_Id);
        //                    //var rType = ent.RentalTypes.Find(booking.RentalType);
        //                    var pckg = ent.ClientPackages.FirstOrDefault(a => a.Customer_Id == companyId &&
        //                      a.RentalType_Id == booking.RentalType
        //                      && a.VehicleModel_Id == booking.VehicleModel_Id
        //                    );
        //                    model.Total = pckg.Fare;
        //                }
        //                model.Data = data;
        //            }
        //            return View(model);
        //        }

        public ActionResult CabReport(DateTime? sDate, DateTime? eDate, string term = "", bool export = false, int menuId = 0)
        {
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(term))
                term = term.Trim().ToLower();
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            string startDate = sDate.Value.ToString("yyyy-MM-dd");
            string endDate = eDate.Value.ToString("yyyy-MM-dd");
            var query = @"select c.Id,c.VehicleNumber,vm.ModelName,v.CompanyName,sub.TotalRides,sub.TotalHrs,sub.TotalKms,sub.TotalAmt from cab c join 
(select cab_Id,count(bk.Id) as  TotalRides,sum(cb.TotalKms) as TotalKms,sum(cb.TotalHrs) as TotalHrs,sum(cb.NetAmount-cb.DiscountAmount) as TotalAmt   from Booking bk
join CorporateBill cb on bk.Id=cb.Booking_Id
where Convert(DateTime,cb.JourneyClosingDate) between '" + startDate + "' and '"+ endDate + "' and cab_Id is not null and cb.IsCancelled=0 group by bk.Cab_Id) sub  on c.Id = sub.Cab_Id left join Vendor v on c.Vendor_Id = v.Id left join VehicleModel vm on c.VehicleModel_Id=vm.Id";
            var data = ent.Database.SqlQuery<CabReportData>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.VehicleNumber.ToLower().Contains(term.ToLower())).ToList();
            }
            if (export)
            {
                var grid = new GridView();
                grid.DataSource = data;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =cab monthly duties (" + startDate + " to " + endDate + ").xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new CabReportModel();
            rm.Data = data;
            rm.Total = data.Sum(a => a.TotalRides);
            rm.sDate = sDate.Value;
            rm.eDate = eDate.Value;
            rm.Term = term;
            rm.MenuId = menuId;
            rm.StartDate = startDate;
            rm.EndDate = endDate;
            return View(rm);
        }

        public ActionResult CabReportDetail(int cabId, string sDate, string eDate, int menuId,string term="")
        {
//            var query = @"select  pt.PType as PackageTypeName,rt.RentalTypeName,cb.TotalHrs,cb.TotalKms,c.VehicleNumber,
//b.*,
//cb.StartKms,cb.EndKms,cb.StartHour,cb.EndHour,cb.TollCharge,cb.TotalNight,cb.NightCharge,cb.OutStationCharge,cb.Total,cb.Fare,cb.TotalKms  from booking b
//left join PackageType pt on b.PackageType_Id = pt.Id
//left join RentalType rt on b.RentalType =rt.Id
//join cab c on b.Cab_Id=c.Id
//join CorporateBill cb on b.Id=cb.Booking_Id
//where b.Cab_Id=" + cabId+" and Convert(date,b.PickupDate) between '"+sDate+"' and '"+eDate+"'";

            var query = @"select  
b.BookingId,d.DriverName,c.VehicleNumber,dc.ModelName as DesiredCar,cb.GuestName,cb.JourneyStartDate,cb.JourneyClosingDate,b.CompanyName,cb.VisitedPlace,pt.PType as PackageTypeName,rt.RentalTypeName,
cb.Fare,cb.StartKms,cb.EndKms,cb.TotalKms,cb.StartHour,cb.EndHour,cb.TotalHrs,cb.ExtraHrs,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsCharge,cb.TotalNight,cb.NightCharge,cb.OutStationDays,cb.OutStationCharge,cb.ParkCharge,cb.MCD,
cb.TollCharge,cb.InterStateCharge as StateCharge,cb.MiscCharge,cb.NetAmount as SubTotal,cb.DiscountAmount,(cb.NetAmount-isnull(cb.DiscountAmount,0)) as NetAmount
from booking b
left join PackageType pt on b.PackageType_Id = pt.Id
left join RentalType rt on b.RentalType =rt.Id
left join Driver d on b.DriverId=d.Id
left join VehicleModel dc on b.VehicleModel_Id=dc.Id
join cab c on b.Cab_Id=c.Id
join CorporateBill cb on b.Id=cb.Booking_Id
where IsCancelled=0 and  b.Cab_Id=" + cabId + " and Convert(date,cb.JourneyClosingDate) between '" + sDate + "' and '" + eDate + "'";
           

            var data = ent.Database.SqlQuery<CabReportDetail>(query).ToList();
            var model = new CabReportDetailModel();
            model.menuId = menuId;
            model.Data = data;
            model.SDate = sDate;
            model.eDate = eDate;
            model.term = term;
            if (data.Count > 0)
                model.CabNo = data[0].VehicleNumber;
            return View(model);
        }

        public ActionResult DriverReport(DateTime? sDate, DateTime? eDate, string term = "", bool export = false, int menuId = 0)
        {
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(term))
                term = term.Trim().ToLower();
            var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (sDate == null || eDate == null)
            {
                sDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                eDate = new DateTime(currentDate.Year, currentDate.Month, lastDay);
            }
            string startDate = sDate.Value.ToString("yyyy-MM-dd");
            string endDate = eDate.Value.ToString("yyyy-MM-dd");
            string leaveMonth= sDate.Value.ToString("MM");
            string leaveYear=  sDate.Value.ToString("yyyy");
            //            var query = @"select d.Id,d.DriverName,d.MobileNumber,sub.TotalRides,sub.TotalHrs,sub.TotalKms,sub.TotalNight,sub.TotalTA,26 as TotalDuty from Driver d join 
            //(select DriverId,count(bk.Id) as  TotalRides,sum(cb.TotalKms) as TotalKms,sum(cb.TotalHrs) as TotalHrs,sum(cb.TotalNight) as TotalNight,sum(cb.OutStationDays) as TotalTA from Booking bk
            //join CorporateBill cb on bk.Id=cb.Booking_Id
            //where Convert(DateTime,cb.JourneyClosingDate) between '" + startDate + "' and '"+ endDate + "' and cab_Id is not null and cb.IsCancelled=0 group by bk.DriverId) sub  on d.Id = sub.DriverId";

            var query = @"select d.Id,d.DriverName,d.MobileNumber,sub.TotalRides,sub.TotalHrs,sub.TotalKms,sub.TotalNight,sub.TotalTA,

(select 30-isnull(sum(datediff(day,DateFrom,DateTo)+1),0) as leaveDays from DriverLeave
where  ((datepart(MM,DateTo)="+leaveMonth+" and datepart(YYYY,DateTo)="+leaveYear+") or (datepart(MM,DateFrom)="+leaveMonth+" and datepart(YYYY,DateFrom)="+leaveYear+ "))  and  Driver_Id=d.Id ) as TotalDuty,sub.TotalAmt " +
" from Driver d join (select DriverId,count(bk.Id) as  TotalRides,sum(cb.TotalKms) as TotalKms,sum(cb.TotalHrs) as TotalHrs,sum(cb.TotalNight) as TotalNight,sum(cb.OutStationDays) as TotalTA,sum(cb.NetAmount-cb.DiscountAmount) as TotalAmt from Booking bk " +
" join CorporateBill cb on bk.Id=cb.Booking_Id "+
"where Convert(DateTime,cb.JourneyClosingDate) between '" + startDate + "' and '" + endDate + "' and cab_Id is not null and cb.IsCancelled=0 group by bk.DriverId) sub  on d.Id = sub.DriverId";
            
            var data = ent.Database.SqlQuery<DriverReportData>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.DriverName.ToLower().Contains(term.ToLower())).ToList();
            }
            if (export)
            {
                var grid = new GridView();
                grid.DataSource = data;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =driver monthly duties (" + startDate + " to " + endDate + ").xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            var rm = new DriverReportModel();
            rm.Data = data;
            rm.Total = data.Sum(a => a.TotalRides);
            rm.sDate = sDate.Value;
            rm.eDate = eDate.Value;
            rm.Term = term;
            rm.MenuId = menuId;
            rm.StartDate = startDate;
            rm.EndDate = endDate;
            return View(rm);
        }

        public ActionResult DriverReportDetail(int driverId, string sDate, string eDate, int menuId, string term = "")
        {
//            var query = @"select d.DriverName,d.MobileNumber,cab.VehicleNumber,pt.PType as PackageTypeName,rt.RentalTypeName,
//cb.TotalHrs,cb.TotalKms,cb.StartKms,cb.EndKms,cb.Total,cb.Fare,cb.TollCharge,cb.OutStationCharge,cb.TotalNight,cb.NightCharge,b.*  from booking b
//left join PackageType pt on b.PackageType_Id = pt.Id
//left join RentalType rt on b.RentalType =rt.Id
//join Driver d on b.DriverId=d.Id
//join CorporateBill cb on b.Id=cb.Booking_Id
//left join cab on b.Cab_Id=cab.Id
//where b.DriverId=" + driverId + " and Convert(date,b.PickupDate) between '" + sDate + "' and '" + eDate + "'";



            var query = @"select d.DriverName,b.BookingId,cab.VehicleNumber,cb.JourneyStartDate,cb.JourneyClosingDate,b.CompanyName,cb.VisitedPlace,pt.PType as PackageTypeName,rt.RentalTypeName,
cb.Fare,cb.ExtraHrs,cb.ExtraHrsCharge,cb.ExtraKms,cb.ExtraKmsCharge,cb.TotalNight,cb.NightCharge,cb.OutStationDays,cb.OutStationCharge,cb.ParkCharge,cb.MCD,
cb.TollCharge,cb.InterStateCharge as StateCharge,cb.MiscCharge,cb.NetAmount as SubTotal,cb.DiscountAmount,(cb.NetAmount-isnull(cb.DiscountAmount,0)) as NetAmount
from booking b
left join PackageType pt on b.PackageType_Id = pt.Id
left join RentalType rt on b.RentalType = rt.Id
join Driver d on b.DriverId = d.Id
join CorporateBill cb on b.Id = cb.Booking_Id
left join cab on b.Cab_Id = cab.Id
where b.DriverId=" + driverId + " and Convert(date,cb.JourneyClosingDate) between '" + sDate + "' and '" + eDate + "' and IsCancelled = 0";

            var data = ent.Database.SqlQuery<DriverReportDetail>(query).ToList();
            var model = new DriverReportDetailModel();
            model.menuId = menuId;
            model.Data = data;
            model.SDate = sDate;
            model.eDate = eDate;
            model.term = term;
            if(data.Count>0)
            model.DriverName = data[0].DriverName;
            return View(model);
        }

    }
}