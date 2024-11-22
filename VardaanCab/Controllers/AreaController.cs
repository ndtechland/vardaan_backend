using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;

namespace VardaanCab.Controllers
{
    public class AreaController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        // GET: Area
        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new AreaMasterVM();
            var data = (from a in ent.AreaMasters
                        join c in ent.CityMasters on a.CityMaster_Id equals c.Id
                        join s in ent.StateMasters on a.StateMaster_Id equals s.Id
                        orderby a.Id descending
                        select new AreaMasterDTO
                        {
                            Id = a.Id,
                            AreaName = a.AreaName,
                            StateName = s.StateName,
                            CityName = c.CityName
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.AreaName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Areas = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }
        public ActionResult Add(int menuId = 0, int id = 0)
        {
            var model = new AreaMasterDTO();
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.AreaMasters.Where(x => x.Id == id).FirstOrDefault();
                model.StateMaster_Id = existdata.StateMaster_Id;
                model.CityMaster_Id = existdata.CityMaster_Id;
                model.AreaName = existdata.AreaName;
                ViewBag.Heading = "Update Area";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.StateMaster_Id = 0;
                model.CityMaster_Id = 0;
                model.AreaName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Area";
                return View(model);
            }
            
        }

        [HttpPost]
        public ActionResult Add(AreaMasterDTO model)
        {
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if(model.Id==0)
                {
                    var area = new AreaMaster()
                    {
                        StateMaster_Id = model.StateMaster_Id,
                        CityMaster_Id = model.CityMaster_Id,
                        AreaName = model.AreaName
                    };
                    ent.AreaMasters.Add(area);
                    ent.SaveChanges();
                    TempData["msg"] = "Record has saved successfully.";
                }
                else
                {
                    var data = ent.AreaMasters.Find(model.Id);
                    data.AreaName=model.AreaName;
                    data.StateMaster_Id=model.StateMaster_Id;
                    data.CityMaster_Id=model.CityMaster_Id;
                    ent.SaveChanges();
                    TempData["msg"] = "Record has updated successfully.";
                }
                
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add", new { menuId = model.MenuId });
        }

        public ActionResult DeleteArea(int id)
        {
            try
            {
                var data = ent.AreaMasters.Find(id);
                ent.AreaMasters.Remove(data);
                ent.SaveChanges();
                TempData["dltmsg"] = "Record has updated successfully.";
                return RedirectToAction("All");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}