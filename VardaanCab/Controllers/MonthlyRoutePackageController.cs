using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class MonthlyRoutePackageController : Controller
    {
        // GET: MonthlyPackageRoute
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();

        public JsonResult GetVehiclePackages(int vehicleModelId)
        {
            var routePackageMaster = ent.MonthlyPackageRouteMasters.Where(a => a.VehicleModel_Id == vehicleModelId).ToList()
                .Select(a => new
                {
                    Id = a.Id,
                    Location = a.PickupLocation + "-" + a.DropLocation
                });
            var record = new
            {
                data = routePackageMaster,
                status = routePackageMaster.Count() > 0 ? 1 : 0
            };
            return Json(record, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPackage(int packageId)
        {
            var pckg = ent.MonthlyPackageRouteMasters.Find(packageId);
            var record = new
            {
                status = pckg == null ? 0 : 1,
                data = pckg
            };
            return Json(record, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateBooking()
        {
            var model = new MonthlyPackageRouteDTO();
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateBooking(MonthlyPackageRouteDTO model)
        {
            model.BookingEndDate = model.BookingStartDate.AddDays(model.NoOfDays);
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var masterPackage = ent.MonthlyPackageRouteMasters.AsNoTracking().FirstOrDefault(a => a.Id == model.MonthlyRoutePackageMaster_Id);
                    if (masterPackage == null)
                    {
                        ModelState.AddModelError("msg", "Invalid master package");
                        return View(model);
                    }
                    var client = ent.Customers.Find(model.Customer_Id);
                    if (client == null)
                    {
                        ModelState.AddModelError("msg", "Invalid client data");
                        return View(model);
                    }
                    var package = Mapper.Map<MonthlyPackageRoute>(model);
                    int loggeInUserId = cr.GetUserDetailId();
                    package.UpdatedBy_Id = loggeInUserId;
                    package.CreateDate = DateTime.Now;
                    package.CreatedBy_Id = loggeInUserId;
                    package.UpdateDate = DateTime.Now;
                    ent.MonthlyPackageRoutes.Add(package);
                    ent.SaveChanges();
                    var user = ent.UserLogins.Find(cr.GetUserDetailId());
                    string newBookingId= cr.GenerateBookingId("regular");
                    int length = newBookingId.Length;
                    string iPart = newBookingId.Substring(3, length - 3);
                    for (int i = 0; i < model.NoOfDays; i++)
                    {
                        int new_i = int.Parse(iPart) + i;
                        string bookingId = "VRC" + new_i;
                        var booking = new Booking
                        {
                            OtherPackage_Id = package.Id,
                            UpdatedBy = cr.GetUserDetailId(),
                            UpdateDate = DateTime.Now,
                            BookedBy = user.Email,
                            BookingStatus = (int)BookingStatus.Pending,
                            CreateBy = user.Email,
                            CreateDate = DateTime.Now,
                            VehicleModel_Id = model.VehicleModel_Id,
                            BookedByPerson = model.BookedBy,
                            BookingDate = DateTime.Now,
                            PickupDate = model.BookingStartDate.AddDays(i),
                            PickupTime = model.PickupTime,
                            PickupAddress = masterPackage.PickupLocation,
                            DropAddress = masterPackage.DropLocation,
                            BookingId = bookingId,
                            BookingType = "monthly-route",
                            Client_Id = model.Customer_Id,
                            CompanyName = client.CompanyName,
                            CustomerName = client.CompanyName
                        };
                        ent.Bookings.Add(booking);
                }
                   ent.SaveChanges();
                    tran.Commit();
                ModelState.AddModelError("msg", "Booking has created successfully");
            }
                catch (Exception ex)
            {
                //if(packageId>0)
                //{
                //    var pck=ent.MonthlyPackageRoutes.Find(packageId);
                //    ent.MonthlyPackageRoutes.Remove(pck);
                //    var bookings = ent.Bookings.Where(a => a.OtherPackage_Id == packageId).ToList();
                //    ent.Bookings.RemoveRange(bookings);
                //    ent.SaveChanges();
                //}
                tran.Rollback();
                ModelState.AddModelError("msg", "Server error");
            }
        }
            return View(model);
    }

         public ActionResult Edit(int id)
    {
        var mp = ent.MonthlyPackageRoutes.Find(id);
        var model = Mapper.Map<MonthlyPackageRouteDTO>(mp);
        model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
        model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.Customer_Id);
        return View(model);
    }

        [HttpPost]
        public ActionResult Edit(MonthlyPackageRouteDTO model)
    {
        model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
        model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.Customer_Id);
        if (!ModelState.IsValid)
            return View(model);
        try
        {
            var package = Mapper.Map<MonthlyPackageRoute>(model);
            int loggeInUserId = cr.GetUserDetailId();
            package.UpdatedBy_Id = loggeInUserId;
            package.UpdateDate = DateTime.Now;
            ent.Entry(package).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            ModelState.AddModelError("msg", "Package has updated successfully");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("msg", "Server error");
        }
        return View(model);
    }

        public ActionResult All(DateTime? sDate, DateTime? eDate, int page = 1, string term = "")
    {
        string bookingType = "regular";
        var model = new MonthlyPackageRoutVm();

        string query = query = @"select mp.*,customer.CompanyName,customer.CustomerName
,customer.ContactNo,created.Email as CreatedBy,
updated.email as UpdatedBy,
vm.ModelName,IsNull(dbo.getLastBillCancelStatus(mp.Id),0) as IsCancelled
from MonthlyPackageRoute mp
join customer on mp.Customer_Id=customer.Id
join Userlogin created on mp.CreatedBy_Id=created.Id
join Userlogin updated on mp.UpdatedBy_Id=updated.Id
join VehicleModel vm on mp.vehiclemodel_id=vm.Id
order by mp.Id desc";
        var data = ent.Database.SqlQuery<MonthlyPackageRouteDTO>(query).ToList();
        if (!string.IsNullOrEmpty(term))
        {
            term = term.ToLower();
            data = data.Where(a => a.CompanyName.ToLower().Contains(term) || a.ContactNo.Contains(term)).ToList();
            page = 1;
        }
        if (sDate != null && eDate != null)
        {
            data = data.Where(a => a.CreateDate.Date >= sDate.Value.Date && a.CreateDate.Date <= eDate.Value.Date).ToList();
            page = 1;
        }
        // pagination
        int total = data.Count;
        int pageSize = 50;
        double totalPages = Math.Ceiling(total / (double)pageSize);
        int skip = pageSize * (page - 1);
        data = data.Skip(skip).Take(pageSize).ToList();
        model.Packages = data;
        model.sDate = sDate;
        model.eDate = eDate;
        model.Term = term;
        model.Page = page;
        model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
        return View(model);
    }

        public ActionResult Entries(int packageId)
    {
        var data = (from e in ent.MonthlyPackageRouteEntries
                    join cab in ent.Cabs on e.Cab_Id equals cab.Id
                    join driver in ent.Drivers on e.Driver_Id equals driver.Id
                    join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                    join user in ent.UserLogins on e.UpdatedBy_Id equals user.Id
                    where e.MonthlyPackageRoute_Id == packageId && e.JourneyClosingDate != null
                    orderby e.Id descending
                    select new MonthlyPackageRouteEntryDTO
                    {
                        Id = e.Id,
                        DriverName = driver.DriverName,
                        VehicleModel = vm.ModelName,
                        UpdatedBy = user.Email,
                        CabNo = e.CabNo,
                        Cab_Id = e.Cab_Id,
                        Driver_Id = e.Driver_Id,
                        EntryDate = e.EntryDate,
                        MonthlyPackageRoute_Id = e.MonthlyPackageRoute_Id,
                        UpdateDate = e.UpdateDate,
                        UpdatedBy_Id = e.UpdatedBy_Id,
                        PickupPlace = e.PickupPlace,
                        PickupTime = e.PickupTime,
                        JourneyClosingDate = e.JourneyClosingDate,
                        JourneyStartDate = e.JourneyStartDate,
                        DropPlace = e.DropPlace,
                        DropTime = e.DropTime,
                        Toll = e.Toll,
                        MCD = e.MCD
                    }
                    ).ToList();
        return View(data);
    }

        public ActionResult DispatchEntries(int packageId)
    {
        var data = (from e in ent.MonthlyPackageRouteEntries
                    join cab in ent.Cabs on e.Cab_Id equals cab.Id
                    join driver in ent.Drivers on e.Driver_Id equals driver.Id
                    join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                    join user in ent.UserLogins on e.UpdatedBy_Id equals user.Id
                    where e.MonthlyPackageRoute_Id == packageId && e.JourneyClosingDate == null
                    orderby e.Id descending
                    select new MonthlyPackageRouteEntryDTO
                    {
                        DriverName = driver.DriverName,
                        VehicleModel = vm.ModelName,
                        UpdatedBy = user.Email,
                        CabNo = e.CabNo,
                        Cab_Id = e.Cab_Id,
                        Driver_Id = e.Driver_Id,
                        EntryDate = e.EntryDate,
                        Id = e.Id,
                        JourneyClosingDate = e.JourneyClosingDate,
                        JourneyStartDate = e.JourneyStartDate,
                        MonthlyPackageRoute_Id = e.MonthlyPackageRoute_Id,
                        UpdateDate = e.UpdateDate,
                        UpdatedBy_Id = e.UpdatedBy_Id,
                        PickupPlace = e.PickupPlace,
                        PickupTime = e.PickupTime,
                        DropPlace = e.DropPlace,
                        DropTime = e.DropTime,
                        Toll = e.Toll,
                        MCD = e.MCD
                    }
                    ).ToList();
        return View(data);
    }

        public ActionResult DayEntry(int packageId)
    {
        var package = ent.MonthlyPackageRoutes.Find(packageId);
        var model = new MonthlyPackageRouteEntryDTO();
        model.MonthlyPackageRoute_Id = packageId;
        var drivers = cr.GetDriver();
        model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
        return View(model);
    }

        [HttpPost]
        public ActionResult DayEntry(MonthlyPackageRouteEntryDTO model)
    {
        var drivers = cr.GetDriver();
        model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
        if (!ModelState.IsValid)
            return View(model);
        using (var tran = ent.Database.BeginTransaction())
        {
            try
            {
                var package = ent.MonthlyPackageRoutes.Find(model.MonthlyPackageRoute_Id);
                var totalEntries = ent.MonthlyPackageRouteEntries.AsNoTracking().Where(a => a.MonthlyPackageRoute_Id == package.Id).ToList().Count;
                if (totalEntries == package.NoOfDays)
                {
                    ModelState.AddModelError("msg", "you can not add more than " + package.NoOfDays + " days.");
                    return View(model);
                }
                var cab = ent.Cabs.Find(model.Cab_Id);
                var entry = Mapper.Map<MonthlyPackageRouteEntry>(model);
                entry.UpdateDate = DateTime.Now;
                entry.CabNo = cab.VehicleNumber;
                entry.UpdatedBy_Id = cr.GetUserDetailId();
                entry.EntryDate = DateTime.Now;
                ent.MonthlyPackageRouteEntries.Add(entry);
                cab.IsAvailable = false;
                var driver = ent.Drivers.Find(model.Driver_Id);
                driver.IsAvailable = false;
                ent.SaveChanges();
                tran.Commit();
                return RedirectToAction("DispatchEntries", new { packageId = model.MonthlyPackageRoute_Id });
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ModelState.AddModelError("msg", "Server Error");
                return View(model);
            }
        }
    }

        public ActionResult EditEntry(int id)
    {
        var entry = ent.MonthlyPackageRouteEntries.Find(id);
        var model = Mapper.Map<MonthlyPackageRouteEntryDTO>(entry);
        var drivers = cr.GetDriver().ToList();
        var driver = ent.Drivers.Find(model.Driver_Id);
        drivers.Add(driver);
        model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
        return View(model);
    }

        [HttpPost]
        public ActionResult EditEntry(MonthlyPackageRouteEntryDTO model)
    {
        using (var tran = ent.Database.BeginTransaction())
        {
            var drivers = cr.GetDriver();
            var driver = ent.Drivers.Find(model.Driver_Id);
            drivers.Add(driver);
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            ModelState.Remove("Driver_Id");
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var entry = ent.MonthlyPackageRouteEntries.Find(model.Id);
                if (entry.Cab_Id != model.Cab_Id)
                {
                    string q1 = "update Cab set IsAvailable=1 where id=" + entry.Cab_Id;
                    ent.Database.ExecuteSqlCommand(q1);
                    var cab = ent.Cabs.Find(model.Cab_Id);
                    entry.Cab_Id = model.Cab_Id;
                    entry.CabNo = cab.VehicleNumber;
                    cab.IsAvailable = false;
                    ent.SaveChanges();
                }
                if (entry.Driver_Id != model.Driver_Id)
                {
                    string q1 = "update Driver set IsAvailable=1 where id=" + entry.Driver_Id;
                    ent.Database.ExecuteSqlCommand(q1);
                    entry.Driver_Id = model.Driver_Id;
                    string q2 = "update Driver set IsAvailable=0 where id=" + model.Driver_Id;
                    ent.Database.ExecuteSqlCommand(q2);
                    ent.SaveChanges();

                }
                entry.UpdatedBy_Id = cr.GetUserDetailId();
                entry.UpdateDate = DateTime.Now;
                ent.SaveChanges();
                tran.Commit();
                return RedirectToAction("DispatchEntries", new { packageId = model.MonthlyPackageRoute_Id });
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ModelState.AddModelError("msg", "Server error.");
                return View(model);
            }
        }
    }

        public ActionResult CloseDailyEntry(int entryId)
    {
        var entry = ent.MonthlyPackageRouteEntries.Find(entryId);
        var model = Mapper.Map<MonthlyPackageRouteEntryDTO>(entry);
        var package = ent.MonthlyPackageRoutes.Find(model.MonthlyPackageRoute_Id);
        model.MonthlyPackageRoute_Id = package.Id;
        return View(model);
    }

        [HttpPost]
        public ActionResult CloseDailyEntry(MonthlyPackageRouteEntryDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);
        using (var tran = ent.Database.BeginTransaction())
        {
            try
            {
                var entry = Mapper.Map<MonthlyPackageRouteEntry>(model);
                entry.UpdateDate = DateTime.Now;
                entry.UpdatedBy_Id = cr.GetUserDetailId();
                ent.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                //make cab available
                var cab = ent.Cabs.Find(entry.Cab_Id);
                cab.IsAvailable = true;
                //  make driver available
                var driver = ent.Drivers.Find(entry.Driver_Id);
                driver.IsAvailable = true;
                ent.SaveChanges();
                tran.Commit();
                ModelState.AddModelError("msg", "Record has saved");
                return RedirectToAction("Entries", new { packageId = model.MonthlyPackageRoute_Id });
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ModelState.AddModelError("msg", "Server Error");
                return View(model);
            }
        }
    }

        public ActionResult CreateMonthlyBill(int packageId, DateTime? sDate, DateTime? eDate, string term = "", int page = 1)
    {
        var model = new MonthlyPackageRouteBillDTO();
        model.MonthlyPackageRoute_Id = packageId;
        var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
        model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
        model.sDate = sDate;
        model.eDate = eDate;
        model.term = term;
        model.page = page;
        return View(model);
    }

        [HttpPost]
        public ActionResult CreateMonthlyBill(MonthlyPackageRouteBillDTO model)
    {
        var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
        model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
        using (var tran = ent.Database.BeginTransaction())
        {
            try
            {
                var package = ent.MonthlyPackageRoutes.Find(model.MonthlyPackageRoute_Id);
                if (package == null)
                    throw new Exception("Package does not found");
                var client = ent.Customers.Find(package.Customer_Id);
                if (client == null)
                    throw new Exception("Client does not found");

                package.IsClosed = true;
                package.UpdateDate = DateTime.Now;
                package.UpdatedBy_Id = cr.GetUserDetailId();
                // create bill
                string billNo = "VRC/1";
                var lastBill = ent.MonthlyPackageRouteBills.OrderByDescending(a => a.Id).FirstOrDefault();
                if (lastBill != null)
                {
                    string lastBillNo = lastBill.BillNumber;
                    string sPart = lastBillNo.Substring(0, 4);
                    string iPart = lastBillNo.Substring(sPart.Length, lastBillNo.Length - sPart.Length);
                    billNo = sPart + (int.Parse(iPart) + 1);
                }
                var entries = ent.MonthlyPackageRouteEntries.Where(a => a.MonthlyPackageRoute_Id == package.Id).ToList();
                var bill = new MonthlyPackageRouteBill();
                var taxInPercent = ent.Taxes.FirstOrDefault().Amount;
                bill.GstInPercent = taxInPercent;
                bill.Fare = package.Amount; //taxable
                bill.BillNumber = billNo;
                bill.NoOfDays = package.NoOfDays ?? 0;
                bill.Toll = entries.Sum(a => a.Toll);
                bill.MCD = entries.Sum(a => a.MCD);
                bill.MonthlyPackageRoute_Id = package.Id;
                bill.BillingState_Id = model.BillingState_Id;
                double taxableAmount = bill.Fare;
                double gst = Math.Round((taxableAmount * taxInPercent) / 100, 2);

                if (bill.BillingState_Id == client.State_Id)
                {
                    bill.SGST = Math.Round(gst / 2, 2);
                    bill.CGST = Math.Round(gst / 2, 2);
                }
                else
                {
                    bill.IGST = Math.Round(gst, 2);
                }
                double total = Math.Round(taxableAmount + bill.Toll + bill.MCD + gst, 2);
                bill.TotalAmount = total;
                bill.UpdateDate = DateTime.Now;
                bill.UpdatedBy_Id = cr.GetUserDetailId();
                bill.BillDate = DateTime.Now;
                ent.MonthlyPackageRouteBills.Add(bill);
                ent.SaveChanges();
                tran.Commit();
                return RedirectToAction("All", new { packageId = model.MonthlyPackageRoute_Id, sDate = model.sDate, eDate = model.eDate, term = model.term, page = model.page });
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ModelState.AddModelError("msg", "Server error");
                return View(model);
            }
        }
    }

        public ActionResult ViewMonthlyBill(int packageId, bool IsTaxInvoice = false, bool isPdf = false)
    {
        var data = (from package in ent.MonthlyPackageRoutes
                    join cbill in ent.MonthlyPackageRouteBills
                    on package.Id equals cbill.MonthlyPackageRoute_Id
                    join swg in ent.StateWiseGSTINs on cbill.BillingState_Id equals swg.State_Id
                    join company in ent.Customers on package.Customer_Id equals company.Id
                    join serviceState in ent.StateMasters on company.State_Id equals serviceState.Id
                    join vm in ent.VehicleModels on package.VehicleModel_Id equals vm.Id
                    join fromState in ent.StateMasters on swg.State_Id equals fromState.Id
                    where package.Id == packageId
                    select new TaxInvoiceViewModel
                    {
                        isPdf = isPdf,
                        Id = cbill.Id,
                        NoOfDays = package.NoOfDays ?? 0,
                        GuestName = company.CompanyName,
                        DutyAddress = company.FullAddress,
                        DsDate = cbill.UpdateDate,
                        DsNo = cbill.BillNumber,
                        Fare = cbill.Fare,
                        Total = cbill.TotalAmount,
                        TollCharge = cbill.Toll,
                        VehicleName = vm.ModelName,
                        FromAddress = swg.Address,
                        FromGSTIN = swg.Gstin,
                        FromStateName = fromState.StateName,
                        BilledAddress = company.FullAddress,
                        CompanyName = company.CompanyName,
                        CompanyGSTIN = company.GSTIN,
                        PlaceOfService = serviceState.StateName,
                        CGST = cbill.CGST,
                        SGST = cbill.SGST,
                        IGST = cbill.IGST,
                        ParkTollStateCharge = cbill.Toll,
                        IgstPercent = cbill.GstInPercent,
                        CgstPercent = cbill.GstInPercent / 2,
                        SgstPercent = cbill.GstInPercent / 2,
                        IsTaxInvoice = IsTaxInvoice,
                        BillFile = cbill.BillFile,
                        MCD = cbill.MCD,
                        NetAmount = cbill.NetAmount,
                        IsCancelled = cbill.IsCancelled
                    }).ToList().OrderByDescending(a => a.Id).FirstOrDefault(a => a.IsCancelled == false);
        return View(data);
    }

        public ActionResult GeneratePdfMonthlyBill(int billId, bool IsTaxInvoice)
    {
        var cbill = ent.MonthlyPackageRouteBills.Find(billId);
        if (string.IsNullOrEmpty(cbill.BillFile))
        {
            var dt = DateTime.Now;
            string filename = "" + dt.Month + dt.Day + dt.Hour + dt.Year + dt.Minute + dt.Second + dt.Millisecond + Guid.NewGuid().ToString() + ".pdf";
            string path = Server.MapPath("/Files/") + filename;
            cbill.BillFile = filename;
            ent.SaveChanges();
            return new ActionAsPdf("ViewMonthlyBill", new { packageId = cbill.MonthlyPackageRoute_Id, IsTaxInvoice = IsTaxInvoice, isPdf = true }) { FileName = filename, SaveOnServerPath = path };
        }
        return Content("already added");
    }

        public ActionResult SendMonthlyPakcageBill(int billId)
    {
        try
        {
            var data = ent.MonthlyPackageRouteBills.Find(billId);
            var package = ent.MonthlyPackageRoutes.Find(data.MonthlyPackageRoute_Id);
            var client = ent.Customers.Find(package.Customer_Id);
            var path = Server.MapPath("~/Files/" + data.BillFile);
            EmailOperation.SendEmail(client.Email, "VardaanRentACar Monthly Route  Invoice", "Please find the Attachements", true, path);
            return Content("Invoice has sent.");
        }
        catch (Exception ex)
        {
            return Content("Server error.");
        }
    }

        public ContentResult CancelInvoice(int packageId)
    {
        using (var tran = ent.Database.BeginTransaction())
        {
            try
            {
                var package = ent.MonthlyPackageRoutes.Find(packageId);
                package.IsClosed = false;
                package.UpdateDate = DateTime.Now;
                package.UpdatedBy_Id = cr.GetUserDetailId();
                /////
                var bill = ent.MonthlyPackageRouteBills.OrderByDescending(a => a.Id).FirstOrDefault(a => a.MonthlyPackageRoute_Id == packageId && a.IsCancelled == false);
                bill.IsCancelled = true;
                bill.UpdateDate = DateTime.Now;
                bill.BillFile = null;
                package.UpdatedBy_Id = cr.GetUserDetailId();
                ent.SaveChanges();
                tran.Commit();
                return Content("ok");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return Content("error");
            }
        }
    }

        //public ActionResult GetCabList(string term)
    //{
    //    string query = @"select c.Id,vm.ModelName,c.VehicleNumber from cab c join VehicleModel vm on c.VehicleModel_Id=vm.Id
    //where c.VehicleModel_Id in (select distinct vehiclemodel_Id from vendorpackage)
    //and c.IsAvailable=1 and c.IsActive=1 and c.VehicleNumber like '" + term.ToLower() + "%'";
    //    var data = ent.Database.SqlQuery<CabDTO>(query).ToList();
    //    return Json(data, JsonRequestBehavior.AllowGet);
    //}
}
}