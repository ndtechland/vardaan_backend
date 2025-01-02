using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class AdministratorETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly IAdministrator _administrator;
        public AdministratorETSController(IAdministrator administrator)
        {
            _administrator = administrator;
        }
        public ActionResult AssignAccess(int menuId = 0, int id = 0)
        {
            var model = new AccessAssignDTO();
            model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).OrderByDescending(c => c.Id).ToList(), "Id", "OrgName");
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
                var roleinfo = ent.UserRoles.Where(r => r.Id == data.UserRoleId).FirstOrDefault();
                model.Id = data.Id;
                model.CompanyId = data.CompanyId;
                model.EmployeeId = data.EmployeeId;
                model.UserRoleId = data.EmployeeId;
                ViewBag.UserRoleId = data.UserRoleId;
                ViewBag.EmployeeId = data.EmployeeId;

                model.IsAllRead = (bool)roleinfo.IsAllRead;
                model.IsAllWrite = (bool)roleinfo.IsAllWrite;
                ViewBag.IsAllRead = (bool)roleinfo.IsAllWrite;
                ViewBag.IsAllWrite = (bool)roleinfo.IsAllWrite;
                model.IsReadChecked = !string.IsNullOrEmpty(roleinfo.IsReadChecked) ? StringToIntArray(roleinfo.IsReadChecked) : null;
                model.IsWriteChecked = !string.IsNullOrEmpty(roleinfo.IsWriteChecked) ? StringToIntArray(roleinfo.IsWriteChecked) : null;
                model.IsSubReadChecked = !string.IsNullOrEmpty(roleinfo.IsSubReadChecked) ? StringToIntArray(roleinfo.IsSubReadChecked) : null;
                model.IsSubWriteChecked = !string.IsNullOrEmpty(roleinfo.IsSubWriteChecked) ? StringToIntArray(roleinfo.IsSubWriteChecked) : null; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
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
        public async Task<ActionResult> AssignAccess(AccessAssignDTO model)
        {
            try
            {
                bool isCreated=await _administrator.AddUpdateAssignAccess(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                }
                else
                {
                    TempData["errormsg"] = "Failed.";
                }

            }
            catch (Exception)
            {
                TempData["errormsg"] = "Server error";
            }
            return RedirectToAction("AssignAccess", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> AccessAssignList()
        {
            try
            {
                var model = new AccessAssignDTO();
                
                model.AccessAssignList = await _administrator.GetAccessAssigns();
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception ( "Server error" +ex);
            }
        }
        
        private static int[] StringToIntArray(string myNumbers)
        {
            List<int> myIntegers = new List<int>();
            Array.ForEach(myNumbers.Split(",".ToCharArray()), s =>
            {
                int currentInt;
                if (Int32.TryParse(s, out currentInt))
                    myIntegers.Add(currentInt);
            });
            return myIntegers.ToArray();
        }
        public async Task<ActionResult> CreateRole(int menuId = 0, int id = 0)
        {
            var model = new UserRoleDTO();
            // model.Companies = new SelectList(ent.Customers.Where(c => c.IsActive == true).OrderByDescending(c=>c.Id).ToList(), "Id", "OrgName");
            model.Companies = ent.Customers.OrderByDescending(c => c.Id).Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.OrgName
            }).ToList();
           
            model.UserRoleLists = await _administrator.GetRoles();

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
                model.IsAllRead = (bool)data.IsAllRead;
                model.IsAllWrite = (bool)data.IsAllWrite;
                ViewBag.IsAllRead = (bool)data.IsAllWrite;
                ViewBag.IsAllWrite = (bool)data.IsAllWrite;
                model.IsReadChecked = !string.IsNullOrEmpty(data.IsReadChecked) ? StringToIntArray(data.IsReadChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
                model.IsWriteChecked = !string.IsNullOrEmpty(data.IsWriteChecked) ? StringToIntArray(data.IsWriteChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
                model.IsSubReadChecked = !string.IsNullOrEmpty(data.IsSubReadChecked) ? StringToIntArray(data.IsSubReadChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
                model.IsSubWriteChecked = !string.IsNullOrEmpty(data.IsSubWriteChecked) ? StringToIntArray(data.IsSubWriteChecked) : new int[0]; //.Split(',').Select(item => int.TryParse(item, out int number) ? number : 0).ToArray();
                ViewBag.Heading = "Update Role";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.RoleName = "";
                model.IsReadChecked = null;
                model.IsWriteChecked = null;
                model.IsSubReadChecked = null;
                model.IsSubWriteChecked = null;
                model.IsAllWrite = false;
                model.IsAllRead = false;
                ViewBag.IsAllRead = false;
                ViewBag.IsAllWrite = false;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Role";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateRole(UserRoleDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                bool isCreated= await _administrator.AddupdateRole(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                    return RedirectToAction("CreateRole", new { menuId = model.MenuId });

                }
                else
                {
                    TempData["msg"] = "Already exist.";
                    return RedirectToAction("CreateRole", new { menuId = model.MenuId });
                }

            }
            catch (Exception)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("CreateRole", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> DeleteUserRole(int id)
        {
            try
            {
                bool isDeleted= await _administrator.DeleteRole(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("CreateRole");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GetDataByRoleId(int roleId)
        {
            var model = new AccessAssignDTO();
            var roleinfo = ent.UserRoles.Where(r => r.Id == roleId).FirstOrDefault();
            model.CompanyId = roleinfo.Id;

            model.IsAllRead = (bool)roleinfo.IsAllRead;
            model.IsAllWrite = (bool)roleinfo.IsAllWrite;
            ViewBag.IsAllRead = (bool)roleinfo.IsAllWrite;
            ViewBag.IsAllWrite = (bool)roleinfo.IsAllWrite;
            model.IsReadChecked = !string.IsNullOrEmpty(roleinfo.IsReadChecked) ? StringToIntArray(roleinfo.IsReadChecked) : null;
            model.IsWriteChecked = !string.IsNullOrEmpty(roleinfo.IsWriteChecked) ? StringToIntArray(roleinfo.IsWriteChecked) : null;
            model.IsSubReadChecked = !string.IsNullOrEmpty(roleinfo.IsSubReadChecked) ? StringToIntArray(roleinfo.IsSubReadChecked) : null;
            model.IsSubWriteChecked = !string.IsNullOrEmpty(roleinfo.IsSubWriteChecked) ? StringToIntArray(roleinfo.IsSubWriteChecked) : null; //.Spl

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Routing()
        {
            try
            {
                return View();  
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
    }
}