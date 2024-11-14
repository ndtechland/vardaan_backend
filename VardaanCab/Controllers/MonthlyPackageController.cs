using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class MonthlyPackageController : Controller
    {
        // GET: MonthlyPackage
        DbEntities ent = new DbEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();

        public ActionResult GetMasterMnthlyPckg(int modelId)
        {
            var package = ent.MonthlyPackageMasters.Find(modelId);
            var data = new {
                package = package,
                status = package == null ? "khali" : "ok"
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreatePackage(int menuId=0)
        {
            var model = new MonthlyPackageDTO();
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePackage(MonthlyPackageDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var package = Mapper.Map<MonthlyPackage>(model);
                    int loggeInUserId = cr.GetUserDetailId();
                    package.UpdatedBy_Id = loggeInUserId;
                    package.CreateDate = DateTime.Now;
                    package.CreatedBy_Id = loggeInUserId;
                    package.UpdateDate = DateTime.Now;
                    ent.MonthlyPackages.Add(package);
                    ent.SaveChanges();
                    ModelState.AddModelError("msg", "Booking has created successfully");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("msg", "Server error");
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var mp = ent.MonthlyPackages.Find(id);
            var model = Mapper.Map<MonthlyPackageDTO>(mp);
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.Customer_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MonthlyPackageDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            model.ClientList = new SelectList(ent.Customers.Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.Customer_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var package = Mapper.Map<MonthlyPackage>(model);
                int loggeInUserId = cr.GetUserDetailId();
                package.UpdatedBy_Id = loggeInUserId;
                package.UpdateDate = DateTime.Now;
                ent.Entry(package).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                ModelState.AddModelError("msg", "Booking has updated successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("msg", "Server error");
            }
            return View(model);
        }

        public ActionResult All(DateTime? sDate, DateTime? eDate, int page = 1, string term = "",int menuId=0)
        {
            string bookingType = "regular";
            var model = new MonthlyPackageVm();
            model.MenuId = menuId;
            string query = query = @"select mp.*,customer.CompanyName,customer.CustomerName
,customer.ContactNo,created.Email as CreatedBy,
updated.email as UpdatedBy,
vm.ModelName,IsNull(dbo.getLastBillCancelStatus(mp.Id),0) as IsCancelled
from MonthlyPackage mp
join customer on mp.Customer_Id=customer.Id
join Userlogin created on mp.CreatedBy_Id=created.Id
join Userlogin updated on mp.UpdatedBy_Id=updated.Id
join VehicleModel vm on mp.vehiclemodel_id=vm.Id
order by mp.Id desc";
            var data = ent.Database.SqlQuery<MonthlyPackageDTO>(query).ToList();
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
            int pageSize = 500;
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

        // closed monthly entries
        public ActionResult Entries(int packageId,int menuId=0)
        {
            var data = (from e in ent.MonthlyPackageEntries
                        join cab in ent.Cabs on e.Cab_Id equals cab.Id
                        join driver in ent.Drivers on e.Driver_Id equals driver.Id
                        join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                        join user in ent.UserLogins on e.UpdatedBy_Id equals user.Id
                        where e.MonthlyPackage_Id == packageId && e.JourneyClosingDate!=null
                        orderby e.Id descending
                        select new MonthlyPackageEntryDTO
                        {
                            DriverName = driver.DriverName,
                            VehicleModel = vm.ModelName,
                            UpdatedBy = user.Email,
                            CabNo = e.CabNo,
                            Cab_Id = e.Cab_Id,
                            NetAmount = e.NetAmount,
                            Driver_Id = e.Driver_Id,
                            EndHr = e.EndHr,
                            EndKm = e.EndKm,
                            EntryDate = e.EntryDate,
                            ExtraHr = e.ExtraHr,
                            ExtraHrsCharge = e.ExtraHrsCharge,
                            ExtraHrsRate = e.ExtraHrsRate,
                            ExtraKm = e.ExtraKm,
                            InterStateTax = e.InterStateTax,
                            Id = e.Id,
                            JourneyClosingDate = e.JourneyClosingDate,
                            JourneyStartDate = e.JourneyStartDate,
                            MCD = e.MCD,
                            MinHrs = e.MinHrs,
                            MonthlyPackage_Id = e.MonthlyPackage_Id,
                            NightCharge = e.NightCharge,
                            NightRate = e.NightRate,
                            ParkingCharge = e.ParkingCharge,
                            StartHr = e.StartHr,
                            StKm = e.StKm,
                            TollCharge = e.TollCharge,
                            Total = e.Total,
                            TotalHrs = e.TotalHrs,
                            TotalKm = e.TotalKm,
                            TotalNight = e.TotalNight,
                            UpdateDate = e.UpdateDate,
                            UpdatedBy_Id = e.UpdatedBy_Id
                        }
                        ).ToList();
            ViewBag.menuId = menuId;
            return View(data);
        }

        //public ActionResult DispatchEntries(int packageId, int menuId = 0)
        //{
        //    var data = (from e in ent.MonthlyPackageEntries
        //                join cab in ent.Cabs on e.Cab_Id equals cab.Id
        //                join driver in ent.Drivers on e.Driver_Id equals driver.Id
        //                join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
        //                join user in ent.UserLogins on e.UpdatedBy_Id equals user.Id
        //                where e.MonthlyPackage_Id == packageId && e.JourneyClosingDate == null
        //                orderby e.Id descending
        //                select new MonthlyPackageEntryDTO
        //                {
        //                    DriverName = driver.DriverName,
        //                    VehicleModel = vm.ModelName,
        //                    UpdatedBy = user.Email,
        //                    CabNo = e.CabNo,
        //                    Cab_Id = e.Cab_Id,
        //                    NetAmount = e.NetAmount,
        //                    Driver_Id = e.Driver_Id,
        //                    EndHr = e.EndHr,
        //                    EndKm = e.EndKm,
        //                    EntryDate = e.EntryDate,
        //                    ExtraHr = e.ExtraHr,
        //                    ExtraHrsCharge = e.ExtraHrsCharge,
        //                    ExtraHrsRate = e.ExtraHrsRate,
        //                    ExtraKm = e.ExtraKm,
        //                    InterStateTax = e.InterStateTax,
        //                    Id = e.Id,
        //                    JourneyClosingDate = e.JourneyClosingDate,
        //                    JourneyStartDate = e.JourneyStartDate,
        //                    MCD = e.MCD,
        //                    MinHrs = e.MinHrs,
        //                    MonthlyPackage_Id = e.MonthlyPackage_Id,
        //                    NightCharge = e.NightCharge,
        //                    NightRate = e.NightRate,
        //                    ParkingCharge = e.ParkingCharge,
        //                    StartHr = e.StartHr,
        //                    StKm = e.StKm,
        //                    TollCharge = e.TollCharge,
        //                    Total = e.Total,
        //                    TotalHrs = e.TotalHrs,
        //                    TotalKm = e.TotalKm,
        //                    TotalNight = e.TotalNight,
        //                    UpdateDate = e.UpdateDate,
        //                    UpdatedBy_Id = e.UpdatedBy_Id
        //                }
        //                ).ToList();
        //    ViewBag.menuId = menuId;  
        //    return View(data);
        //}


        // dispatch new vehicle
        public ActionResult DayEntry(int packageId, int menuId = 0)
        {
            var package = ent.MonthlyPackages.Find(packageId);
            var model = new MonthlyPackageEntryDTO();
            model.MonthlyPackage_Id = packageId;
            var drivers = cr.GetDriver();
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult DayEntry(MonthlyPackageEntryDTO model)
        {
            var drivers = cr.GetDriver();
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var package = ent.MonthlyPackages.Find(model.MonthlyPackage_Id);
                    var totalEntries = ent.MonthlyPackageEntries.AsNoTracking().Where(a => a.MonthlyPackage_Id == package.Id).ToList().Count;
                    if(totalEntries==package.NoOfDays)
                    {
                        ModelState.AddModelError("msg", "you can not add more than " + package.NoOfDays + " days.");
                        return View(model);
                    }
                    var cab = ent.Cabs.Find(model.Cab_Id);
                    var entry = Mapper.Map<MonthlyPackageEntry>(model);
                    entry.UpdateDate = DateTime.Now;
                    entry.CabNo = cab.VehicleNumber;
                    entry.UpdatedBy_Id = cr.GetUserDetailId();
                    entry.EntryDate = DateTime.Now;
                    ent.MonthlyPackageEntries.Add(entry);
                    cab.IsAvailable = false;
                    var driver = ent.Drivers.Find(model.Driver_Id);
                    driver.IsAvailable = false;

                    ent.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("DispatchEntries", new { packageId =model.MonthlyPackage_Id,menuId=model.MenuId});
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ModelState.AddModelError("msg", "Server Error");
                    return View(model);
                }
            }
        }

        public ActionResult EditEntry(int id, int menuId = 0)
        {
            var entry = ent.MonthlyPackageEntries.Find(id);
            var model = Mapper.Map<MonthlyPackageEntryDTO>(entry);
            var drivers = cr.GetDriver().ToList();
            var driver = ent.Drivers.Find(model.Driver_Id);
            drivers.Add(driver);
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEntry(MonthlyPackageEntryDTO model)
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
                    var entry = ent.MonthlyPackageEntries.Find(model.Id);
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
                    entry.JourneyStartDate = model.JourneyStartDate;
                    entry.StartHr = model.StartHr;
                    entry.StKm = model.StKm;
                    entry.UpdatedBy_Id = cr.GetUserDetailId();
                    entry.UpdateDate = DateTime.Now;
                    ent.SaveChanges();
                    tran.Commit();
                    //return RedirectToAction("DispatchEntries", new { packageId = model.MonthlyPackage_Id,menuId=model.MenuId });
                    return RedirectToAction("AllMonthlyDispatchEntries", new { menuId=model.MenuId });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ModelState.AddModelError("msg", "Server error.");
                    return View(model);
                }
            }
        }

        public ActionResult CloseDailyEntry(int entryId,string prevUrl="")
        {
            var entry = ent.MonthlyPackageEntries.Find(entryId);
            var model = Mapper.Map<MonthlyPackageEntryDTO>(entry);
            var package = ent.MonthlyPackages.Find(model.MonthlyPackage_Id);
            model.MonthlyPackage_Id = package.Id;
            model.MinHrs = package.FixedHourPerDay;
            model.NightRate = package.NightCharge;
            model.ExtraHrsRate = package.ExtraHourRate;
            model.PrevUrl = prevUrl;
            //model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CloseDailyEntry(MonthlyPackageEntryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var entry = Mapper.Map<MonthlyPackageEntry>(model);
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
                    //return RedirectToAction("Entries", new { packageId=model.MonthlyPackage_Id,menuId=model.MenuId});
                    //return RedirectToAction("AllMonthlyDispatchEntries", new { menuId=model.MenuId });
                    return Redirect(model.PrevUrl);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ModelState.AddModelError("msg", "Server Error");
                    return View(model);
                }
            }
        }

        public ActionResult CreateMonthlyBill(int packageId, DateTime? sDate, DateTime? eDate, string term = "", int page = 1, int menuId = 0)
        {
            var model = new MonthlyPackageBillDTO();
            model.MonthlyPackage_Id = packageId;
            var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
            model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
            model.sDate = sDate;
            model.eDate = eDate;
            model.term = term;
            model.page = page;
            model.MenuId = model.MenuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateMonthlyBill(MonthlyPackageBillDTO model)
        {
            var stateWiseGstinList = stateWiseGstinRepo.GetStateMasterGstinList();
            model.StateWiseGstinList = new SelectList(stateWiseGstinList, "State_Id", "StateName");
            using (var tran = ent.Database.BeginTransaction())
            {
              try
                {
                    var package = ent.MonthlyPackages.Find(model.MonthlyPackage_Id);
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
                    var lastBill = ent.MonthlyPackageBills.OrderByDescending(a => a.Id).FirstOrDefault();
                    if(lastBill!=null)
                    {
                        string lastBillNo = lastBill.BillNumber;
                        string sPart = lastBillNo.Substring(0, 4);
                        string iPart = lastBillNo.Substring(sPart.Length, lastBillNo.Length - sPart.Length);
                        billNo = sPart+ (int.Parse(iPart)+1);
                    }
                    var entries = ent.MonthlyPackageEntries.Where(a => a.MonthlyPackage_Id == package.Id).ToList();
                    var bill = new MonthlyPackageBill();
                    var taxInPercent = ent.Taxes.FirstOrDefault().Amount;
                    bill.GstInPercent = taxInPercent;
                    bill.Fare = package.Fare; //taxable
                    bill.BillNumber = billNo;
                    bill.FixedKm = package.FixedKms;
                    bill.TotalKm = entries.Sum(a => a.TotalKm);
                    double extraKm = 0;
                    if(bill.TotalKm>bill.FixedKm)
                        extraKm = bill.TotalKm - bill.FixedKm;
                    bill.ExtraKm = (int)extraKm;
                    bill.ExtraKmRate = package.ExtraKmRate;
                    bill.ExtraKmCharge = bill.ExtraKm * bill.ExtraKmRate; //taxable
                    bill.NoOfDays = package.NoOfDays;
                    bill.TotalHr = entries.Sum(a => a.TotalHrs);
                    bill.ExtraHr = entries.Sum(a => a.ExtraHr);
                    bill.ExtraHrRate = package.ExtraHourRate;
                    bill.ExtraHrCharge = bill.ExtraHr*bill.ExtraHrRate; //taxable
                    bill.TollCharge= entries.Sum(a => a.TollCharge);
                    bill.InterStateTax= entries.Sum(a => a.InterStateTax);
                    bill.ParkingCharge= entries.Sum(a => a.ParkingCharge); //taxable
                    bill.MCD= entries.Sum(a => a.MCD); //taxable
                    bill.TotalNights = entries.Sum(a => a.TotalNight);
                    bill.NightRate = package.NightCharge;
                    bill.NightCharge = bill.TotalNights*bill.NightRate; //taxable
                    bill.MonthlyPackage_Id = package.Id;
                    bill.BillingState_Id = model.BillingState_Id;
                    double taxableAmount = bill.Fare + bill.ExtraKmCharge + bill.ExtraHrCharge +
                                          bill.NightCharge+bill.MCD+bill.ParkingCharge;
                    double gst = Math.Round((taxableAmount * taxInPercent) / 100,2);
                   
                    if(bill.BillingState_Id==client.State_Id)
                    {
                        bill.SGST = Math.Round(gst / 2, 2);
                        bill.CGST = Math.Round(gst / 2, 2);
                    }
                    else
                    {
                        bill.IGST = Math.Round(gst, 2);
                    }
                    double total = Math.Round(taxableAmount + bill.TollCharge + bill.InterStateTax + gst,2);
                    bill.TotalAmount = total;
                    bill.UpdateDate = DateTime.Now;
                    bill.UpdatedBy_Id = cr.GetUserDetailId();
                    ent.MonthlyPackageBills.Add(bill);
                    ent.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("All", new { packageId=model.MonthlyPackage_Id,sDate= model.sDate,eDate= model.eDate,term = model.term,page = model.page,menuId=model.MenuId});
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    ModelState.AddModelError("msg", "Server error");
                    return View(model);
                }
            }
        }


        public ActionResult ViewMonthlyBill(int packageId, bool IsTaxInvoice = false,bool isPdf=false, int menuId = 0)
        {
            var data = (from package in ent.MonthlyPackages
                        join cbill in ent.MonthlyPackageBills
                        on package.Id equals cbill.MonthlyPackage_Id  
                        join swg in ent.StateWiseGSTINs on cbill.BillingState_Id equals swg.State_Id
                        join company in ent.Customers on package.Customer_Id equals company.Id
                        join serviceState in ent.StateMasters on company.State_Id equals serviceState.Id
                        join vm in ent.VehicleModels on package.VehicleModel_Id equals vm.Id
                        join fromState in ent.StateMasters on swg.State_Id equals fromState.Id
                        where package.Id == packageId
                        select new TaxInvoiceViewModel
                        {
                            isPdf=isPdf,
                            Id = cbill.Id,
                            NoOfDays=package.NoOfDays,
                            GuestName = company.CompanyName,
                            DutyAddress = company.FullAddress,
                            DsDate = cbill.UpdateDate,
                            DsNo = cbill.BillNumber,
                            Fare = cbill.Fare,
                            ExtraHrsRate = (int)cbill.ExtraHrRate,
                            ExtraKmsRate = (int)cbill.ExtraKmRate,
                            ExtraKms = cbill.ExtraKm,
                            ExtraHrs = cbill.ExtraHr,
                            ExtraHrsCharge = cbill.ExtraHrCharge,
                            ExtraKmsCharge = (int)cbill.ExtraKmCharge,
                            NightCharge = cbill.NightCharge,
                            NightRate = cbill.NightRate,
                            TotalNight = (int)cbill.TotalNights,
                            TotalHrs = cbill.TotalHr,
                            TotalKms = cbill.TotalKm,
                            Total = cbill.TotalAmount,
                            ParkCharge = cbill.ParkingCharge,
                            TollCharge = cbill.TollCharge,
                            InterStateCharge = cbill.InterStateTax,
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
                            ParkTollStateCharge = cbill.ParkingCharge + cbill.TollCharge + cbill.InterStateTax,
                            IgstPercent = cbill.GstInPercent,
                            CgstPercent = cbill.GstInPercent/2,
                            SgstPercent = cbill.GstInPercent/2,
                            IsTaxInvoice = IsTaxInvoice,
                            BillFile = cbill.BillFile,
                            MCD = cbill.MCD,
                            NetAmount = cbill.NetAmount,
                            IsCancelled=cbill.IsCancelled
                        }).ToList().OrderByDescending(a=>a.Id).FirstOrDefault(a=>a.IsCancelled==false);
            return View(data);
        }

        public ActionResult GeneratePdfMonthlyBill(int billId, bool IsTaxInvoice)
        {
            var cbill = ent.MonthlyPackageBills.Find(billId);
            if (string.IsNullOrEmpty(cbill.BillFile))
            {
                var dt = DateTime.Now;
                string filename = "" + dt.Month + dt.Day + dt.Hour + dt.Year + dt.Minute + dt.Second + dt.Millisecond + Guid.NewGuid().ToString() + ".pdf";
                string path = Server.MapPath("/Files/") + filename;
                cbill.BillFile = filename;
                ent.SaveChanges();
                return new ActionAsPdf("ViewMonthlyBill", new { packageId = cbill.MonthlyPackage_Id, IsTaxInvoice=IsTaxInvoice, isPdf=true }) { FileName = filename, SaveOnServerPath = path };
            }
            return Content("already added");
        }

        public ActionResult SendMonthlyPakcageBill(int billId)
        {
            try
            {
                var data = ent.MonthlyPackageBills.Find(billId);
                var package = ent.MonthlyPackages.Find(data.MonthlyPackage_Id);
                var client = ent.Customers.Find(package.Customer_Id);
                var path = Server.MapPath("~/Files/" + data.BillFile);
                EmailOperation.SendEmail(client.Email, "VardaanRentACar Monthly  Invoice", "Please find the Attachements", true, path);
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
                    var package = ent.MonthlyPackages.Find(packageId);
                    package.IsClosed = false;
                    package.UpdateDate = DateTime.Now;
                    package.UpdatedBy_Id = cr.GetUserDetailId();
                    /////
                    var bill = ent.MonthlyPackageBills.OrderByDescending(a => a.Id).FirstOrDefault(a => a.MonthlyPackage_Id == packageId && a.IsCancelled == false);
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

        public ActionResult GetCabList(string term)
        {
            string query = @"select c.Id,vm.ModelName,c.VehicleNumber from cab c join VehicleModel vm on c.VehicleModel_Id=vm.Id
where c.VehicleModel_Id in (select distinct vehiclemodel_Id from vendorpackage)
and c.IsAvailable=1 and c.IsActive=1 and c.VehicleNumber like '" + term.ToLower() + "%'";
            var data = ent.Database.SqlQuery<CabDTO>(query).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllMonthlyDispatchEntries( int menuId = 0)
        {
            var data = (from e in ent.MonthlyPackageEntries
                        join cab in ent.Cabs on e.Cab_Id equals cab.Id
                        join driver in ent.Drivers on e.Driver_Id equals driver.Id
                        join vm in ent.VehicleModels on cab.VehicleModel_Id equals vm.Id
                        join user in ent.UserLogins on e.UpdatedBy_Id equals user.Id
                        join package in ent.MonthlyPackages on e.MonthlyPackage_Id equals package.Id
                        join customer in ent.Customers on package.Customer_Id equals customer.Id
                        where e.JourneyClosingDate == null
                        orderby e.Id descending
                        select new MonthlyPackageEntryDTO
                        {
                            DriverName = driver.DriverName,
                            VehicleModel = vm.ModelName,
                            UpdatedBy = user.Email,
                            CabNo = e.CabNo,
                            Cab_Id = e.Cab_Id,
                            NetAmount = e.NetAmount,
                            Driver_Id = e.Driver_Id,
                            EndHr = e.EndHr,
                            EndKm = e.EndKm,
                            EntryDate = e.EntryDate,
                            ExtraHr = e.ExtraHr,
                            ExtraHrsCharge = e.ExtraHrsCharge,
                            ExtraHrsRate = e.ExtraHrsRate,
                            ExtraKm = e.ExtraKm,
                            InterStateTax = e.InterStateTax,
                            Id = e.Id,
                            JourneyClosingDate = e.JourneyClosingDate,
                            JourneyStartDate = e.JourneyStartDate,
                            MCD = e.MCD,
                            MinHrs = e.MinHrs,
                            MonthlyPackage_Id = e.MonthlyPackage_Id,
                            NightCharge = e.NightCharge,
                            NightRate = e.NightRate,
                            ParkingCharge = e.ParkingCharge,
                            StartHr = e.StartHr,
                            StKm = e.StKm,
                            TollCharge = e.TollCharge,
                            Total = e.Total,
                            TotalHrs = e.TotalHrs,
                            TotalKm = e.TotalKm,
                            TotalNight = e.TotalNight,
                            UpdateDate = e.UpdateDate,
                            UpdatedBy_Id = e.UpdatedBy_Id,
                            CustomerName= customer.CompanyName
                        }
                        ).ToList();
            ViewBag.menuId = menuId;
            return View(data);
        }


    }
}