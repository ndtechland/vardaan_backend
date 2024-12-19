using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;

namespace VardaanCab.Controllers
{
    public class CompanyZoneETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly ICompanyZone _companyZone;
        public CompanyZoneETSController(ICompanyZone companyZone)
        {
            _companyZone = companyZone;
        }
        // GET: CompanyZoneETS
        public ActionResult AddCompanyZone(int menuId = 0, int id = 0)
        {
            var model=new CompanyZoneDTO();
            model.Companies = new SelectList(ent.Customers.Where(x=>x.IsActive==true).OrderByDescending(x=>x.Id).ToList(), "Id", "OrgName");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.CompanyZones.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.CompanyId = existdata.CompanyId;
                model.CompanyZone = existdata.CompanyZone1;
                model.Zonelatlong = existdata.Zonelatlong;
                ViewBag.Heading = "Update Comapny Zone";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.CompanyZone = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Comapny Zone";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddCompanyZone(CompanyZoneDTO model)
        {
          
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool isCreated = await _companyZone.AddUpdateZone(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["msg"] = "Failed to save the record. Please try again.";
                }

                return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error. Please try again later.";
                return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
            }
        }

        public async Task<ActionResult> AllCompanyZones(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CompanyZoneDTO();    
            ViewBag.menuId = menuId;
            model.CompanyZoneLists = await _companyZone.GetZones();
            return View(model);
        }
        public async Task<ActionResult> DeleteCompanyZone(int id)
        {
            try
            {
                bool isDeleted= await _companyZone.DeleteZone(id);               
                TempData["dltmsg"] ="Deleted successfully.";
                return RedirectToAction("AllCompanyZones");

            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult HomeRoute(int menuId = 0, int id = 0)
        {
            var model = new EmployeeHomeRouteDTO();
            model.CompanyZones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.CompanyZoneHomeRoutes.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.CompanyZoneId = existdata.CompanyZoneId;
                model.HomeRouteName = existdata.HomeRouteName;
                ViewBag.Heading = "Update Home Route";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyZoneId = 0;
                model.HomeRouteName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Home Route";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> HomeRoute(EmployeeHomeRouteDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                bool isCreated = await _companyZone.AddUpdateHomeRoute(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["msg"] = "Failed to save the record. Please try again.";
                }


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("HomeRoute", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> AllHomeRoute(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeHomeRouteDTO();
           
            ViewBag.menuId = menuId;
            model.HomeRouteLists = await _companyZone.GetHomeRoutes();
            return View(model);
        }
        public async Task<ActionResult> DeleteHomeRoute(int id)
        {
            try
            {
                bool isDeleted = await _companyZone.DeleteZone(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("AllHomeRoute");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult EmployeeDestinationArea(int menuId = 0, int id = 0)
        {
            var model = new EmployeeDestinationAreaDTO();
            model.HomeRoutes = new SelectList(ent.CompanyZoneHomeRoutes.ToList(), "Id", "HomeRouteName");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.EmployeeDestinationAreas.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.HomeRouteId = existdata.CompanyZoneHomeRouteId;
                model.DestinationAreaName = existdata.DestinationAreaName;
                ViewBag.Heading = "Update Destination Area";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.HomeRouteId = 0;
                model.DestinationAreaName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Destination Area";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> EmployeeDestinationArea(EmployeeDestinationAreaDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                bool isCreated = await _companyZone.AddUpdateDestinationArea(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["msg"] = "Failed to save the record. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("EmployeeDestinationArea", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> AllEmployeeDestinationArea(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeDestinationAreaDTO();

            ViewBag.menuId = menuId;
            model.DestinationAreaLists = await _companyZone.GetDestinationAreas();
            return View(model);
        }
        public async Task<ActionResult>DeleteEmployeeDestinationArea(int id)
        {
            try
            {
                bool isDeleted = await _companyZone.DeleteDestinationArea(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("AllEmployeeDestinationArea");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}