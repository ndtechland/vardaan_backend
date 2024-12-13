using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public class DepartmentETSController : Controller
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        // GET: DepartmentETS
        public ActionResult Departments(int menuId = 0, int id = 0)
        {
            var model = new DepartmentMasterDTO();
            var data = (from dm in ent.DepartmentMasters
                        join c in ent.Customers on dm.CompanyId equals c.Id
                        orderby dm.Id descending
                        where dm.IsActive == true
                        select new DepartmentList
                        {
                            Id = dm.Id,
                            DepartmentName = dm.DepartmentName,
                            CompanyName = c.CompanyName,  
                            CreatedDate = dm.Created,  
                        }
                       ).ToList();
            model.DepartmentMasterList = data;
            ViewBag.menuId = menuId;
           
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x=>x.Id).ToList(), "Id", "OrgName");
            if (id > 0)
            {
                var Depdata = ent.DepartmentMasters.Where(x => x.Id == id).FirstOrDefault();
                model.Id = Depdata.Id;
                model.CompanyId = Depdata.CompanyId;
                model.DepartmentName = Depdata.DepartmentName;
                ViewBag.Heading = "Update Department";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.DepartmentName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Department";
                return View(model);
            } 
        }
        [HttpPost]
        public ActionResult Departments(DepartmentMasterDTO model)
        {
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");
            try
            {               
                if (model.Id == 0)
                {
                    var department = new DepartmentMaster()
                    {
                        CompanyId = model.CompanyId,
                        DepartmentName = model.DepartmentName,
                        IsActive = true,
                        Created = DateTime.Now

                    };
                    ent.DepartmentMasters.Add(department);
                }
                else
                {
                    var data = ent.DepartmentMasters.Find(model.Id);
                    data.Id = model.Id;
                    data.CompanyId = model.CompanyId;
                    data.DepartmentName = model.DepartmentName;
                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Departments", new { menuId = model.MenuId });
        }
        public ActionResult DeleteDepartment(int id)
        {
            try
            {
                var data = ent.DepartmentMasters.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("Departments");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}