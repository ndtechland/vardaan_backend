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
        public ActionResult ShiftTime(int id=0)
        {
            var model = new ShiftDTO();
            var data = ent.ShiftMasters.ToList();
            model.ShiftList = data;
           // ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.ShiftMasters.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.TripTypeId = existdata.TripTypeId;
                model.ShiftTime = existdata.ShiftTime;
                ViewBag.Heading = "Update Shift Time";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.TripTypeId = 0;
                model.ShiftTime = "";
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