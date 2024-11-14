using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class DriverLeaveController : Controller
    {
        // GET: DriverLeave
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();

        public ActionResult AssignLeave(int menuId=0)
        {
            var model = new DriverLeaveDTO();
            model.Drivers = cr.GetDriver().Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            ViewBag.menuId = menuId;
            model.MenuId= menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignLeave(DriverLeaveDTO model)
        {
            var driverlist = cr.GetDriver();
            model.Drivers = driverlist.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString() }).ToList();
            var driverInfo = driverlist.Where(x => x.Id == model.Driver_Id).FirstOrDefault();
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var driverLeave = Mapper.Map<DriverLeave>(model);
                driverLeave.CreateDate = DateTime.Now;
                var user = ent.UserLogins.Find(cr.GetUserDetailId());
                driverLeave.UpdatedBy = user.Email;
                ent.DriverLeaves.Add(driverLeave);
                ent.SaveChanges();

                //send msg to Driver               
                string leavemsg = "Vardaan Car Rental Services Pvt Ltd" + "\n";
                leavemsg += "Driver Name: " + driverInfo.DriverName;
                leavemsg += "\nLeave from: " + model.DateFrom.ToString("dd/MM/yyyy")+ " To " + model.DateTo.ToString("dd/MM/yyyy");
                leavemsg += "\nReason: " + model.Description;

                if (!String.IsNullOrEmpty(driverInfo.MobileNumber))
                {
                    Utilities.SmsOperation.SendSms(driverInfo.MobileNumber, leavemsg, "1107161555788579678");
                }
                TempData["msg"] = "Records has saved";



            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("AssignLeave",new { menuId =model.MenuId});
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var dl = ent.DriverLeaves.Find(id);
            var model = Mapper.Map<DriverLeaveDTO>(dl);
            model.Drivers = ent.Drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString(),Selected=a.Id==dl.Driver_Id }).ToList();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DriverLeaveDTO model)
        {
            model.Drivers = ent.Drivers.Select(a => new SelectListItem { Text = a.DriverName + " (" + a.MobileNumber + " )", Value = a.Id.ToString(), Selected = a.Id == model.Driver_Id }).ToList();
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var driverLeave = Mapper.Map<DriverLeave>(model);
                driverLeave.CreateDate = DateTime.Now;
                var user = ent.UserLogins.Find(cr.GetUserDetailId());
                driverLeave.UpdatedBy = user.Email;
                ent.Entry(driverLeave).State = System.Data.Entity.EntityState.Modified ;
                ent.SaveChanges();
                TempData["msg"] = "Records has saved";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit",new { id=model.Id,menuId=model.MenuId});
        }

        public ActionResult DriverLeaves(string term = "", int page = 1,int menuId=0)
        {
            var model = new ViewDriverLeaveDetail();
            var data = (from dl in ent.DriverLeaves
                        join d in ent.Drivers on dl.Driver_Id equals d.Id
                        select new DriverLeaveDTO
                        {
                            Id=dl.Id,
                            CreateDate=dl.CreateDate,
                            DateFrom=dl.DateFrom,
                            DateTo=dl.DateTo,
                            Description=dl.Description,
                            DriverName=d.DriverName,
                            Mobile=d.MobileNumber,
                            Driver_Id=d.Id,
                            UpdatedBy=dl.UpdatedBy
                        }
                        ).ToList().OrderByDescending(x=>x.DateFrom).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.DriverName.ToLower().Contains(term) || a.Mobile.Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.DriverLeaves = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

    }
}