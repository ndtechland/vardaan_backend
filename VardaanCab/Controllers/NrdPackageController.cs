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
    public class NrdPackageController : Controller
    {
        // GET: NrgPackage
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult Add()
        {
            var model = new NrgPackageDTO();
            model.RentalTypes = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName");
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(NrgPackageDTO model)
        {
            model.RentalTypes = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName");
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                if(ent.NrgPackages.Any(a=>a.RentalType_Id==model.RentalType_Id && a.VehicleModel_Id==model.VehicleModel_Id))
                {
                    TempData["msg"] = "This package has already added";
                    return View(model);
                }
                var package = Mapper.Map<NrgPackage>(model);
                // get user id
                int userId = commonRepo.GetUserDetailId();
                package.UpdatedBy = userId;
                package.UpdateDate = DateTime.Now;
                ent.NrgPackages.Add(package);
                ent.SaveChanges();
                TempData["msg"] = "Package has added successfully.";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error.";
            }
            return RedirectToAction("Add");
        }

        public ActionResult All(string term = "", int page = 1)
        {
            var model = new NrgPackageVm();
            var data = (from cp in ent.NrgPackages
                        join rt in ent.RentalTypes 
                        on cp.RentalType_Id equals rt.Id
                        join vm in ent.VehicleModels 
                        on cp.VehicleModel_Id equals vm.Id
                        join ud in ent.UserLogins
                        on cp.UpdatedBy equals ud.Id
                        orderby cp.Id descending
                        select new NrgPackageDTO
                        {
                            Id = cp.Id,
                            ModelName=vm.ModelName,
                            RentalType= rt.RentalTypeName,
                            ChauffeurTaDa=cp.ChauffeurTaDa,
                            ExtraPerHour=cp.ExtraPerHour,
                            ExtraPerKm=cp.ExtraPerKm,
                            Fare=cp.Fare,
                            IsActive=cp.IsActive,
                            MinHrs=cp.MinHrs,
                            MinKm=cp.MinKm,
                            NightCharges=cp.NightCharges,
                            RentalType_Id=cp.RentalType_Id,
                            UpdateDate=cp.UpdateDate,
                            UpdatedBy=cp.UpdatedBy,
                            UpdatedByUser=ud.Email,
                            VehicleModel_Id =cp.VehicleModel_Id,
                            OutStationCharge=cp.OutStationCharge
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.ModelName.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 500;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Packages = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var cp = ent.NrgPackages.Find(id);
            var model = Mapper.Map<NrgPackageDTO>(cp);
            model.RentalTypes = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName",cp.RentalType_Id);
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",cp.VehicleModel_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(NrgPackageDTO model)
        {
            model.RentalTypes = new SelectList(ent.RentalTypes.ToList(), "Id", "RentalTypeName", model.RentalType_Id);
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var package = Mapper.Map<NrgPackage>(model);
                // get user id
                int userId = commonRepo.GetUserDetailId();
                package.UpdatedBy = userId;
                package.UpdateDate = DateTime.Now;
                ent.Entry(package).State=System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Package has updated successfully.";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit",new { id = model.Id });
        }

        public ActionResult Delete(int id)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var data = ent.NrgPackages.Find(id);
                    ent.NrgPackages.Remove(data);
                    ent.SaveChanges();
                    if (!commonRepo.CreateLog("NRD Package", "deleted"))
                        throw new Exception("Error in log creation");
                    tran.Commit();
                    return Content("ok");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return Content("err");
                }
            }
        }
    }
}