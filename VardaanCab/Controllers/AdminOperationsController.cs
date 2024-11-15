using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class AdminOperationsController : Controller
    {
        // GET: AdminOperations
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public ActionResult All(string term = "", int page = 1,int menuId=0)
        {
            var model = new UserLoginViewModel();
            var data = ent.UserLogins.Where(a=>a.Role.Equals("semiadmin")).OrderByDescending(a => a.Id).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.Email.ToLower().Contains(term) ||a.MobileNumber==term).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 200;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.UsersLogin = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId=0)
        {
            var model = new RoleManageModel();
            var query = @"select * from SoftwareLink where  IsHeading=1";
            var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO2>(query).ToList();
            foreach (var item in softwareLinks)
            {
                var l = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + item.Id).ToList();
                item.ChildMenus = l;
                foreach (var sub in l)
                {
                    var h = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + sub.Id).ToList();
                    sub.ChildMenus = h;
                }

            }
            model.Rights = softwareLinks;
            ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(RoleManageModel model)
        {
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {
                    var query = @"select * from SoftwareLink where  IsHeading=1";
                    var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO2>(query).ToList();
                    foreach (var item in softwareLinks)
                    {
                        var l = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + item.Id).ToList();
                        item.ChildMenus = l;
                        foreach (var sub in l)
                        {
                            var h = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + sub.Id).ToList();
                            sub.ChildMenus = h;
                        }

                    }
                    model.Rights = softwareLinks;
                    //
                    if (!ModelState.IsValid)
                        return View(model);
                    if(ent.UserLogins.Any(a=>a.Email==model.Email))
                    {
                        TempData["msg"] = "User Name already exists";
                        return View(model);
                    }
                    if (ent.UserLogins.Any(a => a.MobileNumber == model.MobileNumber))
                    {
                        TempData["msg"] = "Mobile number already exists";
                        return View(model);
                    }
                    ///////////create user login details/////////////
                    model.Role = "semiadmin";
                    model.Password = Guid.NewGuid().GetHashCode().ToString("x");
                    var data = Mapper.Map<UserLogin>(model);
                    ent.UserLogins.Add(data);
                    ent.SaveChanges();

                    //////// add assign rights to user ////////////
                    if (model.IsChecked != null && model.IsChecked.Count() > 0)
                    {
                        foreach (var item in model.IsChecked)
                        {

                            var usl = new User_SoftwareLink
                            {
                                UserId = data.Id,
                                SoftwareLink_Id = item
                            };
                            ent.User_SoftwareLink.Add(usl);

                        }
                        ent.SaveChanges();
                    }                  
                    trans.Commit();
                    TempData["msg"] = "Record has saved.";

                    ///////send user credential to mobile
                    string msg = "Hi " + data.Email + ",\n Your Username Is: " + data.Email + "\n Password :" + data.Password + "";
                    SmsOperation.SendSms(data.MobileNumber, msg);
                    EmailOperation.SendEmail("bhupal@vardaanrentacar.com","New User Created on CRM",msg,true);


                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }
            
            return RedirectToAction("Add",new { menuId = model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.UserLogins.Find(id);
            var model = Mapper.Map<UserLoginDTO>(data);
            ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserLoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<UserLogin>(model);
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has updated.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });

        }

        public ActionResult UserRights(int userId, int menuId = 0)
        {
            var model = new RoleManageModel();
            
            var query = @"select * from SoftwareLink";
            var softwareLinks = ent.Database.SqlQuery<SoftwareLinkDTO2>(query).ToList();
            //foreach (var item in softwareLinks)
            //{
            //    var l = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + item.Id).ToList();
            //    item.ChildMenus = l;

            //    foreach (var sub in l)
            //    {
            //        var h = ent.Database.SqlQuery<SoftwareLinkDTO2>(@"select * from SoftwareLink where  Parent_Id=" + sub.Id).ToList();
            //        sub.ChildMenus = h;
            //    }
            //}
            var userLinks = ent.User_SoftwareLink.Where(a => a.UserId == userId).ToList();
            var data = (from sl in softwareLinks
                        join ul in userLinks on sl.Id equals ul.SoftwareLink_Id into slUl
                        from d in slUl.DefaultIfEmpty()
                        select new
                        SoftwareLinkDTO2
                        {
                            IsChecked = d == null ? false:true,
                            Id=sl.Id,
                            Title=sl.Title,
                            IsHeading=sl.IsHeading,
                            IsMenu=sl.IsMenu,
                            Parent_Id=sl.Parent_Id,
                            Url=sl.Url,
                            ChildMenus=sl.ChildMenus
                        }).ToList();
            var heading = data.Where(a => a.IsHeading).ToList();
            foreach (var item in heading)
            {
                var l = data.Where(a => a.Parent_Id == item.Id).ToList();
                item.ChildMenus = l;

                foreach (var sub in l)
                {
                    var h = data.Where(a => a.Parent_Id == sub.Id).ToList();
                    sub.ChildMenus = h;
                }
            }
            model.Rights = heading;
            model.UserId = userId;
            ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult UserRights(RoleManageModel model)
        {
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {
                    //delete exixting software link for new link add
                    ent.Database.ExecuteSqlCommand("delete from User_SoftwareLink where USERID=" + model.UserId);

                    //////// add assign rights to user ////////////
                    if (model.IsChecked != null && model.IsChecked.Count() > 0)
                    {
                        foreach (var item in model.IsChecked)
                        {
                            var usl = new User_SoftwareLink
                            {
                                UserId = model.UserId,
                                SoftwareLink_Id = item
                            };
                            ent.User_SoftwareLink.Add(usl);
                        }
                        ent.SaveChanges();
                    }
                    trans.Commit();
                    TempData["msg"] = "Record has been updated.";
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }

            return RedirectToAction("UserRights",new { userId=model.UserId, menuId= model.MenuId });
        }

        public ActionResult Delete(int id,int menuId=0)
         {
            try
            {
                //delete user details
                var data = ent.UserLogins.Find(id);
                ent.UserLogins.Remove(data);
                ent.SaveChanges();
                //delete user software link 
                ent.Database.ExecuteSqlCommand("delete from User_SoftwareLink where USERID=" + id);

                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }
        
        public ActionResult Deactivate(int id, int menuId = 0)
        {
            try
            {
                //Deactivate user details
                var data = ent.UserLogins.Find(id);
                if (data.IsActive == false)
                {
                    data.IsActive = true;
                }
                else
                {
                    data.IsActive = false;
                }
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                //delete user software link 
                ent.Database.ExecuteSqlCommand("delete from User_SoftwareLink where USERID=" + id);

                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }
        public ActionResult CustomerUserMapping(int userId, int menuId = 0)
        {
            var model = new List<CustomerUserMappingDTO>();           
            var data = GetUserCustomerList(userId).ToList();           
            model = data;  
            ViewBag.menuId = menuId;
            ViewBag.userId = userId;
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerUserMapping(List<CustomerUserMappingDTO> model)
        {
            string userid = model[0].UserId.ToString();
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {
                    //delete exixting software link for new link add
                    ent.Database.ExecuteSqlCommand("delete from CustomerUserMapping where UserId=" + userid);
                    //////// add assign rights to user ////////////
                    if (model != null && model.Count() > 0)
                    {
                        if (model.Any(x => x.IsChecked == true))
                        {
                            foreach (var item in model.Where(x => x.IsChecked == true).ToList())
                            {
                                var usl = new CustomerUserMapping
                                {
                                    UserId = item.UserId,
                                    CustomerId = item.CustomerId
                                };
                                ent.CustomerUserMappings.Add(usl);
                            }
                            ent.SaveChanges();
                        }
                        trans.Commit();
                        TempData["msg"] = "Record has been updated.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }
            return RedirectToAction("CustomerUserMapping", new { userId = userid, menuId = 0 });
        }
        private List<CustomerUserMappingDTO> GetUserCustomerList(int userId)
        {

            string Query = @"select case when cu.Id is null then 0 else cu.Id end as Id,c.Id as CustomerId,0 as UserId,CustomerName,CompanyName,
  ContactNo,Email, GSTIN,sm.StateName,cm.CityName
from Customer c
left join CustomerUserMapping cu on c.Id=cu.CustomerId
join StateMaster sm on c.State_Id=sm.Id
join CityMaster cm on c.City_Id=cm.Id
where (UserId="+ userId.ToString() + " or UserId is null) and IsActive=1";
            var customerData = ent.Database.SqlQuery<CustomerUserMappingDTO>(Query).ToList();
            var data = (from c in customerData
                       
                        orderby c.CustomerName ascending
                        select new CustomerUserMappingDTO
                        {
                            Id = c.Id,
                            CustomerId=c.CustomerId,
                            UserId=userId,                                                   
                            CustomerName = c.CustomerName,
                            CompanyName=c.CompanyName,
                            ContactNo = c.ContactNo,
                            Email = c.Email,
                            GSTIN = c.GSTIN,
                            StateName = c.StateName,
                            CityName = c.CityName,
                            IsChecked=c.Id==0?false:true
                            //  ParentCustomer = p1.CompanyName,
                            //  FullAddress = c.FullAddress,
                            // CreateDate = c.CreateDate,
                            // AlternateNo = c.AlternateNo,
                            //  CompanyName = c.CompanyName,

                        }).ToList();           
            return data;
        }
        //**********Client Interface
        public ActionResult ClientAll(string term = "", int page = 1, int menuId = 0)
        {
            var model = new ClientUsersViewModel();
            //var data = ent.ClientUsers.Where(a => a.IsActive==true).OrderByDescending(a => a.Id).ToList();
            string cpbQuery = @"select cu.*,cm.CompanyName from ClientUser cu join ClientUserMapping cum
on cu.id=cum.UserId join Customer cm on cum.CustomerId=cm.Id";
            var data = ent.Database.SqlQuery<ClientUserAll>(cpbQuery).ToList();

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.Email.ToLower().Contains(term) || a.MobileNumber == term).ToList();
                page = 1;
            }
            // pagination
            int total = data.Count;
            int pageSize = 200;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.clientUsers = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }
        public ActionResult AddClient(int menuId = 0)
        {
            ViewBag.menuId = menuId;
            VRCHelper vc= new VRCHelper();
            var OrganizationList = vc.getCustomerWithCity();
            ViewBag.customerlist = OrganizationList; //ent.Customers.Where(x => x.IsActive == true).ToList().OrderBy(a => a.CompanyName).ToList();// OrganizationList;
            return View();
        }

        [HttpPost]
        public ActionResult AddClient(ClientUserDTO model)
        {
            try
            {
                VRCHelper vc = new VRCHelper();
                var OrganizationList = vc.getCustomerWithCity();
                ViewBag.customerlist =  OrganizationList;

                if (!ModelState.IsValid)
                    return View(model);
                if (ent.ClientUsers.Any(a => a.MobileNumber.ToLower().Equals(model.MobileNumber.ToLower())))
                {
                    TempData["msg"] = "Client Mobile Number is already exist in our database";
                    return View(model);
                }

                var cuser = Mapper.Map<ClientUser>(model);
                cuser.CreatedDate = DateTime.Now;
                cuser.CreatedBy = Convert.ToString(Session["uEmail"]);
                cuser.IsActive = true;
                cuser.Role = "Client";
                ent.ClientUsers.Add(cuser);
                ent.SaveChanges();

                AddClientUserMapping(cuser.Id,model.Customer_Id);
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("ClientAll", new { menuId = model.MenuId });
        }

        public ActionResult EditClient(int id, int menuId = 0)
        {
            var data = ent.ClientUsers.Find(id);
            var model = Mapper.Map<ClientUserDTO>(data);
            if(ent.ClientUserMappings.Any(x => x.UserId == id)) 
            { 
            model.Customer_Id = ent.ClientUserMappings.Where(x => x.UserId == id).FirstOrDefault().CustomerId;
            }
            VRCHelper vc = new VRCHelper();
            var OrganizationList = vc.getCustomerWithCity();
            ViewBag.customerlist =  OrganizationList;
            ViewBag.menuId = menuId;
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditClient(ClientUserDTO model)
        {
            try
            {
                VRCHelper vc = new VRCHelper();
                var OrganizationList = vc.getCustomerWithCity();
                ViewBag.customerlist =  OrganizationList;
                if (!ModelState.IsValid)
                    return View(model);
                var data = Mapper.Map<ClientUser>(model);
                ent.Entry(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                AddClientUserMapping(model.Id, model.Customer_Id);
                TempData["msg"] = "Record has updated.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("EditClient", new { id = model.Id, menuId = model.MenuId });

        }

        public ActionResult ClientUserMapping(int userId, int menuId = 0)
        {
            var model = new List<ClientUserMappingDTO>();
            var data = GetUserClientList(userId).ToList();
            model = data;
            ViewBag.menuId = menuId;
            ViewBag.userId = userId;
            return View(model);
        }
        [HttpPost]
        public ActionResult ClientUserMapping(List<ClientUserMappingDTO> model)
        {
            string userid = model[0].UserId.ToString();
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {
                    //delete exixting software link for new link add
                    ent.Database.ExecuteSqlCommand("delete from ClientUserMapping where UserId=" + userid);
                    //////// add assign rights to user ////////////
                    if (model != null && model.Count() > 0)
                    {
                        if (model.Any(x => x.IsChecked == true))
                        {
                            foreach (var item in model.Where(x => x.IsChecked == true).ToList())
                            {
                                var usl = new ClientUserMapping
                                {
                                    UserId = item.UserId,
                                    CustomerId = item.CustomerId
                                };
                                ent.ClientUserMappings.Add(usl);
                            }
                            ent.SaveChanges();
                        }
                        trans.Commit();
                        TempData["msg"] = "Record has been updated.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }
            return RedirectToAction("CustomerUserMapping", new { userId = userid, menuId = 0 });
        }
        private List<ClientUserMappingDTO> GetUserClientList(int userId)
        {

            string Query = @"select case when cu.Id is null then 0 else cu.Id end as Id,c.Id as CustomerId,0 as UserId,CustomerName,CompanyName,
  ContactNo,Email, GSTIN,sm.StateName,cm.CityName
from Customer c
left join (select * from ClientUserMapping where UserId=" + userId.ToString() + ") cu on c.Id=cu.CustomerId join " +
"StateMaster sm on c.State_Id=sm.Id join CityMaster cm on c.City_Id=cm.Id where  IsActive=1 order by c.CustomerName";

            var customerData = ent.Database.SqlQuery<ClientUserMappingDTO>(Query).ToList();
            var data = (from c in customerData

                        orderby c.CustomerName ascending
                        select new ClientUserMappingDTO
                        {
                            Id = c.Id,
                            CustomerId = c.CustomerId,
                            UserId = userId,
                            CustomerName = c.CustomerName,
                            CompanyName = c.CompanyName,
                            ContactNo = c.ContactNo,
                            Email = c.Email,
                            GSTIN = c.GSTIN,
                            StateName = c.StateName,
                            CityName = c.CityName,
                            IsChecked = c.Id == 0 ? false : true
                            //  ParentCustomer = p1.CompanyName,
                            //  FullAddress = c.FullAddress,
                            // CreateDate = c.CreateDate,
                            // AlternateNo = c.AlternateNo,
                            //  CompanyName = c.CompanyName,

                        }).ToList();
            return data;
        }

        private void AddClientUserMapping(int userId,int customerId)
        {
            string UserId = userId.ToString();
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {
                    //delete exixting software link for new link add
                    ent.Database.ExecuteSqlCommand("delete from ClientUserMapping where UserId=" + userId);
                    //// add assign rights to user ////////////
                      
                                var usl = new ClientUserMapping
                                {
                                    UserId = userId,
                                    CustomerId = customerId
                                };
                                ent.ClientUserMappings.Add(usl);
                           
                            ent.SaveChanges();
                        
                        trans.Commit();
                        TempData["msg"] = "Record has been updated.";
                   
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "Server error";
                    trans.Rollback();
                }
            }
           
        }
    }
}