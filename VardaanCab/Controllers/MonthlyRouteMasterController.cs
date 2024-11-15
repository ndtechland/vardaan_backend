using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;

namespace VardaanCab.Controllers
{
    public class MonthlyRouteMasterController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();

        public ActionResult CreatePackage(int menuId = 0)
        {
            var model = new MonthlyPackageRouteMasterDTO();
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePackage(MonthlyPackageRouteMasterDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (ent.MonthlyPackageRouteMasters.Any(a => a.VehicleModel_Id == model.VehicleModel_Id && a.PickupLocation == model.PickupLocation && a.DropLocation == model.DropLocation))
                {
                    ModelState.AddModelError("msg", "This package has already created.");
                }
                var package = Mapper.Map<MonthlyPackageRouteMaster>(model);
                int loggeInUserId = cr.GetUserDetailId();
                package.UpdatedBy_Id = loggeInUserId;
                package.UpdateDate = DateTime.Now;
                package.UpdatedBy_Id = loggeInUserId;
                ent.MonthlyPackageRouteMasters.Add(package);
                ent.SaveChanges();
                ModelState.AddModelError("msg", "Package has created successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("msg", "Server error");
            }
            return View(model);
        }

        public ActionResult All(int page = 1, string term = "", int menuId = 0)
        {
            var model = new MonthlyRouteMasterVm();
            string query = query = @"select mp.*,updated.email as UpdatedBy,vm.ModelName
                                   from MonthlyPackageRouteMaster mp
                                   join Userlogin updated on mp.UpdatedBy_Id=updated.Id
                                   join VehicleModel vm on mp.vehiclemodel_id=vm.Id
                                   order by vm.ModelName";
            var data = ent.Database.SqlQuery<MonthlyPackageRouteMasterDTO>(query).ToList();
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
            model.Packages = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            model.MenuId = menuId;
            return View(model);
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.MonthlyPackageRouteMasters.Find(id);
            var model = Mapper.Map<MonthlyPackageRouteMasterDTO>(data);
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",model.VehicleModel_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MonthlyPackageRouteMasterDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",model.VehicleModel_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var data = Mapper.Map<MonthlyPackageRouteMaster>(model);
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                int loggeInUserId = cr.GetUserDetailId();
                data.UpdatedBy_Id = loggeInUserId;
                data.UpdateDate = DateTime.Now;
                data.UpdatedBy_Id = loggeInUserId;
                ent.SaveChanges();
                ModelState.AddModelError("msg", "Updated successfully.");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("msg", "Server error.");
            }
            return View(model);
        }

        public ActionResult Delete(int id,int menuid=0)
        {
            var package = ent.MonthlyPackageRouteMasters.Find(id);
            try
            {
                ent.MonthlyPackageRouteMasters.Remove(package);
                ent.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("All", new { menuId = menuid });
        }
        

    }
}