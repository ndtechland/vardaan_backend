using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;
namespace VardaanCab.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();
        private readonly CommonOperations _random = new CommonOperations();

        public ActionResult ChangeStatus(int id,int menuId=0)
        {
            string query = @"update Customer set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
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



        private IEnumerable<CustomerDTO> GetCustomerList(string term = "")
        {
            var data = (from c in ent.Customers
                        join p in ent.Customers
                        on c.ParentCustomer_Id equals p.Id into pc
                        from p1 in pc.DefaultIfEmpty()
                        join sm in ent.StateMasters on c.State_Id equals sm.Id
                        join ci in ent.CityMasters on c.City_Id equals ci.Id
                        orderby c.Id descending
                        select new CustomerDTO
                        {
                            Id = c.Id,
                            ContactNo = c.ContactNo,
                            CreateDate = c.CreateDate,
                            AlternateNo = c.AlternateNo,
                            CustomerName = c.CustomerName,
                            ParentCustomer = p1.CompanyName,
                            Email = c.Email,
                            FullAddress = c.FullAddress,
                            IsActive = c.IsActive,
                            ParentCustomer_Id = c.ParentCustomer_Id,
                            ProfilePic = c.ProfilePic,
                            CompanyName = c.CompanyName,
                            GSTIN = c.GSTIN,
                            StateName = sm.StateName,
                            StateCode = sm.StateCode,
                            City_Name=ci.CityName
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.CompanyName.ToLower().Contains(term) || a.ContactNo.Contains(term) || a.StateName.StartsWith(term)).ToList();
            }
            return data;
        }

        public ActionResult All(string term = "", int page = 1, bool export = false, int menuId = 0)
        {
            var model = new CustomerListVm();
            var data = GetCustomerList(term).ToList();
            if (export)
            {
                var ed = (from d in data
                          select new CustomerExcelModel
                          {
                              CustomerName = d.CompanyName,
                              ContactPerson = d.CustomerName,
                              ParentCustomer = d.ParentCustomer,
                              ContactNo = d.ContactNo,
                              AlternateNo = d.AlternateNo,
                              Email = d.Email,
                              FullAddress = d.FullAddress,
                              StateName = d.StateName,
                              StateCode = d.StateCode,
                              GSTIN = d.GSTIN,
                              City_Name=d.City_Name
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =clientList.xls");
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
            model.Customers = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new CustomerDTO();
            model.CustomerList = new SelectList(ent.Customers.ToList(), "Id", "CompanyName");
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.Packages = ent.Database.SqlQuery<ClientPackageDTO>(@"select cp.*,cp.Id as CorporatePackage_Id,rt.RentalTypeName,pt.PType as PackageTypeName,vm.ModelName from corporatepackage cp
join RentalType rt on cp.RentalType_Id=rt.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
join VehicleModel vm on cp.VehicleModel_Id= vm.Id order by vm.ModelName").ToList();
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CustomerDTO model)
        {
            string RandomPassword = _random.GenerateRandomPassword();
            model.CustomerList = new SelectList(ent.Customers.ToList(), "Id", "CompanyName");
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            //model.Packages = ent.Database.SqlQuery<ClientPackageDTO>(@"select cp.*,cp.Id as CorporatePackage_Id,rt.RentalTypeName,vm.ModelName from corporatepackage cp
            //join RentalType rt on cp.RentalType_Id=rt.Id
            //join VehicleModel vm on cp.VehicleModel_Id= vm.Id order by vm.ModelName").ToList();
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {

                    if (!ModelState.IsValid)
                        return View(model);

                    if(ent.Customers.Any(a=>a.CompanyName.ToLower().Equals(model.CompanyName.ToLower()) && a.City_Id==model.City_Id))
                    {
                      TempData["msg"] = "Company name already exist in our database";
                        return View(model);
                    }
                    var data = Mapper.Map<Customer>(model);
                    data.CreateDate = DateTime.Now;
                    data.IsActive = true;
                    data.GeoLocation = model.GeoLocation;
                    ent.Customers.Add(data);
                    ent.SaveChanges();
                    //data saved in userlogin
                    var userlogin = new UserLogin()
                    {
                        Email=data.CompanyName,
                        MobileNumber=data.ContactNo,
                        Password= RandomPassword,
                        Role="Customer",
                        IsActive=true
                    };
                    ent.UserLogins.Add(userlogin);
                    ent.SaveChanges();

                    // add client package
                    if (model.ParentCustomer_Id == null)
                    {
                        if (model.Packages.Count > 0)
                        {
                            foreach (var item in model.Packages)
                            {
                                if (item.IsChecked)
                                {
                                    var cp = new ClientPackage
                                    {
                                        Customer_Id = data.Id,
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
                    }
                    //else
                    //{
                    //    var parent = ent.Customers.Find(model.ParentCustomer_Id);
                    //    var parentPackages = ent.ClientPackages.Where(a => a.Customer_Id == parent.Id).ToList();
                    //    foreach (var pp in parentPackages)
                    //    {
                    //        pp.Customer_Id = data.Id;
                    //    }
                    //    ent.ClientPackages.AddRange(parentPackages);
                    //    ent.SaveChanges();
                    //}
                    TempData["msg"] = "Record has saved.";
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }
            return RedirectToAction("Add",new { menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.Customers.Find(id);
            var model = Mapper.Map<CustomerDTO>(data);
            model.CustomerList = new SelectList(ent.Customers.ToList(), "Id", "CompanyName", data.ParentCustomer_Id);
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", data.State_Id);
            //by bhupesh
            model.CityList = new SelectList(ent.CityMasters.ToList(), "Id", "CityName", data.City_Id);

            //
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CustomerDTO model)
        {
            try
            {
                model.CustomerList = new SelectList(ent.Customers.AsNoTracking().ToList(), "Id", "CompanyName", model.ParentCustomer_Id);
                model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName", model.State_Id);
                //by bhupesh
                model.CityList = new SelectList(ent.CityMasters.ToList(), "Id", "CityName", model.City_Id);

                if (!ModelState.IsValid)
                    return View(model);
                if (ent.Customers.Any(a =>a.Id!=model.Id&& a.CompanyName.ToLower().Equals(model.CompanyName.ToLower()) && a.City_Id == model.City_Id))
                {
                    TempData["msg"] = "Company name already exist in our database";
                    return View(model);
                }
                var state = Mapper.Map<Customer>(model);
                state.GeoLocation = model.GeoLocation;
                ent.Entry(state).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });

        }

        public ActionResult Delete(int id,int menuId=0)
        {
            try
            {
                var data = ent.Customers.Find(id);
                ent.Customers.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

        public ActionResult CreateOrg(int menuId = 0, int id = 0)
        {
            var model = new createorg();
            model.Companies = new SelectList(ent.Customers.Where(c=>c.IsActive==true).ToList(), "Id", "CustomerName");
            model.OrgNameList = ent.Customers.Where(c=>c.OrgName!=null).ToList();
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.Customers.Where(x => x.Id == id).FirstOrDefault();
                model.CompanyId = data.Id;
                model.OrgName = data.OrgName;
                ViewBag.Heading = "Update Org";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.OrgName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Org";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult CreateOrg(createorg model)
        {
            try
            {
                var customerinfo = ent.Customers.Where(c => c.IsActive && c.Id == model.CompanyId).FirstOrDefault();
                 
                if (customerinfo != null)
                {
                    customerinfo.OrgName=model.OrgName;
                    ent.SaveChanges();
                    TempData["msg"] = "Org name created successfully.";
                    return RedirectToAction("CreateOrg" ,new { MenuId = model.MenuId});
                }
                else
                {
                    TempData["msg"] = "Company not found.";
                    return RedirectToAction("CreateOrg", new { MenuId = model.MenuId });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}