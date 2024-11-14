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
    public class RentalTypeController : Controller
    {
        // GET: RentalType
        DbEntities ent = new DbEntities();

        public ActionResult All(string term = "", int page = 1,int menuId=0)
        {
            var model = new RentalTypeViewModel();
            var data = ent.Database.SqlQuery<RentalTypeDTO>(@"select rt.*,pt.PType as PackageTypeName from RentalType rt left join PackageType pt on rt.PackageType_Id=pt.Id").ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.RentalTypeName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Rentals = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new RentalTypeDTO();
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(),"Id", "PType");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(RentalTypeDTO model)
        {
            try
            {
                model.PackageTypes = new SelectList(ent.PackageTypes.AsNoTracking().ToList(), "Id", "PType");
                if (!ModelState.IsValid)
                    return View(model);
                if(ent.RentalTypes.Any(a=>a.RentalTypeName==model.RentalTypeName && a.PackageType_Id == model.PackageType_Id))
                {
                    TempData["msg"] = "RentalType can not be duplicate.";
                    return View(model);
                }
                var data = Mapper.Map<RentalType>(model);
                ent.RentalTypes.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId = 0)
        {
            var data = ent.RentalTypes.Find(id);
            var model = Mapper.Map<RentalTypeDTO>(data);
            model.PackageTypes = new SelectList(ent.PackageTypes.AsNoTracking().ToList(), "Id", "PType", model.PackageType_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RentalTypeDTO model)
        {
            try
            {
                model.PackageTypes = new SelectList(ent.PackageTypes.AsNoTracking().ToList(), "Id", "PType", model.PackageType_Id);
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<RentalType>(model);
                if(ent.RentalTypes.Any(a=>a.Id!=model.Id && a.RentalTypeName==model.RentalTypeName))
                {
                    TempData["msg"] = "RentalType can not be duplicate.";
                    return View(model);
                }
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
                return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });
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
                var data = ent.RentalTypes.Find(id);
                ent.RentalTypes.Remove(data);
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