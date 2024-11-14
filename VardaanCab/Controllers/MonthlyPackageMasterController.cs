using AutoMapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MonthlyPackageMasterController : Controller
    {
        // GET: MonthlyPackage
        DbEntities ent = new DbEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();

        public ActionResult CreatePackage(int menuId=0)
        {
            var model = new MonthlyPackageMasterDTO();
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePackage(MonthlyPackageMasterDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if(ent.MonthlyPackageMasters.Any(a=>a.VehicleModel_Id==model.VehicleModel_Id))
                {
                    ModelState.AddModelError("msg", "This Package is already added");
                    return View(model);
                }
                var package = Mapper.Map<MonthlyPackageMaster>(model);
                int loggeInUserId = cr.GetUserDetailId();
                package.UpdatedBy_Id = loggeInUserId;
                package.CreatedBy_Id = loggeInUserId;
                package.UpdateDate = DateTime.Now;
                ent.MonthlyPackageMasters.Add(package);
                ent.SaveChanges();
                ModelState.AddModelError("msg", "Package has created successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("msg", "Server error");
            }
            return View(model);
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var mp = ent.MonthlyPackageMasters.Find(id);
            var model = Mapper.Map<MonthlyPackageMasterDTO>(mp);
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MonthlyPackageMasterDTO model)
        {
            model.VehicleModelList = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var package = Mapper.Map<MonthlyPackageMaster>(model);
                int loggeInUserId = cr.GetUserDetailId();
                package.UpdatedBy_Id = loggeInUserId;
                package.UpdateDate = DateTime.Now;
                ent.Entry(package).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                ModelState.AddModelError("msg", "Package has updated successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("msg", "Server error");
            }
            return View(model);
        }

        public ActionResult All(DateTime? sDate, DateTime? eDate, int page = 1, string term = "",int menuId=0)
        {
            var model = new MonthlyPackageMasterVm();
            string query = @"select mp.*,created.Email as CreatedBy,
                           updated.email as UpdatedBy,
                           vm.ModelName,IsNull(dbo.getLastBillCancelStatus(mp.Id),0) as IsCancelled
                           from MonthlyPackageMaster mp
                           join Userlogin created on mp.CreatedBy_Id=created.Id
                           join Userlogin updated on mp.UpdatedBy_Id=updated.Id
                           join VehicleModel vm on mp.vehiclemodel_id=vm.Id
                           order by mp.Id desc";
            var data = ent.Database.SqlQuery<MonthlyPackageMasterDTO>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.ModelName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 150;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Packages = data;
             model.sDate = sDate;
            model.eDate = eDate;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            model.MenuId = menuId;
            return View(model);
        }

        public ActionResult Delete(int id,int menuId=0)
        {
           try
            {
                var data = ent.MonthlyPackageMasters.Find(id);
                ent.MonthlyPackageMasters.Remove(data);
                ent.SaveChanges();
            }
            catch(Exception ex)
            {
                ///
            }
            return RedirectToAction("All",new { menuId = menuId });
        }

        

    }
}