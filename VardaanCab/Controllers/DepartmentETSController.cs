using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using NPOI.OpenXmlFormats.Wordprocessing;
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
    public class DepartmentETSController : Controller
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        private readonly IDepartment _department;
        public DepartmentETSController(IDepartment department)
        {
            _department = department;
        }
        // GET: DepartmentETS
        public async Task<ActionResult> Departments(int menuId = 0, int id = 0)
        {
            var model = new DepartmentMasterDTO();
            model.DepartmentMasterList = await _department.DepartmentList();
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
        public async Task<ActionResult> Departments(DepartmentMasterDTO model)
        {
            //model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");
            try
            {
                bool isCreated = await _department.AddDepartments(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Departments", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            try
            {
                bool isCreated= await _department.DeleteDepartment(id);
                if (isCreated)
                {
                    TempData["dltmsg"] = "Deleted successfully.";
                }
                else
                {
                    TempData["dltmsg"] = "Data not found.";
                }                
                return RedirectToAction("Departments");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}