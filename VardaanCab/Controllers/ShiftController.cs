using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class ShiftController : Controller
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        // GET: Shift
        public ActionResult ShiftTime(int menuId = 0, int id = 0)
        {
             var model = new ShiftDTO();
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            var data = (from sm in ent.ShiftMasters
                        join tt in ent.TripTypes on sm.TripTypeId equals tt.Id
                        join c in ent.Customers on sm.CompanyId equals c.Id
                        join dm in ent.DepartmentMasters on sm.DepartmentId equals dm.Id
                        join cz in ent.CompanyZones on sm.CompanyZoneId equals cz.Id
                        orderby sm.Id descending
                        select new GetShift
                        {
                            Id = sm.Id,
                            TripType = tt.TripTypeName,
                            CustomerName = c.CustomerName,
                            CompanyZoneName = cz.CompanyZone1,
                            DepartmentName = dm.DepartmentName,
                            ShiftBufferTime = sm.ShiftBufferTime,
                            ShiftTime = sm.ShiftTime
                        }
                       ).ToList();
            model.ShiftList = data;
             ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.ShiftMasters.Where(x => x.Id == id).FirstOrDefault();
                ViewBag.Id = existdata.Id;
                ViewBag.TripTypeId = existdata.TripTypeId;
                ViewBag.ShiftTime = existdata.ShiftTime;
                model.ShiftBufferTime = existdata.ShiftBufferTime;
                model.CompanyId = existdata.CompanyId;
                model.CompanyZoneId = existdata.CompanyZoneId;
                model.DepartmentId = existdata.DepartmentId;
                ViewBag.DepartmentId = existdata.DepartmentId;
                ViewBag.CompanyZoneId = existdata.CompanyZoneId;
                ViewBag.Heading = "Update Shift Time";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                ViewBag.Id = 0;
                ViewBag.TripTypeId = 0;
                ViewBag.ShiftTime = "";
                model.ShiftBufferTime = "";
                model.CompanyId = 0;
                ViewBag.CompanyZoneId = 0;
                ViewBag.DepartmentId = 0;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Shift Time";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult ShiftTime(ShiftMaster model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var shift = new ShiftMaster()
                    {
                        TripTypeId = model.TripTypeId,
                        ShiftTime = model.ShiftTime,
                        CompanyId = model.CompanyId,
                        DepartmentId = model.DepartmentId,
                        ShiftBufferTime = model.ShiftBufferTime,
                        CompanyZoneId = model.CompanyZoneId
                    };
                    ent.ShiftMasters.Add(shift);
                }
                else
                {
                    var data = ent.ShiftMasters.Find(model.Id);
                    data.TripTypeId = model.TripTypeId;
                    data.ShiftTime = model.ShiftTime;
                    data.CompanyId = model.CompanyId;
                    data.DepartmentId = model.DepartmentId;
                    data.ShiftBufferTime = model.ShiftBufferTime;
                    data.CompanyZoneId = model.CompanyZoneId;

                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("ShiftTime");
        }
        public ActionResult DeleteShiftTime(int id)
        {
            try
            {
                var data = ent.ShiftMasters.Find(id);
                ent.ShiftMasters.Remove(data);
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("ShiftTime");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}