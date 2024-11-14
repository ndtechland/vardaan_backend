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
    public class CorporatePackageController : Controller
    {
        // GET: CorporatePackage
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult Add(int menuId = 0)
        {
            var model = new CorporatePackageDTO();
         //   model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName + "-" + a.PackageTypeName });
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType");
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CorporatePackageDTO model)
        {
         //   model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName + "-" + a.PackageTypeName });
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType");
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                if(ent.CorporatePackages.Any(a=>a.RentalType_Id==model.RentalType_Id && a.VehicleModel_Id==model.VehicleModel_Id))
                {
                    TempData["msg"] = "This package has already added";
                    return View(model);
                }
                var package = Mapper.Map<CorporatePackage>(model);
                // get user id
                int userId = commonRepo.GetUserDetailId();
                package.UpdatedBy = userId;
                package.UpdateDate = DateTime.Now;
                ent.CorporatePackages.Add(package);
                ent.SaveChanges();
                TempData["msg"] = "Package has added successfully.";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error.";
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult All(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CorporatePackageVm();
            //var data = (from cp in ent.CorporatePackages
            //            join rt in ent.RentalTypes 
            //            on cp.RentalType_Id equals rt.Id
            //            join vm in ent.VehicleModels 
            //            on cp.VehicleModel_Id equals vm.Id
            //            join ud in ent.UserLogins
            //            on cp.UpdatedBy equals ud.Id
            //            join packageType in ent.PackageTypes
            //            on rt.PackageType_Id equals packageType.Id into rentalPackage
            //            from pt in rentalPackage.DefaultIfEmpty()
            //            orderby vm.ModelName 
            //            select new CorporatePackageDTO
            //            {
            //                Id = cp.Id,
            //                ModelName=vm.ModelName,
            //                RentalType= rt.RentalTypeName,
            //                ChauffeurTaDa=cp.ChauffeurTaDa,
            //                ExtraPerHour=cp.ExtraPerHour,
            //                ExtraPerKm=cp.ExtraPerKm,
            //                Fare=cp.Fare,
            //                IsActive=cp.IsActive,
            //                MinHrs=cp.MinHrs,
            //                MinKm=cp.MinKm,
            //                NightCharges=cp.NightCharges,
            //                RentalType_Id=cp.RentalType_Id,
            //                UpdateDate=cp.UpdateDate,
            //                UpdatedBy=cp.UpdatedBy,
            //                UpdatedByUser=ud.Email,
            //                VehicleModel_Id =cp.VehicleModel_Id,
            //                OutStationCharge=cp.OutStationCharge,
            //                DropLocation=cp.DropLocation,
            //                PickupLocation=cp.PickupLocation,
            //                NoOfDays=cp.NoOfDays,
            //                PackageTypeName=pt.PackageTypeName
            //            }
            //            ).ToList();
            string q = @"select cp.*,vm.ModelName,ul.Email as UpdatedByUser,rt.RentalTypeName as RentalType,pt.PType as PackageTypeName
from corporatepackage cp
left join RentalType rt on cp.RentalType_Id=rt.Id
left join VehicleModel vm on cp.VehicleModel_Id=vm.Id
left join UserLogin ul on cp.UpdatedBy = ul.Id
left join PackageType pt on rt.PackageType_Id=pt.Id order by vm.ModelName ";
            var data = ent.Database.SqlQuery<CorporatePackageDTO>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => (!string.IsNullOrEmpty(a.ModelName)&& a.ModelName.ToLower().Contains(term))||a.RentalType.ToLower().Contains(term)).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 1000;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Packages = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var cp = ent.CorporatePackages.Find(id);
            var rType = ent.RentalTypes.Find(cp.RentalType_Id);
            var pType = ent.PackageTypes.FirstOrDefault(a => a.Id == rType.PackageType_Id);
            var model = Mapper.Map<CorporatePackageDTO>(cp);
            model.PackageType_Id = pType.Id;
            model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName,Selected=a.Id==model.RentalType_Id });
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",cp.VehicleModel_Id);
          
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType",pType.Id);
            model.MenuId = menuId;
            model.PackageTypeName = pType.PType;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CorporatePackageDTO model)
        {
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            var rType = ent.RentalTypes.Find(model.RentalType_Id);
            var pType = ent.PackageTypes.FirstOrDefault(a => a.Id == rType.PackageType_Id);
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", pType.Id);
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (ent.CorporatePackages.Any(a => a.Id!=model.Id && a.RentalType_Id == model.RentalType_Id && a.VehicleModel_Id == model.VehicleModel_Id))
                {
                   TempData["msg"] = "This package has already added";
                   return View(model);
                }
                var package = Mapper.Map<CorporatePackage>(model);
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
            return RedirectToAction("Edit",new { id = model.Id, menuId=model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var data = ent.CorporatePackages.Find(id);
                    ent.CorporatePackages.Remove(data);
                    ent.SaveChanges();
                    if (!commonRepo.CreateLog("Corporate Package", "deleted"))
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

        public JsonResult GetRentalType(int pTypeId)
        {
            var data = ent.RentalTypes.Where(a => a.PackageType_Id == pTypeId).ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

    }
}