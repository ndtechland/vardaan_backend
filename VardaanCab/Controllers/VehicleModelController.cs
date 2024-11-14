using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class VehicleModelController : Controller
    {
        // GET: VehicleModel
        DbEntities ent = new DbEntities();

        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new VehicleViewModel();
            var data = ent.VehicleModels.OrderByDescending(a => a.Id).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.ModelName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.VModels = data;
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
        public ActionResult Add(VehicleModelDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<VehicleModel>(model);
                ent.VehicleModels.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.VehicleModels.Find(id);
            var model = Mapper.Map<VehicleModelDTO>(data);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VehicleModelDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<VehicleModel>(model);
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
                return RedirectToAction("All",new { menuId=model.MenuId});
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return RedirectToAction("Edit", new { id=model.Id, menuId = model.MenuId });

            }
        }

        public ActionResult Delete(int id, int menuId = 0)
        {
            try
            {
                var data = ent.VehicleModels.Find(id);
                ent.VehicleModels.Remove(data);
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