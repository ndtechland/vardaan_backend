using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Domain.DTO;
using AutoMapper;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;
using VardaanCab.Repository;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        // GET: Vendor
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult Add(int menuId=0)
        {
            var model = new VendorDTO();
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.Vendors = new SelectList(ent.Vendors.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            model.Packages = commonRepo.GetAllVendorPackages().ToList();
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VendorDTO model)
        {
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.Vendors = new SelectList(ent.Vendors.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            model.Packages = commonRepo.GetAllVendorPackages().ToList();
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if(ent.Vendors.Any(a=>a.CompanyName.ToLower().Equals(model.CompanyName.ToLower())))
                    {
                        TempData["msg"] = "Company name already exist in our database";
                        return View(model);
                    }
                    if (model.PanFile != null)
                    {
                        if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                        {
                            TempData["msg"] = "Pan file image should not exceed 1 mb";
                            return View(model);
                        }
                        var result = FileOperation.UploadImage(model.PanFile, "Images");
                        if (result == "not allowed")
                        {
                            TempData["msg"] = "Only .jpg, .png, .jpeg files are allowed as PAN Document";
                            return View(model);
                        }
                        model.PanImage = result;
                    }
                    var vendor = Mapper.Map<Vendor>(model);
                    vendor.CreateDate = DateTime.Now;
                    vendor.IsActive = true;
                    ent.Vendors.Add(vendor);
                    ent.SaveChanges();
                    if (model.Packages.Count > 0)
                    {
                        foreach (var item in model.Packages)
                        {
                            var vp = new VendorPersonalPackage
                            {
                                Vendor_Id = vendor.Id,
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
                                VendorPackage_Id = item.VendorPackage_Id,
                                PickupLocation = item.PickupLocation,
                                DropLocation = item.DropLocation,
                                NoOfDays = item.NoOfDays
                            };
                            ent.VendorPersonalPackages.Add(vp);
                        }
                        ent.SaveChanges();
                    }
                    tran.Commit();
                    TempData["msg"] = "Vendor has registered successfully";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Server error";
                }
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult All(string term = "", int page = 1, bool export = false, int menuId = 0)
        {
            var model = new VendorListVm();
            var data = (from v in ent.Vendors
                        join s in ent.StateMasters on v.StateMaster_Id equals s.Id into v_s
                        from vendorState in v_s.DefaultIfEmpty()
                        join c in ent.CityMasters on v.CityMaster_Id equals c.Id into v_c
                        from vendorCity in v_c.DefaultIfEmpty()
                        join pVendor in ent.Vendors on v.ParentVendor_Id equals pVendor.Id into v_pVendor
                        from pv in v_pVendor.DefaultIfEmpty()
                        orderby v.Id descending
                        select new VendorDTO
                        {
                            Id = v.Id,
                            AlernateMobile = v.AlernateMobile,
                            CityName = vendorCity.CityName,
                            FullAddress = v.FullAddress,
                            StateName = vendorState.StateName,
                            Email = v.Email,
                            MobileNumber = v.MobileNumber,
                            ParentVendorName = pv == null ? "none" : pv.VendorName,
                            CityMaster_Id = v.CityMaster_Id,
                            StateMaster_Id = v.StateMaster_Id,
                            ParentVendor_Id = v.ParentVendor_Id,
                            CreateDate = v.CreateDate,
                            IsActive = v.IsActive,
                            CompanyName = v.CompanyName,
                            VendorName = v.VendorName,
                            GSTIN = v.GSTIN,
                            PAN = v.PAN,
                            CIN = v.CIN,
                            PanImage = v.PanImage
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.VendorName.ToLower().Contains(term) || (!string.IsNullOrEmpty(a.MobileNumber) && a.MobileNumber.Contains(term)) || a.CompanyName.ToLower().Contains(term)).ToList();
                page = 1;
            }

            if (export)
            {

                var ed = (from d in data
                          select new VendorExcelModel
                          {
                              RegistrationDate = d.CreateDate.Date,
                              AlernateMobile = d.AlernateMobile,
                              CityName = d.CityName,
                              FullAddress = d.FullAddress,
                              StateName = d.StateName,
                              Email = d.Email,
                              MobileNumber = d.MobileNumber,
                              ParentVendorName = d.ParentVendorName == null ? "none" : d.ParentVendorName,
                              CompanyName = d.CompanyName,
                              VendorName = d.VendorName,
                              GSTIN = d.GSTIN,
                              PAN = d.PAN,
                              CIN = d.CIN,
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =vendorList.xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            // pagination
            int total = data.Count;
            int pageSize = 1000;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Vendors = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var data = ent.Vendors.Find(id);
            var model = Mapper.Map<VendorDTO>(data);
            model.States = new SelectList(ent.StateMasters.AsNoTracking().ToList(), "Id", "StateName", data.StateMaster_Id);
            model.Vendors = new SelectList(ent.Vendors.AsNoTracking().Where(a => a.IsActive).ToList(), "Id", "CompanyName", data.ParentVendor_Id);
            model.Cities = new SelectList(ent.CityMasters.AsNoTracking().ToList(), "Id", "CityName", data.CityMaster_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorDTO model)
        {
            ModelState.Remove("PanFile");
            model.States = new SelectList(ent.StateMasters.AsNoTracking().ToList(), "Id", "StateName", model.StateMaster_Id);
            model.Vendors = new SelectList(ent.Vendors.AsNoTracking().Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.ParentVendor_Id);
            model.Cities = new SelectList(ent.CityMasters.AsNoTracking().ToList(), "Id", "CityName", model.CityMaster_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (ent.Vendors.Any(a =>a.Id!=model.Id&& a.CompanyName.ToLower().Equals(model.CompanyName.ToLower())))
                {
                    TempData["msg"] = "Company name already exist in our database";
                    return View(model);
                }
                if (model.PanFile != null)
                {
                    if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Pan file image should not exceed 1 mb";
                        return View(model);
                    }
                    var result = FileOperation.UploadImage(model.PanFile, "Images");
                    if (result == "not allowed")
                    {
                        TempData["msg"] = "Only .jpg, .png, .jpeg files are allowed as PAN Document";
                        return View(model);
                    }
                    model.PanImage = result;
                }
                var vendor = Mapper.Map<Vendor>(model);
                ent.Entry(vendor).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Vendor has updated successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id,menuId=model.MenuId });
        }

        public ActionResult Delete(int id,int menuId=0)
        {
            try
            {
                var data = ent.Vendors.Find(id);
                ent.Vendors.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

        public ActionResult ChangeStatus(int id, int menuId = 0)
        {
            string query = @"update vendor set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("All",new { menuId=menuId});
        }

    }
}