using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class EmployeeMobAppSettingsController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly IEmployeeMobAppSettings _employeeMobAppSettings;
        public EmployeeMobAppSettingsController(IEmployeeMobAppSettings employeeMobAppSettings)
        {
            _employeeMobAppSettings = employeeMobAppSettings;
        }
        // GET: EmployeeMobAppSettings
        public async Task<ActionResult> CabBookingRequestDay(int menuId = 0, int id = 0)
        {
            var model = new EmployeeMobAppSettingDTO();
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");
           
            model.CabReqDayList = await _employeeMobAppSettings.GetCabRequestDayList();


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
        public async Task<ActionResult> CabBookingRequestDay(EmployeeMobAppSettingDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool isCreated = await _employeeMobAppSettings.AddCabRequestDays(model);

                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["Errormsg"] = "Record already exists.";
                }

                return RedirectToAction("CabBookingRequestDay", new { menuId = model.MenuId });
            }
            catch (Exception ex)
            {
                TempData["Errormsg"] = "Server Error: " + ex.Message;
                return RedirectToAction("CabBookingRequestDay", new { menuId = model.MenuId });
            }
        }

        public async Task<ActionResult> DeleteCabRequestDay(int id)
        {
            try
            {
                bool isDeleted = await _employeeMobAppSettings.DeletedRequestDay(id);

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