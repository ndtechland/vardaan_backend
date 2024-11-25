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
            var data = (from sm in ent.ShiftMasters
                        join tt in ent.TripTypes on sm.TripTypeId equals tt.Id
                        orderby sm.Id descending
                        select new GetShift
                        {
                            Id = sm.Id,
                            TripType = tt.TripTypeName,
                            ShiftTime = sm.ShiftTime
                        }
                       ).ToList();
             ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.ShiftMasters.Where(x => x.Id == id).FirstOrDefault();
                ViewBag.Id = existdata.Id;
                ViewBag.TripTypeId = existdata.TripTypeId;
                ViewBag.ShiftTime = existdata.ShiftTime;
                ViewBag.Heading = "Update Shift Time";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                ViewBag.Id = 0;
                ViewBag.TripTypeId = 0;
                ViewBag.ShiftTime = "";
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
                        ShiftTime = model.ShiftTime
                    };
                    ent.ShiftMasters.Add(shift);
                }
                else
                {
                    var data = ent.ShiftMasters.Find(model.Id);
                    data.TripTypeId = model.TripTypeId;
                    data.ShiftTime = model.ShiftTime;

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
    }
}