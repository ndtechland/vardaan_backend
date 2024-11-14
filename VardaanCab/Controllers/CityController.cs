using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CityMasterVm();
            var data = (from c in ent.CityMasters join s in ent.StateMasters
                        on c.StateMaster_Id equals s.Id
                        orderby c.Id descending
                        select new CityMasterDTO
                        {
                            Id=c.Id,
                            CityName=c.CityName,
                            StateMaster_Id=c.StateMaster_Id,
                            StateName=s.StateName
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.CityName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Cities = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new CityMasterDTO();
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CityMasterDTO model)
        {
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<CityMaster>(model);
                ent.CityMasters.Add(state);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var data = ent.CityMasters.Find(id);
            var model = Mapper.Map<CityMasterDTO>(data);
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName",data.StateMaster_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CityMasterDTO model)
        {
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", model.StateMaster_Id);
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<CityMaster>(model);
                ent.Entry(state).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
                return RedirectToAction("All",new { menuId=model.MenuId});
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return View(model);
            }
        }

        public ActionResult Delete(int id, int menuId = 0)
        {
            try
            {
                var data = ent.CityMasters.Find(id);
                ent.CityMasters.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

       
    }
}