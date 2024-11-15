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
    [Authorize]
    public class ClientPackageController : Controller
    {
        // GET: ClientPackage
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult ViewPackage(int id, string term = "", int page = 1,int menuId=0)
        {
            var model = new ClientPackageVm();
            model.Customer_Id = id;
            var data = ent.Database.SqlQuery<ClientPackageDTO>(@"select cp.*,rt.RentalTypeName,pt.PType as PackageTypeName,vm.ModelName,ud.Email as UpdatedByUser from clientpackage cp
join RentalType rt on cp.RentalType_Id=rt.Id
join VehicleModel vm on cp.VehicleModel_Id= vm.Id
join  UserLogin ud on cp.UpdatedBy = ud.Id 
left join PackageType pt on rt.PackageType_Id=pt.Id
where cp.Customer_Id='" + id + "' order by vm.ModelName").ToList();
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

        public ActionResult Add(int ClientId, int page = 1,int menuId=0)
        {
            var model = new ClientPackageVm();
            model.Customer_Id = ClientId;
            string query = @"select cp.*,cp.Id as CorporatePackage_Id,rt.RentalTypeName,pt.PType as PackageTypeName,vm.ModelName from corporatepackage cp
join RentalType rt on cp.RentalType_Id=rt.Id
join VehicleModel vm on cp.VehicleModel_Id= vm.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
where cp.Id not in (select CorporatePackage_Id from ClientPackage where Customer_Id=" + ClientId + ") order by vm.ModelName";
            var data=ent.Database.SqlQuery<ClientPackageDTO>(query).ToList(); 
            int total = data.Count;
            int pageSize = 50;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();

            model.Packages = data;
            model.Page= page;
            model.NumberOfPages = (int)totalPages;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(ClientPackageVm model)
        {
//            string query = @"select cp.*,cp.Id as CorporatePackage_Id,rt.RentalTypeName,vm.ModelName from corporatepackage cp
//join RentalType rt on cp.RentalType_Id=rt.Id
//join VehicleModel vm on cp.VehicleModel_Id= vm.Id
//where cp.Id not in (select CorporatePackage_Id from ClientPackage where Customer_Id=" + model.Customer_Id + ")";
//            model.Packages = ent.Database.SqlQuery<ClientPackageDTO>(query).ToList();

            try
            {
                // add client package
                if (model.Packages.Count > 0)
                {
                    foreach (var item in model.Packages)
                    {
                        if (item.IsChecked)
                        {
                            var cp = new ClientPackage
                            {
                                Customer_Id = model.Customer_Id,
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
                                UpdatedBy = commonRepo.GetUserDetailId(),
                                CorporatePackage_Id = item.CorporatePackage_Id,
                                PickupLocation=item.PickupLocation,
                                DropLocation=item.DropLocation,
                                NoOfDays=item.NoOfDays
                            };
                            ent.ClientPackages.Add(cp);
                        }
                    }
                   ent.SaveChanges();
                }

                TempData["msg"] = "Record has saved.";
                return RedirectToAction("ViewPackage", new {id=model.Customer_Id ,menuId=model.MenuId});

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
                return View(model);
            }
        }

        public ActionResult Edit(int id ,int menuId=0)
        {
            string query = @"select cp.*,rt.RentalTypeName,pt.PType as PackageTypeName,vm.ModelName from clientpackage cp
join RentalType rt on cp.RentalType_Id = rt.Id
join VehicleModel vm on cp.VehicleModel_Id = vm.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
where cp.Id = '" + id + "'";
            var model  = ent.Database.SqlQuery<ClientPackageDTO>(query).FirstOrDefault();
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ClientPackageDTO model)
        {
            
            try
            {
                var package = Mapper.Map<ClientPackage>(model);
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
            return RedirectToAction("Edit", new { id = model.Id,menuId=model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var data = ent.ClientPackages.Find(id);
                    ent.ClientPackages.Remove(data);
                    ent.SaveChanges();
                    if (!commonRepo.CreateLog("Client Package", "deleted"))
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