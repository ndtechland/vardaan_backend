using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class EscortETSController : Controller
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        // GET: EscortETS
        public ActionResult Escort(int menuId = 0, int id = 0)
        {
            var model = new EscortDTO();
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            model.Vendors = new SelectList(ent.Vendors.Where(x => x.IsActive == true).ToList(), "Id", "VendorName");
            
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.Escorts.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.EscortFatheName = data.EscortFatheName;
                model.CompanyId = data.CompanyId;
                model.EscortName = data.EscortName;
                model.EscortMobileNumber = data.EscortMobileNumber;
                model.EscortAadhaarNumber = data.EscortAadhaarNumber;
                model.VendorId = data.VendorId;
                model.DOB = data.DOB;
                model.EscortEmployeeId = data.EscortEmployeeId;
                model.Pincode = data.Pincode;
                model.PermanentAddress = data.PermanentAddress;
                model.EscortAddress = data.EscortAddress;
                ViewBag.Heading = "Update Escort";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.EscortFatheName = "";
                model.CompanyId = 0;
                model.EscortName = "";
                model.EscortMobileNumber = "";
                model.EscortAadhaarNumber = "";
                model.VendorId = 0;
                model.DOB = null;
                model.EscortEmployeeId = "";
                model.Pincode = "";
                model.PermanentAddress = "";
                model.EscortAddress = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create Escort";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Escort(EscortDTO model)
        {
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            model.Vendors = new SelectList(ent.Vendors.Where(x => x.IsActive == true).ToList(), "Id", "VendorName");

            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var domainmodel = new Escort()
                    {
                        EscortFatheName = model.EscortFatheName,
                        CompanyId = model.CompanyId,
                        EscortName = model.EscortName,
                        EscortMobileNumber = model.EscortMobileNumber,
                        EscortAadhaarNumber = model.EscortAadhaarNumber,
                        VendorId = model.VendorId,
                        DOB = model.DOB,
                        EscortEmployeeId = model.EscortEmployeeId,
                        Pincode = model.Pincode,
                        PermanentAddress = model.PermanentAddress,
                        EscortAddress = model.EscortAddress,
                        IsActive = true,
                        CreatedDate = DateTime.Now

                    };
                    ent.Escorts.Add(domainmodel);
                }
                else
                {
                    var data = ent.Escorts.Find(model.Id);
                    data.EscortFatheName = model.EscortFatheName;
                    data.CompanyId = model.CompanyId;
                    data.EscortName = model.EscortName;
                    data.EscortMobileNumber = model.EscortMobileNumber;
                    data.EscortAadhaarNumber = model.EscortAadhaarNumber;
                    data.VendorId = model.VendorId;
                    data.DOB = model.DOB;
                    data.EscortEmployeeId = model.EscortEmployeeId;
                    data.Pincode = model.Pincode;
                    data.PermanentAddress = model.PermanentAddress;
                    data.EscortAddress = model.EscortAddress;

                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Escort", new { menuId = model.MenuId });
        }
        public ActionResult EscortsList(int menuId = 0)
        {
            try
            {
                var model = new EscortDTO();
                var data = (from er in ent.Escorts
                            join e in ent.Employees on er.EscortEmployeeId equals e.Employee_Id
                            join c in ent.Customers on er.CompanyId equals c.Id
                            join v in ent.Vendors on er.VendorId equals v.Id
                            orderby er.Id descending
                            select new Escorts
                            {
                                Id = er.Id,
                                CompanyName = c.CustomerName,
                                EscortName = er.EscortName,
                                EscortMobileNumber = er.EscortMobileNumber,
                                EscortAadhaarNumber = er.EscortAadhaarNumber,
                                EscortFatheName = er.EscortFatheName,
                                EmployeeFullName = e.Employee_First_Name + " " + e.Employee_Middle_Name + " " + e.Employee_Last_Name,                                
                                VendorName = v.VendorName,
                                DOB = er.DOB,
                                EscortEmployeeId = er.EscortEmployeeId,
                                Pincode = er.Pincode,
                                PermanentAddress = er.PermanentAddress,
                                EscortAddress = er.EscortAddress,
                                CreatedDate = er.CreatedDate
                            }
                            ).ToList();

                ViewBag.menuId = menuId;
                model.EscortList = data;
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult DeleteEscort(int id)
        {
            try
            {
                var data = ent.Escorts.Find(id);
                ent.Escorts.Remove(data);
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("EscortsList");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}