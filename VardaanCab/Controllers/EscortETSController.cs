using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
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
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;

        // GET: EscortETS
        public ActionResult Escort(int menuId = 0, int id = 0)
        {
            var model = new EscortDTO();
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).ToList(), "Id", "OrgName");
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
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).ToList(), "Id", "OrgName");
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
                TempData["msg"] = "Server error"+ ex;
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
                            where er.IsActive == true
                            orderby er.Id descending
                            select new Escorts
                            {
                                Id = er.Id,
                                CompanyName = c.CompanyName,
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
                data.IsActive = false;
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("EscortsList");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExportToExcel()
        {
            string qry = @"SELECT 
    er.Id,
    c.CompanyName,
    er.EscortName,
    er.EscortFatheName,
er.EscortMobileNumber,
    v.VendorName,
    er.DOB,
    er.EscortEmployeeId,
    er.Pincode,
    er.PermanentAddress,
    er.EscortAddress,
    er.CreatedDate
FROM Escort er
JOIN Employee e ON er.EscortEmployeeId = e.Employee_Id
JOIN Customer c ON er.CompanyId = c.Id
JOIN Vendor v ON er.VendorId = v.Id
ORDER BY er.Id DESC;";
            var data = ent.Database.SqlQuery<Escorts>(qry).ToList();
            var excelData = ExportVisitsToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "escorts.xlsx");
        }
        public byte[] ExportVisitsToExcel(IEnumerable<Escorts> Escorts)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Escorts");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Escort Name";
                worksheet.Cell(currentRow, 2).Value = "Escort Id";
                worksheet.Cell(currentRow, 3).Value = "Escort Gender";
                worksheet.Cell(currentRow, 4).Value = "Escort Address";
                worksheet.Cell(currentRow, 5).Value = "Permanent Address";
                worksheet.Cell(currentRow, 6).Value = "Escort Mobile Number";
                worksheet.Cell(currentRow, 7).Value = "Date Of Birth";
                worksheet.Cell(currentRow, 8).Value = "Escort Vendor Name";
                worksheet.Cell(currentRow, 9).Value = "Escort Batch Number";
                worksheet.Cell(currentRow, 10).Value = "Escort Father Name";
                worksheet.Cell(currentRow, 11).Value = "Escort Pin code";
                worksheet.Cell(currentRow, 12).Value = "Remarks";
                worksheet.Cell(currentRow, 13).Value = "Facility Name";
                currentRow++;
                foreach (var item in Escorts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.EscortName;
                    worksheet.Cell(currentRow, 2).Value = item.Id;
                    worksheet.Cell(currentRow, 3).Value = item.Gender;
                    worksheet.Cell(currentRow, 4).Value = item.EscortAddress;
                    worksheet.Cell(currentRow, 5).Value = item.PermanentAddress;
                    worksheet.Cell(currentRow, 6).Value = item.EscortMobileNumber;
                    worksheet.Cell(currentRow, 7).Value = item.DOB;
                    worksheet.Cell(currentRow, 8).Value = item.VendorName;
                    worksheet.Cell(currentRow, 9).Value = "";
                    worksheet.Cell(currentRow, 10).Value = item.EscortFatheName;
                    worksheet.Cell(currentRow, 11).Value = item.Pincode;
                    worksheet.Cell(currentRow, 12).Value = "";
                    worksheet.Cell(currentRow, 13).Value = item.CompanyName;
                    currentRow++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}