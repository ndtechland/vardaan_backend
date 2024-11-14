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
    public class StateGSTINController : Controller
    {
        // GET: StateGSTIN
        DbEntities ent = new DbEntities();
        StateMasterGstinRepository swg = new StateMasterGstinRepository();

        public ActionResult ChangeStatus(int id,int menuId=0)
        {
            string query = @"update StateWiseGSTIN set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("All",new { menuId=menuId});
        }

        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new StateWiseGSTINViewModel();
            var data = swg.GetStateMasterGstinList().ToList();
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.StateName.ToLower().Contains(term) || a.Gstin==term).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.StatesGstin = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new StateWiseGstinDTO();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(StateWiseGstinDTO model)
        {
            try
            {
                model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<StateWiseGSTIN>(model);
                
                data.IsActive = true;
                ent.StateWiseGSTINs.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add", new { menuId = model.MenuId });
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var data = ent.StateWiseGSTINs.Find(id);
            var model = Mapper.Map<StateWiseGstinDTO>(data);
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", data.State_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StateWiseGstinDTO model)
        {
            try
            {
                model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", model.State_Id);
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<StateWiseGSTIN>(model);
                ent.Entry(state).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });

        }

        public ActionResult Delete(int id,int menuId=0)
        {
            try
            {
                var data = ent.StateWiseGSTINs.Find(id);
                ent.StateWiseGSTINs.Remove(data);
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