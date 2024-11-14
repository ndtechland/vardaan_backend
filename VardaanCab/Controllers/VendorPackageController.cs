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
    public class VendorPackageController : Controller
    {
        // GET: VendorPackage
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult Add(int menuId=0)
        {
            var model = new VendorPackageDTO();
         //   model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName + "-" + a.PackageTypeName });
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VendorPackageDTO model)
        {
           // model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName + "-" + a.PackageTypeName });
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType");
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                if(ent.VendorPackages.Any(a=>a.RentalType_Id==model.RentalType_Id && a.VehicleModel_Id==model.VehicleModel_Id))
                {
                    TempData["msg"] = "This package has already added";
                    return View(model);
                }
                var package = Mapper.Map<VendorPackage>(model);
                // get user id
                int userId = commonRepo.GetUserDetailId();
                package.UpdatedBy = userId;
                package.UpdateDate = DateTime.Now;
                ent.VendorPackages.Add(package);
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
            var model = new VendorPackageVm();
            var data = (from cp in ent.VendorPackages
                        join rt in ent.RentalTypes 
                        on cp.RentalType_Id equals rt.Id
                        join vm in ent.VehicleModels 
                        on cp.VehicleModel_Id equals vm.Id
                        join ud in ent.UserLogins
                        on cp.UpdatedBy equals ud.Id
                        join packageType in ent.PackageTypes
                         on rt.PackageType_Id equals packageType.Id into rentalPackage
                        from pt in rentalPackage.DefaultIfEmpty()
                        orderby vm.ModelName
                        select new VendorPackageDTO
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
                            OutStationCharge=cp.OutStationCharge,
                            DropLocation=cp.DropLocation,
                            PickupLocation=cp.PickupLocation,
                            NoOfDays=cp.NoOfDays,
                            PackageTypeName=pt.PackageTypeName
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
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var cp = ent.VendorPackages.Find(id);
            var model = Mapper.Map<VendorPackageDTO>(cp);
            model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName, Selected = a.Id == model.RentalType_Id });
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",cp.VehicleModel_Id);
            var rType = ent.RentalTypes.Find(cp.RentalType_Id);
            var pType = ent.PackageTypes.FirstOrDefault(a => a.Id == rType.PackageType_Id);
            model.PackageType_Id = pType.Id;
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType",pType.Id);
            model.MenuId = menuId;
            model.PackageTypeName = pType.PackageTypeName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorPackageDTO model)
        {
            var rType = ent.RentalTypes.Find(model.RentalType_Id);
            var pType = ent.PackageTypes.FirstOrDefault(a => a.Id == rType.PackageType_Id);
            model.RentalTypes = commonRepo.GetRentalTypeList().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.RentalTypeName, Selected = a.Id == model.RentalType_Id });
            model.PackageTypes = new SelectList(ent.PackageTypes.ToList(), "Id", "PType", pType.Id);
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (ent.VendorPackages.Any(a => a.Id!=model.Id && a.RentalType_Id == model.RentalType_Id && a.VehicleModel_Id == model.VehicleModel_Id))
                {
                    TempData["msg"] = "This package has already added";
                    return View(model);
                }
                var package = Mapper.Map<VendorPackage>(model);
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
            return RedirectToAction("Edit",new { id = model.Id,menuId=model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var data = ent.VendorPackages.Find(id);
                    ent.VendorPackages.Remove(data);
                    ent.SaveChanges();
                    if (!commonRepo.CreateLog("Vendor Package", "deleted"))
                        throw new Exception("Error in log creation");
                    tran.Commit();
                    return Content("ok");
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    return Content("err");
                }
            }
        }

    }
}