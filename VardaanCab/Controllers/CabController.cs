using AutoMapper;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class CabController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public ActionResult GetVendorList(string term)
        {
            var data = ent.Vendors.Where(a => a.IsActive && a.CompanyName.ToLower().Contains(term.ToLower())).ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatus(int id,int menuId=0)
        {
            string query = @"update cab set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
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

        public ActionResult ChangeAvailStatus (int id, int menuId = 0)
        {
            string query = @"update cab set IsAvailable= case when isactive=1 then 0 else 1 end where id=" + id;
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

        public ActionResult All(string term = "", int page = 1,bool export=false, int menuId=0)
        {
            var model = new CabListVm();
            var data = (from c in ent.Cabs join vm in ent.VehicleModels
                        on c.VehicleModel_Id equals vm.Id into c_vm 
                        from vehiclModel in c_vm.DefaultIfEmpty()
                        join v in ent.Vendors on c.Vendor_Id equals v.Id into c_v
                        from cv in c_v.DefaultIfEmpty()
                        orderby c.Id descending
                        select new CabDTO
                        {
                            Id=c.Id,
                            CreateDate=c.CreateDate,
                            Company=c.Company,
                            CompanyName=c.Company,
                            ModelName= vehiclModel.ModelName,
                            FitnessVality=c.FitnessVality,
                            RCDoc=c.RCDoc,
                            RcIssueDate=c.RcIssueDate,
                            RcValidity=c.RcValidity,
                            RcNumber=c.RcNumber,
                            PermitValidity=c.PermitValidity,
                            IsActive=c.IsActive,
                            FitnessDoc=c.FitnessDoc,
                            InsuranceDoc=c.InsuranceDoc,
                            InsurranceValidity=c.InsurranceValidity,
                            PolutionDoc=c.PolutionDoc,
                            PolutionValidity=c.PolutionValidity,
                            VehicleModel_Id=c.VehicleModel_Id,
                            VendorName= cv.CompanyName,
                            VehicleNumber=c.VehicleNumber,
                            Vendor_Id=c.Vendor_Id,
                            PermitDoc=c.PermitDoc,
                            PermitNo=c.PermitNo,
                            IsAvailable=c.IsAvailable,
                            FuelEfficiency=c.FuelEfficiency
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.Company.ToLower().Contains(term) || a.VehicleNumber.ToLower().Contains(term) || a.ModelName.ToLower().Contains(term) ).ToList();
                page = 1;
            }

            if (export)
            {
               
                var ed = (from d in data
                          select new  CabExcelModel
                          {
                              CreateDate = d.CreateDate.Date,
                              ModelName = d.ModelName,
                              FitnessVality = d.FitnessVality.Date,
                              RcIssueDate = d.RcIssueDate,
                              RcValidity = d.RcValidity,
                              RcNumber = d.RcNumber,
                              PermitValidity = d.PermitValidity.Date,
                              InsurranceValidity = d.InsurranceValidity.Date,
                              PolutionValidity = d.PolutionValidity.Date,
                              VendorName = d.VendorName,
                              VehicleNumber = d.VehicleNumber,
                              PermitNo = d.PermitNo,
                              FuelEfficiency = d.FuelEfficiency
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =cabList.xls");
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
            int pageSize = 500;
            double totalPages = Math.Ceiling(total / (double)pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Cabs = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Add(int menuId = 0)
        {
            var model = new CabDTO();
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CabDTO model)
        {
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName");


            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if(ent.Cabs.Any(a=>a.VehicleNumber.ToLower().Equals(model.VehicleNumber.ToUpper())))
                {
                    TempData["msg"] = "Vehicle number already exist in our database";
                    return View(model);
                }

                if (model.FitnessDocFile != null)
                {
                    if (model.FitnessDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "fitness doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.FitnessDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as fitness doc";
                        return View(model);
                    }
                    model.FitnessDoc = img;
                }

                if (model.InsuranceDocFile != null)
                {
                    if (model.InsuranceDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Insurrance doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.InsuranceDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Insurrance doc";
                        return View(model);
                    }
                    model.InsuranceDoc = img;
                }

                if (model.PolutionDocFile != null)
                {
                    if (model.PolutionDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Polution Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolutionDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Polution Doc";
                        return View(model);
                    }
                    model.PolutionDoc = img;
                }

                if (model.RCDocFile != null)
                {
                    if (model.RCDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "RC Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.RCDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as RC Doc";
                        return View(model);
                    }
                    model.RCDoc = img;
                }

                if (model.PermitDocFile != null)
                {
                    if (model.PermitDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Permit doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PermitDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as permit doc";
                        return View(model);
                    }
                    model.PermitDoc = img;
                }
                model.VehicleNumber=model.VehicleNumber.ToUpper();
                var data = Mapper.Map<Cab>(model);
                data.CreateDate = DateTime.Now;
                data.IsActive = true;
                data.IsAvailable = true;
                ent.Cabs.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error,please contact to admin department.";
            }

            return RedirectToAction("Add", new {menuId=model.MenuId});
        }

        public ActionResult Edit(int id,int menuId=0)
        {
            var data = ent.Cabs.Find(id);
            var model = Mapper.Map<CabDTO>(data);
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName",data.VehicleModel_Id);
            var vendor = ent.Vendors.Find(data.Vendor_Id);
            if(vendor !=null)
            {
                model.CompanyName = vendor.CompanyName;
            }
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CabDTO model)
        {
            model.VehicleModels = new SelectList(ent.VehicleModels.ToList(), "Id", "ModelName", model.VehicleModel_Id);

            try
            {
               // var cabData = ent.Cabs.AsNoTracking().FirstOrDefault(a=>a.Id==model.Id);
                if (!string.IsNullOrEmpty(model.FitnessDoc))
                    ModelState.Remove("FitnessDocFile");

                if(!string.IsNullOrEmpty(model.PolutionDoc))
                    ModelState.Remove("PolutionDocFile");

                if(!string.IsNullOrEmpty(model.InsuranceDoc))
                    ModelState.Remove("InsuranceDocFile");

                if(!string.IsNullOrEmpty(model.RCDoc))
                    ModelState.Remove("RCDocFile");

                if (!string.IsNullOrEmpty(model.PermitDoc))
                    ModelState.Remove("PermitDocFile");

                if (!ModelState.IsValid)
                    return View(model);

                if (ent.Cabs.Any(a =>a.Id!=model.Id && a.VehicleNumber.ToLower().Equals(model.VehicleNumber.ToLower())))
                {
                    TempData["msg"] = "Vehicle number already exist in our database";
                    return View(model);
                }
                if (model.FitnessDocFile != null)
                {
                    if (model.FitnessDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "fitness doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.FitnessDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as fitness doc";
                        return View(model);
                    }
                    model.FitnessDoc = img;
                }

                if (model.InsuranceDocFile != null)
                {
                    if (model.InsuranceDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Insurrance doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.InsuranceDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Insurrance doc";
                        return View(model);
                    }
                    model.InsuranceDoc = img;
                }

                if (model.PolutionDocFile != null)
                {
                    if (model.PolutionDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Polution Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PolutionDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as Polution Doc";
                        return View(model);
                    }
                    model.PolutionDoc = img;
                }

                if (model.RCDocFile != null)
                {
                    if (model.RCDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "RC Doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.RCDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as RC Doc";
                        return View(model);
                    }
                    model.RCDoc = img;
                }
                if (model.PermitDocFile != null)
                {
                    if (model.PermitDocFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Permit doc size cant't exceed 1 mb";
                        return View(model);
                    }
                    var img = FileOperation.UploadPdf(model.PermitDocFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "only .pdf,.jpg,.jpeg,.png files are allowed as permit doc";
                        return View(model);
                    }
                    model.PermitDoc = img;
                }
                var data = Mapper.Map<Cab>(model);
                ent.Entry(data).State=System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has saved.";
            
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id,menuId=model.MenuId });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var data = ent.Cabs.Find(id);
                ent.Cabs.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

        //public ActionResult ExportToExcel()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("VehicleModel_Id");
        //    dt.Columns.Add("Company");
        //    dt.Columns.Add("VehicleNumber");
        //    dt.Columns.Add("FitnessVality");
        //    dt.Columns.Add("PolutionValidity");
        //    dt.Columns.Add("InsurranceValidity");
        //    dt.Columns.Add("FitnessDoc");
        //    dt.Columns.Add("PolutionDoc");
        //    dt.Columns.Add("InsuranceDoc");
        //    dt.Columns.Add("RCDoc");
        //    dt.Columns.Add("FuelEfficiency");
        //    dt.Columns.Add("PermitNo");
        //    dt.Columns.Add("PermitDoc");
        //    dt.Columns.Add("PermitValidity");
        //    dt.Columns.Add("RcNumber");
        //    dt.Columns.Add("RcValidity");
        //    dt.Columns.Add("RcIssueDate");
        //    dt.Columns.Add("Vendor_Id");

        //    Dictionary<string, string> columnMappings = new Dictionary<string, string>()
        //    {
        //    { "VehicleModel_Id", "VehicleModel" },
        //    { "Company", "Company" },
        //    { "VehicleNumber", "Vehicle Number" },
        //    { "FitnessVality", "Fitness Vality" },
        //    { "PolutionValidity", "Polution Validity" },
        //    { "InsurranceValidity", "Insurrance Validity" },
        //    { "FitnessDoc", "Fitness Doc" },
        //    { "PolutionDoc", "Polution Doc" },
        //    { "InsuranceDoc", "Insurance Doc" },
        //    { "RCDoc", "RC Doc" },
        //    { "FuelEfficiency", "FuelEfficiency" },
        //    { "PermitNo", "Permit No" },
        //    { "PermitDoc", "Permit Doc" },
        //    { "PermitValidity", "Permit Validity" },
        //    { "RcNumber", "Rc Number" },
        //    { "RcValidity", "Rc Validity" },
        //    { "RcIssueDate", "Rc Issue Date" },
        //    { "Vendor_Id", "Vendor" }
        //    };

        //    // Export to Excel
        //    using (XLWorkbook workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Cab");


        //        int colIndex = 1;
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            string oldColumnName = column.ColumnName;
        //            if (columnMappings.ContainsKey(oldColumnName))
        //            {
        //                worksheet.Cell(1, colIndex).Value = columnMappings[oldColumnName];
        //            }
        //            else
        //            {
        //                worksheet.Cell(1, colIndex).Value = oldColumnName;
        //            }
        //            worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.Yellow;
        //            colIndex++;
        //        }

        //        // Create a hidden sheet to store company names for dropdown
        //        var hiddenSheet = workbook.Worksheets.Add("VehicleModelList");
        //        var hiddenVendorSheet = workbook.Worksheets.Add("VendorList");


        //        // Retrieve active customers for the dropdown list
        //        var vehicleModelList = ent.VehicleModels.ToList();
        //        var vendorList = ent.Vendors.Where(x=>x.IsActive).ToList();

        //        // Populate hidden sheet with company names
        //        int hiddenRow = 1;
        //        foreach (var company in vehicleModelList.OrderByDescending(x => x.Id))
        //        {
        //            hiddenSheet.Cell(hiddenRow++, 1).Value = company.ModelName;
        //        }
        //        hiddenRow = 1;
        //        foreach (var ven in vendorList.OrderByDescending(x => x.Id))
        //        {
        //            hiddenVendorSheet.Cell(hiddenRow++, 1).Value = ven.VendorName;
        //        }
        //        // Define the dropdown list range
        //        var veicleModelRange = hiddenSheet.Range($"A1:A{vehicleModelList.Count}");
        //        var VendorRange = hiddenVendorSheet.Range($"A1:A{vendorList.Count}");


        //        //Apply dropdown list validation to cell A2(under "Company ID")
        //        var validationOne = worksheet.Cell(2, 1).DataValidation;
        //        validationOne.List(veicleModelRange); // Dropdown from hidden sheet
        //        validationOne.IgnoreBlanks = true;
        //        validationOne.InCellDropdown = true;

        //        //Vendor
        //        var validationVendorOne = worksheet.Cell(2, 18).DataValidation;
        //        validationVendorOne.List(VendorRange); // Dropdown from hidden sheet
        //        validationVendorOne.IgnoreBlanks = true;
        //        validationVendorOne.InCellDropdown = true;

        //        // Save and return Excel file as download
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            stream.Position = 0;
        //            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CabData.xlsx");
        //        }
        //    }
        //}

        public ActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("VehicleModel_Id");
            dt.Columns.Add("Company");
            dt.Columns.Add("VehicleNumber");
            dt.Columns.Add("FitnessVality");
            dt.Columns.Add("PolutionValidity");
            dt.Columns.Add("InsurranceValidity");
            //dt.Columns.Add("FitnessDoc");
            //dt.Columns.Add("PolutionDoc");
            //dt.Columns.Add("InsuranceDoc");
            //dt.Columns.Add("RCDoc");
            dt.Columns.Add("FuelEfficiency");
            dt.Columns.Add("PermitNo");
            //dt.Columns.Add("PermitDoc");
            dt.Columns.Add("PermitValidity");
            dt.Columns.Add("RcNumber");
            dt.Columns.Add("RcValidity");
            dt.Columns.Add("RcIssueDate");
            dt.Columns.Add("Vendor_Id");

            // Adding dummy data
            dt.Rows.Add("CIAZ", "MARUTI", "ABC123", "2025-01-01", "2025-06-01", "2025-12-01", "20", "Permit123", "2025-11-01", "RC123", "2025-08-01", "2024-12-01", "Vendor 1");

            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
    {
        { "VehicleModel_Id", "VehicleModel" },
        { "Company", "Company" },
        { "VehicleNumber", "Vehicle Number" },
        { "FitnessVality", "Fitness Vality" },
        { "PolutionValidity", "Polution Validity" },
        { "InsurranceValidity", "Insurrance Validity" },
        //{ "FitnessDoc", "Fitness Doc" },
        //{ "PolutionDoc", "Polution Doc" },
        //{ "InsuranceDoc", "Insurance Doc" },
        //{ "RCDoc", "RC Doc" },
        { "FuelEfficiency", "Fuel Efficiency" },
        { "PermitNo", "Permit No" },
        //{ "PermitDoc", "Permit Doc" },
        { "PermitValidity", "Permit Validity" },
        { "RcNumber", "RC Number" },
        { "RcValidity", "RC Validity" },
        { "RcIssueDate", "RC Issue Date" },
        { "Vendor_Id", "Vendor" }
    };

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cab");

                // Write the headers
                int colIndex = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    string oldColumnName = column.ColumnName;
                    if (columnMappings.ContainsKey(oldColumnName))
                    {
                        worksheet.Cell(1, colIndex).Value = columnMappings[oldColumnName];
                    }
                    else
                    {
                        worksheet.Cell(1, colIndex).Value = oldColumnName;
                    }
                    worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.Yellow;
                    colIndex++;
                }

                // Write the data rows
                int rowIndex = 2; // Start from the second row (below headers)
                foreach (DataRow row in dt.Rows)
                {
                    colIndex = 1; // Reset column index for each row
                    foreach (var item in row.ItemArray)
                    {
                        worksheet.Cell(rowIndex, colIndex).Value = item;
                        colIndex++;
                    }
                    rowIndex++;
                }

                // Create a hidden sheet to store company names for dropdown
                var hiddenSheet = workbook.Worksheets.Add("VehicleModelList");
                var hiddenVendorSheet = workbook.Worksheets.Add("VendorList");

                // Retrieve active customers for the dropdown list
                var vehicleModelList = ent.VehicleModels.ToList();
                var vendorList = ent.Vendors.Where(x => x.IsActive).ToList();

                // Populate hidden sheet with company names
                int hiddenRow = 1;
                foreach (var company in vehicleModelList.OrderByDescending(x => x.Id))
                {
                    hiddenSheet.Cell(hiddenRow++, 1).Value = company.ModelName;
                }
                hiddenRow = 1;
                foreach (var ven in vendorList.OrderByDescending(x => x.Id))
                {
                    hiddenVendorSheet.Cell(hiddenRow++, 1).Value = ven.VendorName;
                }
                // Define the dropdown list range
                var veicleModelRange = hiddenSheet.Range($"A1:A{vehicleModelList.Count}");
                var VendorRange = hiddenVendorSheet.Range($"A1:A{vendorList.Count}");

                // Apply dropdown list validation to cell A2(under "Company ID")
                var validationOne = worksheet.Cell(2, 1).DataValidation;
                validationOne.List(veicleModelRange); // Dropdown from hidden sheet
                validationOne.IgnoreBlanks = true;
                validationOne.InCellDropdown = true;

                // Vendor
                var validationVendorOne = worksheet.Cell(2, 13).DataValidation;
                validationVendorOne.List(VendorRange); // Dropdown from hidden sheet
                validationVendorOne.IgnoreBlanks = true;
                validationVendorOne.InCellDropdown = true;

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CabData.xlsx");
                }
            }
        }


        [HttpPost]
        public ActionResult ImportCabData(HttpPostedFileBase file)
        {
            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);  
                        List<Cab> cabs = new List<Cab>();

                        foreach (var row in rows)
                        {
                            string VehicleModel = row.Cell(1).GetValue<string>();
                            string Vendors = row.Cell(13).GetValue<string>();

                            Cab cab = new Cab
                            {
                                VehicleModel_Id = string.IsNullOrEmpty(VehicleModel) ? 0 :
                                    ent.VehicleModels.Where(x => x.ModelName.ToLower() == VehicleModel.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,

                                Company = row.Cell(2).GetValue<string>() ?? string.Empty,
                                VehicleNumber = row.Cell(3).GetValue<string>() ?? string.Empty,
                                FitnessVality = row.Cell(4).GetValue<DateTime>(),
                                PolutionValidity = row.Cell(5).GetValue<DateTime>(),
                                InsurranceValidity = row.Cell(6).GetValue<DateTime>(),                                
                                FuelEfficiency = row.Cell(7).GetValue<double?>() ?? 0.0,
                                PermitNo = row.Cell(8).GetValue<string>() ?? string.Empty,
                                PermitValidity = row.Cell(9).GetValue<DateTime>(),
                                RcNumber = row.Cell(10).GetValue<string>() ?? string.Empty,
                                RcValidity = row.Cell(11).GetValue<DateTime>(),
                                RcIssueDate = row.Cell(12).GetValue<DateTime>(),
                                Vendor_Id = string.IsNullOrEmpty(Vendors) ? 0 :
                                    ent.Vendors.Where(x => x.VendorName.ToLower() == Vendors.ToLower())
                                        .FirstOrDefault()?.Id ?? 0,
                                IsActive = true,
                                IsAvailable = true,
                                IsOutsider = false,
                                CreateDate = DateTime.Now,
                            };

                            cabs.Add(cab);
                        }

                        if (cabs.Any())
                        {
                            ent.Cabs.AddRange(cabs);
                            ent.SaveChanges();
                        }
                        TempData["dltmsg"] = "Data imported successfully!";
                        return RedirectToAction("All");
                    }
                }

                ViewBag.Message = "Please select an Excel file to import.";
                return View();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        // Log or output the validation errors
                        Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                ViewBag.Message = "Validation failed for one or more entities. Please check the logs for more details.";
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception or handle other errors
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }

    }
}