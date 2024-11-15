using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;

namespace VardaanCab.Controllers
{
    [Authorize(Roles ="admin")]
    public class CategoryController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult All(string term = "", int page = 1)
        {
            var model = new CategoryVm();
            var data = (from c in ent.Categories
                         join p in ent.Categories
on c.ParentCategory_Id equals p.Id into pc
from x in pc.DefaultIfEmpty()
                        orderby c.Id descending
                        select new CategoryDTO
                        {
                            Id = c.Id,
                            CategoryName = c.CategoryName,
                            ParentCategory = x!=null?x.CategoryName:"None",
                            ParentCategory_Id = c.ParentCategory_Id
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.CategoryName.ToLower().Contains(term)).ToList();
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
            var model = new CategoryDTO();
            
            model.Categories = new SelectList(commonRepo.GetCategoryWithParents(), "Id", "CategoryName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CategoryDTO model)
        {
            model.Categories = new SelectList(commonRepo.GetCategoryWithParents(), "Id", "CategoryName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<Category>(model);
                ent.Categories.Add(state);
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
            var data = ent.Categories.Find(id);
            var model = Mapper.Map<CategoryDTO>(data);
            model.Categories = new SelectList(commonRepo.GetCategoryWithParents(), "Id", "CategoryName",data.ParentCategory_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CategoryDTO model)
        {           
            model.Categories = new SelectList(commonRepo.GetCategoryWithParents(), "Id", "CategoryName",model.ParentCategory_Id);
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var state = Mapper.Map<Category>(model);
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
                var data = ent.Categories.Find(id);
                ent.Categories.Remove(data);
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