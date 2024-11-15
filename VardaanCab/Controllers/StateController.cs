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
    [Authorize]
    public class StateController : Controller
    {
        // GET: State
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new StateMasterVm();
            var data = ent.StateMasters.OrderByDescending(a => a.Id).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.StateName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.States = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            ViewBag.menuId = menuId;
            return View();
        }

        [HttpPost]
        public ActionResult Add(StateMasterDTO model)
        {
            try
            {
               if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<StateMaster>(model);
                ent.StateMasters.Add(state);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";                
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.StateMasters.Find(id);
            var model = Mapper.Map<StateMasterDTO>(data);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StateMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<StateMaster>(model);
                ent.Entry(state).State=System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
                return RedirectToAction("All",new { menuId =model.MenuId});
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return View(model);
            }
        }

        public ActionResult Delete(int id,int menuId=0)
        {
            try
            {
                var data = ent.StateMasters.Find(id);
                ent.StateMasters.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch(Exception ex)
            {
                return Content("Server error");
            }
        }

    }
}