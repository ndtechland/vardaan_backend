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
            var model = new createorg();
            model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).ToList(), "Id", "CustomerName");
            model.OrgNameList = ent.Customers.Where(c => c.OrgName != null).ToList();
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
        public ActionResult AssignAccessList()
        {
            return View();
        }
        public ActionResult CreateRole(int menuId = 0, int id = 0)
        {
            var model = new UserRoleDTO();
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
                var data = ent.UserRoles.Where(x => x.Id == id).FirstOrDefault();
                model.CompanyId = data.Id;
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
    }
}