using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    public class AssignController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository cr = new CommonRepository();
        // GET: Assign

        public ActionResult AssignCab(int id, int? pDriver_Id, int? pCab_Id, DateTime? sDate, DateTime? eDate, int page = 1, string term = "", int menuId = 0, string bookingStatus= "", int pbookingCat = 2)
        {
            var booking = ent.Bookings.Find(id);
            var desireCar = ent.VehicleModels.Find(booking.VehicleModel_Id).ModelName;
            var model = new AssignCabModel();
            model.BookingStatus = bookingStatus;
            model.PickupTime = booking.PickupTime;
            ViewBag.pendingCat = pbookingCat;
            if ( pCab_Id != null)
            {
                model.PCab_Id = pCab_Id;
                var cab = ent.Cabs.Find(pCab_Id);
                var vehicle = ent.VehicleModels.Find(cab.VehicleModel_Id);
                model.cabName = vehicle.ModelName + "(" + cab.VehicleNumber + ")";
            }

            if (booking.StartKms>0 && booking.StartHour!=null)
            {
                model.StartKms = booking.StartKms;
                model.StartHour = booking.StartHour;
            }
            model.Booking_Id = id;
            var drivers = cr.GetDriver();
            if (pDriver_Id != null)
            {
                model.PDriver_Id = pDriver_Id;
                var dr = ent.Drivers.Find(pDriver_Id);
                if (dr != null)
                    drivers.Add(dr);
            }
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString(), Selected = a.Id == model.PDriver_Id }).ToList();
            model.MenuId = menuId;
            model.VehicleModels = new SelectList(ent.VehicleModels.OrderBy(a => a.ModelName).ToList(), "Id", "ModelName");
            model.Vendors = new SelectList(ent.Vendors.ToList(), "Id", "CompanyName");
            model.DesiredCar = desireCar;
            model.Term = term;
            model.Page = page;
            model.sDate = sDate;
            model.eDate = eDate;
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignCab(AssignCabModel model) 
        {
            var pCategory = Request["pendingBCat"];
            var data = ent.Bookings.Find(model.Booking_Id);
            model.VehicleModels = new SelectList(ent.VehicleModels.OrderBy(a => a.ModelName).ToList(), "Id", "ModelName");
            model.Vendors = new SelectList(ent.Vendors.ToList(), "Id", "CompanyName");
            var drivers = cr.GetDriver();
          
            if (model.PDriver_Id != null)
            {
                var dr = ent.Drivers.Find(model.PDriver_Id);
                if (dr != null)
                    drivers.Add(dr);
            }
            else
            {
                ModelState.Remove("Driver_Id");
            }
            if (model.PCab_Id != null)
            {
                ModelState.Remove("Cab_Id");
            }
            model.Drivers = drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString(), Selected = a.Id == model.PDriver_Id }).ToList();

            //Add by Bhupesh            
            if (data.BookingStatus == ((int)BookingStatus.Dispatched) && data.DriverId==model.Driver_Id && data.Cab_Id==model.Cab_Id)
            {
                TempData["msg"] = "Booking is already dispatched.Please check.";
                return View(model);
            }
            //End check alreadt dispached

            if (model.Cab_Id == 0 && model.Driver_Id == null)
            {
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

           

            string bookerMobile = data.BookedByPersonMobileNo;
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (data != null)
                    {
                        var user = ent.UserLogins.Find(cr.GetUserDetailId());
                        //update driver id and cab id
                        if (model.Cab_Id > 0)
                            data.Cab_Id = model.Cab_Id;
                        if (model.Driver_Id != null)
                            data.DriverId = model.Driver_Id;
                        data.StartHour = model.StartHour;
                        data.StartKms = model.StartKms;
                        data.UpdatedBy = user.Id;
                        data.BookingStatus = (int)BookingStatus.Dispatched;
                        data.UpdateDate = DateTime.Now;
                        ent.SaveChanges();

                        
                        if (model.PCab_Id != null && model.PCab_Id != model.Cab_Id)
                        {
                            var pvehicle = ent.Cabs.Find(model.PCab_Id);
                            pvehicle.IsAvailable = true;
                            ent.SaveChanges();
                        }

                        if (model.PDriver_Id != null && model.PDriver_Id != model.Driver_Id)
                        {
                            var pdriver = ent.Drivers.Find(model.PDriver_Id);
                            pdriver.IsAvailable = true;
                            ent.SaveChanges();
                        }

                        var vehicle = new Cab();
                        var vm = new VehicleModel();
                        var driver = new Driver();

                        var modelName = "";
                        var vehicleNumber = "";
                        var driverName = "";
                        var driverMobile = "";

                        string pLandmark = "";
                        string dLandmark = "";

                        vehicle = ent.Cabs.Find(model.Cab_Id > 0 ? model.Cab_Id : model.PCab_Id);
                         driver = ent.Drivers.Find(model.Driver_Id != null ? model.Driver_Id : model.PDriver_Id);

                        if (vehicle!=null)
                        {
                            vehicle.IsAvailable = false;
                            vm = ent.VehicleModels.Find(vehicle.VehicleModel_Id);
                            modelName = ent.VehicleModels.Where(a => a.Id == vehicle.VehicleModel_Id).FirstOrDefault().ModelName;
                            vehicleNumber = vehicle.VehicleNumber;
                        }
                        
                        
                        if(driver!=null)
                        {
                            driver.IsAvailable = false;
                            driverName = driver.DriverName;
                            driverMobile = driver.MobileNumber;
                        }

                       
                        var log = new Log
                        {
                            Booking_Id = data.Id,
                            UpdateDate = DateTime.Now,
                            UserLogin_Id = user.Id,
                            Description = "Driver : " + driverName + ",Vehicle :  " + modelName  + " " + vehicleNumber + "  is assigned to Booking (" + data.BookingId + ") by " + user.Email+ " on " + cr.GetISTDate().ToString("dd-MMM-yyyy") + " " + cr.GetISTDate().ToShortTimeString()
                        };
                        ent.Logs.Add(log);

                        ent.SaveChanges();
                        tran.Commit();



                        // customer sms
                        string usermsg = "Vardaan Car Rental Services Pvt Ltd" + "\n";
                        usermsg += "Dear Mr/Ms:"+data.CustomerName;
                        usermsg += "\nDriver Name: " + driverName  + " \nCell.: " + driverMobile;
                        usermsg += "\nCar: " + modelName ;
                        usermsg += "\nNo.: " + vehicleNumber;
                        usermsg +="\nTime: " + data.ReportingTime;
                        usermsg += "\nBID: " + data.BookingId;
                        usermsg += "\nCall 24x7:8130874555";

                       


                        string userEmailMsg = $@"Dear Sir / Madam,<br>
Greetings from VARDAAN Car Rental Services !!!!<br/>
Chauffeur and Cab details of your booking for Mr / Ms. {data.CustomerName} is as below: <br/>
*************************************************<br> 
Guest Name: Mr/Ms. {data.CustomerName} <br/>

Driver Name: {driverName} <br/>

Cell:{driverMobile} <br/>
Cab:{modelName} <br/>
Cab No.:{vehicleNumber} <br/>

Date: {data.PickupDate.ToString("dd-MMM-yyyy")} <br/>
Time:{data.PickupTime} <br/>
Booking ID:{data.BookingId} <br/>
Call24X7:8130874555<br/>
************************************************<br/>

For any further clarifications, write to us or call us at 24x7 Helpline numbers, mentioned below.<br/>
LL:   0120-4204668; Mo:  08130874555; email: reservation@vardaanrentacar.com 
<br/><br/>
Thanks and kind regards
<br/><br/>
Vardaan Team 
";
                        //*****Send multiple email
                       
                        string[] mEmail = data.Email.Split(',');
                        foreach (string bEmail in mEmail)
                        {
                           // Console.WriteLine(author);
                            var res = EmailOperation.SendEmail(bEmail, "Booking Detail", userEmailMsg, true);
                        }
                        //*****End Multiple mail


                        //var res = EmailOperation.SendEmail(data.Email, "Booking Detail", userEmailMsg, true);

                        if (data.PickupLandMark != null)
                        {
                            pLandmark = data.PickupLandMark;
                        }

                        if (data.DropLandmark != null)
                        {
                            dLandmark = data.DropLandmark;
                        }

                        //driver sms
                        //string drivermsg = "Vardaan Car Rental Services Pvt Ltd" + "\n";
                        //drivermsg += "BID: " + data.BookingId;
                        ////string drivermsg =  "BID: " + data.BookingId;
                        //drivermsg += "\n" + "Name: " + data.CustomerName + " \nCell: " + data.ContactNo;
                        //drivermsg += "\n" + "Company name: " + data.CompanyName;
                        //drivermsg += "\nAdd: " + data.PickupAddress + " " + pLandmark;
                        //drivermsg += "\nDrop: " + data.DropAddress + " " + dLandmark;
                        //drivermsg += "\nDate: " + data.PickupDate.ToString("dd-MMM-yyyy");
                        //drivermsg += "\nTime: " + data.ReportingTime;

                        string[] gustcontact = data.ContactNo.Split(',');

                        string drivermsg = "Vardaan Car Rental Services Pvt Ltd" + "\n";
                        drivermsg += "BID: " + data.BookingId;
                        // drivermsg += "\n" + "Name: " + data.CustomerName + "\nCell: " + data.ContactNo;
                        drivermsg += "\n" + "Name: " + data.CustomerName + "\nCell: " + gustcontact[0];

                        drivermsg += "\n" + "Company name: " + data.CompanyName; 

                        drivermsg += "\nAdd: " + data.PickupAddress + " " + pLandmark;
                        drivermsg += "\nDrop: " + data.DropAddress + " " + dLandmark;
                        drivermsg += "\nDate: " + data.PickupDate.ToString("dd-MMM-yyyy");
                        drivermsg += "\nTime: " + data.ReportingTime;


                        //                        string drivermsg = @"Vardaan Car Rental Services Pvt Ltd
                        //BID: var
                        //Name: var
                        //Cell: var
                        //Company name: var
                        //Add: var
                        //Drop: var
                        //Date: var
                        //Time: var";

                        if (driver!=null && vm!=null&& vehicle!=null)
                        {
                            //SmsOperation.SendSms(data.ContactNo, usermsg, "1107161561680288814");

                            string[] mcontact = data.ContactNo.Split(',');
                            foreach (string bContact in mcontact)
                            {
                                // SmsOperation.SendSms(bContact, usermsg, "1107161561680288814");1107164621623290390
                                SmsOperation.SendSms(bContact, usermsg, "1107164621623290390");
                            }
                        }
                        if(driver!=null)
                        {
                            //SmsOperation.SendSms(driver.MobileNumber, drivermsg, "1107164551583701465");
                            SmsOperation.SendSms(driver.MobileNumber, drivermsg, "1107164621723111785");
                        }
                        //update by bhupesh send msg to booker
                        if (!String.IsNullOrEmpty(bookerMobile))
                        {
                            //SmsOperation.SendSms(bookerMobile, usermsg, "1107161561680288814");
                            SmsOperation.SendSms(bookerMobile, usermsg, "1107164621623290390");
                        }
                    }
                    if (data.PackageType_Id == 1 || data.PackageType_Id == 2 || data.PackageType_Id == 3)
                    {
                        return RedirectToAction("ShowBooking", "Booking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, bookingStatus = model.BookingStatus, pbookingCat = pCategory, menuId = model.MenuId });
                    }
                    else
                    {
                        return RedirectToAction("ShowBooking", "Booking", new { sDate = model.sDate, eDate = model.eDate, page = model.Page, term = model.Term, bookingStatus = model.BookingStatus, pbookingCat = pCategory, menuId = model.MenuId });
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Server error";
                    return View(model);
                }
            }
        }

        public ActionResult GetCabList(string term)
        {
            //           string query = @"select cb.Id,cb.VehicleNumber,vm.ModelName from Cab cb
            //join VehicleModel vm on cb.VehicleModel_Id = vm.Id 
            //where vm.ModelName like'"+term+"%' and cb.IsActive =1 and cb.IsAvailable=1";
            //           var data = ent.Database.SqlQuery<CabDTO>(query).ToList();

            //var data = (from c in ent.Cabs
            //            join vm in ent.VehicleModels on c.VehicleModel_Id equals vm.Id
            //            join cp  in ent.CorporatePackages on c.VehicleModel_Id equals cp.VehicleModel_Id
            //            join rt in ent.RentalTypes on cp.RentalType_Id equals rt.Id
            //            where vm.ModelName.ToLower().StartsWith(term.ToLower()) &&  c.IsActive == true  && c.IsAvailable == true
            //            select new CabDTO
            //            {
            //                Id = c.Id,
            //                ModelName = vm.ModelName,
            //                VehicleNumber = c.VehicleNumber
            //            }
            //            ).ToList();
            //            string query = @"select c.Id,vm.ModelName,c.VehicleNumber from cab c join VehicleModel vm on c.VehicleModel_Id=vm.Id
            //where c.IsAvailable=1 and c.IsActive=1 and (vm.ModelName like '" + term.ToLower() + "%' or c.VehicleNumber like '%" + term.ToLower() + "%')";

            string query = @"select c.Id,vm.ModelName,c.VehicleNumber from cab c join VehicleModel vm on c.VehicleModel_Id=vm.Id
            where c.IsAvailable=1 and c.IsActive=1 and (vm.ModelName like '%" + term.ToLower() + "%' or c.VehicleNumber like '%" + term.ToLower() + "%')";
            var data = ent.Database.SqlQuery<CabDTO>(query).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateDriver(CreateDriverModel model)
        {
            if (!ModelState.IsValid)
                return Content("Invalid Data");
            if (ent.Drivers.Any(a => a.MobileNumber.ToLower().Equals(model.MobileNumber.ToLower())))
            {
                //TempData["msg"] = "Driver Mobile Number is already exist in our database";
                return Content("Driver and Mobile Number is already exist in our database");
                //return View(model);
            }
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var driver = new Driver
                    {
                        DriverName = model.DriverName,
                        MobileNumber = model.MobileNumber,
                        CreateDate = cr.GetISTDate(),
                        DriverAddress = string.IsNullOrEmpty(model.Address) ? "n/a" : model.Address,
                        IsActive = true,
                        IsAvailable = true,
                        LicenceExpDate = cr.GetISTDate().AddDays(30),
                        IsOutsider = true
                    };
                    ent.Drivers.Add(driver);
                    ent.SaveChanges();

                    var vendor = new Vendor
                    {
                        CompanyName = model.DriverName,
                        VendorName = model.DriverName,
                        CreateDate = cr.GetISTDate(),
                        CityMaster_Id = 0,
                        StateMaster_Id = 0,
                        MobileNumber = model.MobileNumber,
                        IsActive = true,
                        IsOutsider = true
                    };
                    ent.Vendors.Add(vendor);
                    ent.SaveChanges();

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

        public ActionResult CreateCab(CreateCabModel model)
        {
            if (!ModelState.IsValid)
                return Content("Invalid Data");
            if (ent.Cabs.Any(a => a.VehicleNumber.ToLower().Equals(model.VehicleNumber.ToLower())))
            {
                return Content("Vehicle number already exist in our database");
                
            }

            try
            {
                var cab = new Cab
                {
                    Company = model.Company,
                    VehicleModel_Id = model.VehicleModel_Id,
                    VehicleNumber = model.VehicleNumber.ToUpper(),
                    Vendor_Id = model.Vendor_Id,
                    IsActive = true,
                    IsAvailable = true,
                    CreateDate = cr.GetISTDate(),
                    FitnessVality = cr.GetISTDate().AddYears(2),
                    PolutionValidity = cr.GetISTDate().AddMonths(3),
                    InsurranceValidity = cr.GetISTDate().AddYears(1),
                    PermitValidity = cr.GetISTDate().AddDays(30),
                    IsOutsider = true
                };
                ent.Cabs.Add(cab);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

        //public ActionResult CreateCabDriver(CreateCabDriverModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return Content("Invalid Data");
        //    using (var tran = ent.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var driver = new Driver
        //            {
        //                DriverName = model.DriverName,
        //                MobileNumber = model.MobileNumber,
        //                CreateDate = cr.GetISTDate(),
        //                DriverAddress = string.IsNullOrEmpty(model.Address) ? "n/a" : model.Address,
        //                IsActive = true,
        //                IsAvailable = true,
        //                LicenceExpDate = cr.GetISTDate().AddDays(30),
        //                IsOutsider = true
        //            };
        //            ent.Drivers.Add(driver);
        //            ent.SaveChanges();

        //            var vendor = new Vendor
        //            {
        //                CompanyName = model.DriverName,
        //                VendorName = model.DriverName,
        //                CreateDate = cr.GetISTDate(),
        //                CityMaster_Id = 0,
        //                StateMaster_Id = 0,
        //                MobileNumber = model.MobileNumber,
        //                IsActive = true,
        //                IsOutsider = true
        //            };
        //            ent.Vendors.Add(vendor);
        //            ent.SaveChanges();
        //            var cab = new Cab
        //            {
        //                Company = model.Company,
        //                VehicleModel_Id = model.VehicleModel_Id,
        //                VehicleNumber = model.VehicleNumber,
        //                Vendor_Id = vendor.Id,
        //                IsActive = true,
        //                IsAvailable = true,
        //                CreateDate = cr.GetISTDate(),
        //                FitnessVality = cr.GetISTDate().AddDays(30),
        //                PolutionValidity = cr.GetISTDate().AddDays(30),
        //                InsurranceValidity = cr.GetISTDate().AddDays(30),
        //                PermitValidity = cr.GetISTDate().AddDays(30),
        //                IsOutsider = true
        //            };
        //            ent.Cabs.Add(cab);
        //            ent.SaveChanges();
        //            tran.Commit();
        //            return Content("ok");

        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            return Content("Server error");
        //        }
        //    }


        //}




    }
}