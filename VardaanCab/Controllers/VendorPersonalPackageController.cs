using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Domain.DTO;
using AutoMapper;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class VendorPersonalPackageController : Controller
    {
        // GET: VendorPersonalPackage
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult ViewPackage(int id, string term = "", int page = 1,int menuId=0)
        {
            var model = new VendorPersonalPackageVm();
            model.Vendor_Id = id;
            var data = ent.Database.SqlQuery<VendorPersonalPackageDTO>(@"select cp.*,rt.RentalTypeName,pt.PType as PackageTypeName,vm.ModelName,ud.Email as UpdatedByUser from VendorPersonalPackage cp
join RentalType rt on cp.RentalType_Id=rt.Id
join VehicleModel vm on cp.VehicleModel_Id= vm.Id
join  UserLogin ud on cp.UpdatedBy = ud.Id 
left join PackageType pt on rt.PackageType_Id=pt.Id
where cp.Vendor_Id='" + id + "' order by vm.ModelName").ToList();
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
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int VendorId,int menuId=0)
        {
            var model = new VendorPersonalPackageVm();
            model.Vendor_Id = VendorId;
            //            string query = @"select * from (select ROW_NUMBER() over (partition by vp.Id order by vp.Id) as RowNo, vp.*,vp.Id as VendorPackage_Id,rt.RentalTypeName,vm.ModelName from VendorPackage vp
            //join RentalType rt on vp.RentalType_Id=rt.Id
            //join VehicleModel vm on vp.VehicleModel_Id= vm.Id
            //join cab c on vp.vehiclemodel_Id=c.vehiclemodel_Id
            //join vendor v on c.Vendor_Id=v.Id
            //where vp.Id not in (select VendorPackage_Id from VendorPersonalPackage where Vendor_Id="+model.Vendor_Id+ ")and v.Id=" + model.Vendor_Id + " and c.IsActive=1) tbl where RowNo=1;";

            model.Packages = commonRepo.GetUnassignedVendorPackages(VendorId).ToList();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VendorPersonalPackageVm model)
        {
//            string query = @"select * from (select ROW_NUMBER() over (partition by vp.Id order by vp.Id) as RowNo, vp.*,vp.Id as VendorPackage_Id,rt.RentalTypeName,vm.ModelName from VendorPackage vp
//join RentalType rt on vp.RentalType_Id=rt.Id
//join VehicleModel vm on vp.VehicleModel_Id= vm.Id
//join cab c on vp.vehiclemodel_Id=c.vehiclemodel_Id
//join vendor v on c.Vendor_Id=v.Id
//where c.VehicleModel_Id in (select distinct vehiclemodel_Id from VendorPackage) and vp.Id not in (select VendorPackage_Id from VendorPersonalPackage where Vendor_Id=" + model.Vendor_Id + ")and v.Id=" + model.Vendor_Id + ") tbl where RowNo=1;";
//            model.Packages = ent.Database.SqlQuery<VendorPersonalPackageDTO>(query).ToList();

            try
            {
                int loggedInId = commonRepo.GetUserDetailId();
                // add client package
                if (model.Packages.Count > 0)
                {
                    foreach (var item in model.Packages)
                    {
                        var vp = new VendorPersonalPackage
                        {
                            Vendor_Id = model.Vendor_Id,
                            RentalType_Id = item.RentalType_Id,
                            VehicleModel_Id = item.VehicleModel_Id,
                            Fare = item.Fare,
                            MinHrs = item.MinHrs,
                            MinKm = item.MinKm,
                            ExtraPerHour = item.ExtraPerHour,
                            ExtraPerKm = item.ExtraPerKm,
                            NightCharges = item.NightCharges,
                            ChauffeurTaDa = item.ChauffeurTaDa,
                            InterStateCharge = item.InterStateCharge,
                            OutStationCharge = item.OutStationCharge,
                            IsActive = true,
                            UpdateDate = DateTime.Now,
                            UpdatedBy = loggedInId,
                            VendorPackage_Id = item.VendorPackage_Id,
                            PickupLocation=item.PickupLocation,
                            DropLocation=item.DropLocation,
                            NoOfDays=item.NoOfDays
                        };
                        ent.VendorPersonalPackages.Add(vp);
                    }
                    ent.SaveChanges();
                }

                TempData["msg"] = "Record has saved.";
                return RedirectToAction("ViewPackage", new { id = model.Vendor_Id,menuId=model.MenuId });

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return View(model);
            }
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            string query = @"select vp.*,pt.PType as PackageTypeName,rt.RentalTypeName,vm.ModelName from VendorPersonalPackage vp
join RentalType rt on vp.RentalType_Id = rt.Id
join VehicleModel vm on vp.VehicleModel_Id = vm.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
where vp.Id = '" + id + "'";
            var model = ent.Database.SqlQuery<VendorPersonalPackageDTO>(query).FirstOrDefault();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorPersonalPackageDTO model)
        {
            try
            {
                var package = Mapper.Map<VendorPersonalPackage>(model);
                // get user id
                int userId = commonRepo.GetUserDetailId();
                package.UpdatedBy = userId;
                package.UpdateDate = DateTime.Now;
                ent.Entry(package).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Package has updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id,menuId = model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var data = ent.VendorPersonalPackages.Find(id);
                    ent.VendorPersonalPackages.Remove(data);
                    ent.SaveChanges();
                    if (!commonRepo.CreateLog("Vendor Package", "deleted"))
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