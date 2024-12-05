using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;

namespace VardaanCab.Controllers
{
    public class CompanyZoneETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        // GET: CompanyZoneETS
        public ActionResult AddCompanyZone(int menuId = 0, int id = 0)
        {
            var model=new CompanyZoneDTO();
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
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
        public ActionResult AddCompanyZone(CompanyZoneDTO model)
         {
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var zone = new CompanyZone()
                    {
                        CompanyId = model.CompanyId,
                        CompanyZone1 = model.CompanyZone,
                        CreatedDate = DateTime.Now,
                        Zonelatlong = model.Zonelatlong
                    };
                    ent.CompanyZones.Add(zone);                   
                }
                else
                {
                    var data = ent.CompanyZones.Find(model.Id);
                    data.CompanyZone1 = model.CompanyZone;
                    data.CompanyId = model.CompanyId;
                    data.Zonelatlong = model.Zonelatlong;


                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
        }
        public ActionResult AllCompanyZones(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CompanyZoneDTO();
            var data = (from cz in ent.CompanyZones
                        join c in ent.Customers on cz.CompanyId equals c.Id
                        orderby cz.Id descending
                        select new CompanyZoneList
                        {
                            Id = cz.Id,
                            CompanyZone = cz.CompanyZone1,
                            CompanyName = c.CustomerName,
                            CreatedDate = cz.CreatedDate
                        }
                        ).ToList();
            
            ViewBag.menuId = menuId;
            model.CompanyZoneLists = data;
            return View(model);
        }
        public ActionResult DeleteCompanyZone(int id)
        {
            try
            {
                var data = ent.CompanyZones.Find(id);
                ent.CompanyZones.Remove(data);
                ent.SaveChanges();
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
        public ActionResult HomeRoute(EmployeeHomeRouteDTO model)
        {
            model.CompanyZones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var zone = new CompanyZoneHomeRoute()
                    {
                        CompanyZoneId = model.CompanyZoneId,
                        HomeRouteName = model.HomeRouteName,
                        CreatedDate = DateTime.Now
                    };
                    ent.CompanyZoneHomeRoutes.Add(zone);
                }
                else
                {
                    var data = ent.CompanyZoneHomeRoutes.Find(model.Id);
                    data.CompanyZoneId = model.CompanyZoneId;
                    data.HomeRouteName = model.HomeRouteName;

                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("HomeRoute", new { menuId = model.MenuId });
        }
        public ActionResult AllHomeRoute(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeHomeRouteDTO();
            var data = (from hr in ent.CompanyZoneHomeRoutes
                        join cz in ent.CompanyZones on hr.CompanyZoneId equals cz.Id
                        orderby cz.Id descending
                        select new HomeRouteList
                        {
                            Id = hr.Id,
                            CompanyZone = cz.CompanyZone1,
                            HomeRouteName = hr.HomeRouteName,
                            CreatedDate = hr.CreatedDate
                        }
                        ).ToList();

            ViewBag.menuId = menuId;
            model.HomeRouteLists = data;
            return View(model);
        }
        public ActionResult DeleteHomeRoute(int id)
        {
            try
            {
                var data = ent.CompanyZoneHomeRoutes.Find(id);
                ent.CompanyZoneHomeRoutes.Remove(data);
                ent.SaveChanges();
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
        public ActionResult EmployeeDestinationArea(EmployeeDestinationAreaDTO model)
        {
            model.HomeRoutes = new SelectList(ent.CompanyZoneHomeRoutes.ToList(), "Id", "HomeRouteName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var zone = new EmployeeDestinationArea()
                    {
                        CompanyZoneHomeRouteId = model.HomeRouteId,
                        DestinationAreaName = model.DestinationAreaName,
                        CreatedDate = DateTime.Now
                    };
                    ent.EmployeeDestinationAreas.Add(zone);
                }
                else
                {
                    var data = ent.EmployeeDestinationAreas.Find(model.Id);
                    data.CompanyZoneHomeRouteId = model.HomeRouteId;
                    data.DestinationAreaName = model.DestinationAreaName;

                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("EmployeeDestinationArea", new { menuId = model.MenuId });
        }
        public ActionResult AllEmployeeDestinationArea(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeDestinationAreaDTO();
            var data = (from da in ent.EmployeeDestinationAreas
                        join c in ent.CompanyZoneHomeRoutes on da.CompanyZoneHomeRouteId equals c.Id
                        orderby da.Id descending
                        select new DestinationAreaList
                        {
                            Id = da.Id,
                            DestinationAreaName = da.DestinationAreaName,
                            HomeRouteName = c.HomeRouteName,
                            CreatedDate = da.CreatedDate
                        }
                        ).ToList();

            ViewBag.menuId = menuId;
            model.DestinationAreaLists = data;
            return View(model);
        }
        public ActionResult DeleteEmployeeDestinationArea(int id)
        {
            try
            {
                var data = ent.EmployeeDestinationAreas.Find(id);
                ent.EmployeeDestinationAreas.Remove(data);
                ent.SaveChanges();
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