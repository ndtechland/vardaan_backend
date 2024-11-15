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
    public class CabCategoryController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public ActionResult All(string term = "", int page = 1)
        {
            var model = new CabCategoryVm();
            var data = ent.CabCategories.OrderByDescending(a => a.Id).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.CabCategoryName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Categories = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CabCategoryDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<CabCategory>(model);
                ent.CabCategories.Add(state);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add");
        }

        public ActionResult Edit(int id)
        {
            var data = ent.CabCategories.Find(id);
            var model = Mapper.Map<CabCategoryDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CabCategoryDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<CabCategory>(model);
                ent.Entry(state).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = ent.CabCategories.Find(id);
                ent.CabCategories.Remove(data);
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