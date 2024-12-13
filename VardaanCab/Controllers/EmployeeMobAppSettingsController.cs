using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class EmployeeMobAppSettingsController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        // GET: EmployeeMobAppSettings
        public ActionResult CabBookingRequestDay(int menuId = 0, int id = 0)
        {
            var model = new EmployeeMobAppSettingDTO();
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");
            var datalist = (from es in ent.EmployeeMobileAppSettings
                        join c in ent.Customers on es.CompanyId equals c.Id
                        where es.IsActive==true
                        orderby es.Id descending
                        select new CabReqDays
                        {
                            Id = es.Id,
                            CompanyName = c.CompanyName,
                            CompanyId = es.CompanyId,
                            CreatedDate = es.CreatedDate,
                            CabRequestDays = es.CabRequestDays
                        }
                      ).ToList();
            model.CabReqDayList = datalist;


            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.EmployeeMobileAppSettings.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.CompanyId = data.CompanyId;
                model.CabRequestDays = data.CabRequestDays;
                ViewBag.Heading = "Update Cab Request Day";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.CabRequestDays = null;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Cab Request Day";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult CabBookingRequestDay(EmployeeMobAppSettingDTO model)
        {
            try
            {

                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var checkinfo = ent.EmployeeMobileAppSettings.Where(x => x.CompanyId == model.CompanyId).FirstOrDefault();
                    if (checkinfo != null)
                    {
                        TempData["Errormsg"] = "Already exist.";
                        return RedirectToAction("CabBookingRequestDay", new { menuId = model.MenuId });
                    }
                    var EmpcabReqday = new EmployeeMobileAppSetting()
                    {
                        CompanyId = model.CompanyId,
                        CabRequestDays = model.CabRequestDays,
                        IsActive=true,
                        CreatedDate = DateTime.Now

                    };
                    ent.EmployeeMobileAppSettings.Add(EmpcabReqday);
                }
                else
                {
                    var data = ent.EmployeeMobileAppSettings.Find(model.Id);
                    data.CompanyId = model.CompanyId;
                    data.CabRequestDays = model.CabRequestDays;
                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";

            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("CabBookingRequestDay", new { menuId = model.MenuId });
        }
        public ActionResult DeleteCabRequestDay(int id)
        {
            try
            {
                var data = ent.EmployeeMobileAppSettings.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("CabBookingRequestDay");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}