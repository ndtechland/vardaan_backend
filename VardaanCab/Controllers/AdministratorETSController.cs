using DocumentFormat.OpenXml.Drawing.Charts;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class AdministratorETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        
        public ActionResult AssignAccess(int menuId = 0, int id = 0)
        {
            var model = new AccessAssignDTO();
            model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).OrderByDescending(c=>c.Id).ToList(), "Id", "OrgName");
            int userId = int.Parse(User.Identity.Name);
            if (!ent.UserLogins.Any(x => x.Id == userId && x.Role == "Customer"))
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                model.SoftwareLinkDTO = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in model.SoftwareLinkDTO)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + " and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
            }
            else
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (" + 1141 + "," + 1142 + ")";
                model.SoftwareLinkDTO = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in model.SoftwareLinkDTO)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + "";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
            }
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.AccessAssigns.Where(x => x.Id == id).FirstOrDefault();
                model.CompanyId = data.Id;
                model.EmployeeId = data.EmployeeId;
                model.UserRoleId = data.EmployeeId;
                ViewBag.UserRoleId = data.UserRoleId;
                ViewBag.EmployeeId = data.EmployeeId;
                ViewBag.Heading = "Update Assign Access";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.EmployeeId = 0;
                model.UserRoleId = 0;
                ViewBag.UserRoleId = 0;
                ViewBag.EmployeeId = 0;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Assign Access";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult AssignAccess(AccessAssignDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var DomainModel = new AccessAssign()
                    {
                        CompanyId = model.CompanyId,
                        UserRoleId = model.UserRoleId,
                        EmployeeId = model.EmployeeId,
                        IsActive = true,
                        CreatedDate = DateTime.Now

                    };
                    ent.AccessAssigns.Add(DomainModel);
                }
                else
                {
                    var data = ent.AccessAssigns.Find(model.Id);
                    data.CompanyId = model.CompanyId;
                    data.UserRoleId = model.UserRoleId;
                    data.EmployeeId = model.EmployeeId;
                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("AssignAccess", new { menuId = model.MenuId });
        }
        public ActionResult AssignAccessList()
        {
            return View();
        }
        public ActionResult CreateRole(int menuId = 0, int id = 0)
        {
            var model = new UserRoleDTO();
            // model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).OrderByDescending(c=>c.Id).ToList(), "Id", "OrgName");
            model.Companies = ent.Customers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.OrgName
            }).ToList();
            var Getdata = (from r in ent.UserRoles
                        join c in ent.Customers on r.CompanyId equals c.Id
                        where r.IsActive==true
                        orderby r.Id descending
                        select new UserRoleList
                        {
                            Id = r.Id,
                            CompanyName = c.CompanyName,
                            OrgName = c.OrgName,
                            RoleName = r.RoleName
                        }
                       ).ToList();
            model.UserRoleLists = Getdata;

            int userId = int.Parse(User.Identity.Name);
            if (!ent.UserLogins.Any(x => x.Id == userId && x.Role == "Customer"))
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                model.SoftwareLinkDTO = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in model.SoftwareLinkDTO)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + " and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
            }
            else
            {
                var query = @"select * from SoftwareLink where  IsHeading=1 and Id in (" + 1141 + "," + 1142 + ")";
                model.SoftwareLinkDTO = ent.Database.SqlQuery<SoftwareLinkDTO>(query).ToList();
                foreach (var item in model.SoftwareLinkDTO)
                {
                    var q = @"select * from SoftwareLink where  Parent_Id=" + item.Id + "";
                    var l = ent.Database.SqlQuery<SoftwareLink>(q).ToList();
                    item.ChildMenus = l;
                }
            }

            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.UserRoles.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.CompanyId = data.CompanyId;
                model.RoleName = data.RoleName;
                ViewBag.Heading = "Update Role";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.RoleName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Role";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult CreateRole(UserRoleDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var EmpReq = new UserRole()
                    {
                        CompanyId = model.CompanyId,
                        RoleName = model.RoleName,
                        IsActive=true,
                        CreatedDate = DateTime.Now

                    };
                    ent.UserRoles.Add(EmpReq);
                    
                }
                else
                {
                    var data = ent.UserRoles.Find(model.Id); 
                    data.CompanyId = model.CompanyId;
                    data.RoleName = model.RoleName;
                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("CreateRole", new { menuId = model.MenuId });
        }
        public ActionResult DeleteUserRole(int id)
        {
            try
            {
                var data = ent.UserRoles.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("CreateRole");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}