using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Domain.DTO;
using AutoMapper;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.ViewModels;
using VardaanCab.Utilities;
using VardaanCab.Repository;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.Entity.Validation;
using System.Security.Cryptography.X509Certificates;

namespace VardaanCab.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        // GET: Vendor
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        CommonRepository commonRepo = new CommonRepository();

        public ActionResult Add(int menuId = 0)
        {
            var model = new VendorDTO();
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.Vendors = new SelectList(ent.Vendors.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            model.Packages = commonRepo.GetAllVendorPackages().ToList();
            ViewBag.menuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VendorDTO model)
        {
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            model.Vendors = new SelectList(ent.Vendors.Where(a => a.IsActive).ToList(), "Id", "CompanyName");
            model.Packages = commonRepo.GetAllVendorPackages().ToList();
            if (!ModelState.IsValid)
                return View(model);
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (ent.Vendors.Any(a => a.CompanyName.ToLower().Equals(model.CompanyName.ToLower())))
                    {
                        TempData["msg"] = "Company name already exist in our database";
                        return View(model);
                    }
                    if (model.PanFile != null)
                    {
                        if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                        {
                            TempData["msg"] = "Pan file image should not exceed 1 mb";
                            return View(model);
                        }
                        var result = FileOperation.UploadImage(model.PanFile, "Images");
                        if (result == "not allowed")
                        {
                            TempData["msg"] = "Only .jpg, .png, .jpeg files are allowed as PAN Document";
                            return View(model);
                        }
                        model.PanImage = result;
                    }
                    var vendor = Mapper.Map<Vendor>(model);
                    vendor.CreateDate = DateTime.Now;
                    vendor.IsActive = true;
                    ent.Vendors.Add(vendor);
                    ent.SaveChanges();
                    if (model.Packages.Count > 0)
                    {
                        foreach (var item in model.Packages)
                        {
                            var vp = new VendorPersonalPackage
                            {
                                Vendor_Id = vendor.Id,
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
                                VendorPackage_Id = item.VendorPackage_Id,
                                PickupLocation = item.PickupLocation,
                                DropLocation = item.DropLocation,
                                NoOfDays = item.NoOfDays
                            };
                            ent.VendorPersonalPackages.Add(vp);
                        }
                        ent.SaveChanges();
                    }
                    tran.Commit();
                    TempData["msg"] = "Vendor has registered successfully";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["msg"] = "Server error";
                }
            }
            return RedirectToAction("Add", new { menuId = model.MenuId });
        }

        public ActionResult All(string term = "", int page = 1, bool export = false, int menuId = 0)
        {
            var model = new VendorListVm();
            var data = (from v in ent.Vendors
                        join s in ent.StateMasters on v.StateMaster_Id equals s.Id into v_s
                        from vendorState in v_s.DefaultIfEmpty()
                        join c in ent.CityMasters on v.CityMaster_Id equals c.Id into v_c
                        from vendorCity in v_c.DefaultIfEmpty()
                        join pVendor in ent.Vendors on v.ParentVendor_Id equals pVendor.Id into v_pVendor
                        from pv in v_pVendor.DefaultIfEmpty()
                        orderby v.Id descending
                        select new VendorDTO
                        {
                            Id = v.Id,
                            AlernateMobile = v.AlernateMobile,
                            CityName = vendorCity.CityName,
                            FullAddress = v.FullAddress,
                            StateName = vendorState.StateName,
                            Email = v.Email,
                            MobileNumber = v.MobileNumber,
                            ParentVendorName = pv == null ? "none" : pv.VendorName,
                            CityMaster_Id = v.CityMaster_Id,
                            StateMaster_Id = v.StateMaster_Id,
                            ParentVendor_Id = v.ParentVendor_Id,
                            CreateDate = v.CreateDate,
                            IsActive = v.IsActive,
                            CompanyName = v.CompanyName,
                            VendorName = v.VendorName,
                            GSTIN = v.GSTIN,
                            PAN = v.PAN,
                            CIN = v.CIN,
                            PanImage = v.PanImage
                        }
                        ).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                data = data.Where(a => a.VendorName.ToLower().Contains(term) || (!string.IsNullOrEmpty(a.MobileNumber) && a.MobileNumber.Contains(term)) || a.CompanyName.ToLower().Contains(term)).ToList();
                page = 1;
            }

            if (export)
            {

                var ed = (from d in data
                          select new VendorExcelModel
                          {
                              RegistrationDate = d.CreateDate.Date,
                              AlernateMobile = d.AlernateMobile,
                              CityName = d.CityName,
                              FullAddress = d.FullAddress,
                              StateName = d.StateName,
                              Email = d.Email,
                              MobileNumber = d.MobileNumber,
                              ParentVendorName = d.ParentVendorName == null ? "none" : d.ParentVendorName,
                              CompanyName = d.CompanyName,
                              VendorName = d.VendorName,
                              GSTIN = d.GSTIN,
                              PAN = d.PAN,
                              CIN = d.CIN,
                          }).ToList(); ;
                var grid = new GridView();
                grid.DataSource = ed;
                grid.DataBind();
                 
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename =vendorList.xls");
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
            model.Vendors = data;
            model.Term = term;
            model.Page = page;
            model.NumberOfPages = (int)totalPages;
            model.SrNo = skip;
            ViewBag.menuId = menuId;
            return View(model);
        }

        public ActionResult Edit(int id, int menuId = 0)
        {
            var data = ent.Vendors.Find(id);
            var model = Mapper.Map<VendorDTO>(data);
            model.States = new SelectList(ent.StateMasters.AsNoTracking().ToList(), "Id", "StateName", data.StateMaster_Id);
            model.Vendors = new SelectList(ent.Vendors.AsNoTracking().Where(a => a.IsActive).ToList(), "Id", "CompanyName", data.ParentVendor_Id);
            model.Cities = new SelectList(ent.CityMasters.AsNoTracking().ToList(), "Id", "CityName", data.CityMaster_Id);
            model.MenuId = menuId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorDTO model)
        {
            ModelState.Remove("PanFile");
            model.States = new SelectList(ent.StateMasters.AsNoTracking().ToList(), "Id", "StateName", model.StateMaster_Id);
            model.Vendors = new SelectList(ent.Vendors.AsNoTracking().Where(a => a.IsActive).ToList(), "Id", "CompanyName", model.ParentVendor_Id);
            model.Cities = new SelectList(ent.CityMasters.AsNoTracking().ToList(), "Id", "CityName", model.CityMaster_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if (ent.Vendors.Any(a => a.Id != model.Id && a.CompanyName.ToLower().Equals(model.CompanyName.ToLower())))
                {
                    TempData["msg"] = "Company name already exist in our database";
                    return View(model);
                }
                if (model.PanFile != null)
                {
                    if (model.PanFile.ContentLength > 1 * 1024 * 1024)
                    {
                        TempData["msg"] = "Pan file image should not exceed 1 mb";
                        return View(model);
                    }
                    var result = FileOperation.UploadImage(model.PanFile, "Images");
                    if (result == "not allowed")
                    {
                        TempData["msg"] = "Only .jpg, .png, .jpeg files are allowed as PAN Document";
                        return View(model);
                    }
                    model.PanImage = result;
                }
                var vendor = Mapper.Map<Vendor>(model);
                ent.Entry(vendor).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Vendor has updated successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("Edit", new { id = model.Id, menuId = model.MenuId });
        }

        public ActionResult Delete(int id, int menuId = 0)
        {
            try
            {
                var data = ent.Vendors.Find(id);
                ent.Vendors.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                return Content("Server error");
            }
        }

        public ActionResult ChangeStatus(int id, int menuId = 0)
        {
            string query = @"update vendor set IsActive= case when isactive=1 then 0 else 1 end where id=" + id;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("All", new { menuId = menuId });
        }

        public ActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CompanyName");
            dt.Columns.Add("VendorName");
            dt.Columns.Add("Email");
            dt.Columns.Add("MobileNumber");
            dt.Columns.Add("AlernateMobile");
            dt.Columns.Add("GSTIN");
            dt.Columns.Add("ParentVendor_Id");
            dt.Columns.Add("StateMaster_Id");
            dt.Columns.Add("CityMaster_Id");
            dt.Columns.Add("FullAddress");
            dt.Columns.Add("PAN");
            dt.Columns.Add("CIN");
            Dictionary<string, string> columnMappings = new Dictionary<string, string>()
            {
                { "CompanyName", "Company Name" },
                { "VendorName", "Vendor Name" },
                { "Email", "Email" },
                { "MobileNumber", "Mobile Number" },
                { "AlernateMobile", "Alternate Mobile" },
                { "GSTIN", "GSTIN" },
                { "ParentVendor_Id", "Parent Vendor ID" },
                { "StateMaster_Id", "State Master ID" },
                { "CityMaster_Id", "City Master ID" },
                { "FullAddress", "Full Address" },
                { "PAN", "PAN" },
                { "CIN", "CIN" }
            };

            // Export to Excel
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vendor");


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

                // Create a hidden sheet to store company names for dropdown
                var hiddenSheet = workbook.Worksheets.Add("ParentVendorList");
                var StatehiddenSheet = workbook.Worksheets.Add("StateList");
                var CityhiddenSheet = workbook.Worksheets.Add("CityList");


                // Retrieve active customers for the dropdown list
                var VendorList = ent.Vendors.Where(x => x.IsActive == true).ToList();
                var StateList = ent.StateMasters.ToList();
                var CityList = ent.CityMasters.ToList();


                // Populate hidden sheet with company names
                int hiddenRow = 1;
                foreach (var company in VendorList.OrderByDescending(x => x.Id))
                {
                    hiddenSheet.Cell(hiddenRow++, 1).Value = company.VendorName;
                }
                hiddenRow = 1;
                foreach (var state in StateList.OrderByDescending(x => x.Id))
                {
                    StatehiddenSheet.Cell(hiddenRow++, 1).Value = state.StateName;

                }
                hiddenRow = 1;
                foreach (var city in CityList.OrderByDescending(x => x.Id))
                {
                    CityhiddenSheet.Cell(hiddenRow++, 1).Value = city.CityName;
                }

                // Define the dropdown list range
                var companyRange = hiddenSheet.Range($"A1:A{VendorList.Count}");
                var StateRange = StatehiddenSheet.Range($"A1:A{StateList.Count}");
                var CityRange = CityhiddenSheet.Range($"A1:A{CityList.Count}");



                //Apply dropdown list validation to cell A2(under "Company ID")
                var validationOne = worksheet.Cell(2, 7).DataValidation;
                validationOne.List(companyRange); // Dropdown from hidden sheet
                validationOne.IgnoreBlanks = true;
                validationOne.InCellDropdown = true;

                var validationTwo = worksheet.Cell(2, 8).DataValidation;
                validationTwo.List(StateRange); // Dropdown from hidden sheet
                validationTwo.IgnoreBlanks = true;
                validationTwo.InCellDropdown = true;

                var validationThree = worksheet.Cell(2, 9).DataValidation;
                validationThree.List(CityRange); // Dropdown from hidden sheet
                validationThree.IgnoreBlanks = true;
                validationThree.InCellDropdown = true;

                // Save and return Excel file as download
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VendorData.xlsx");
                }
            }
        }

        public ActionResult ImportVendorData(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<Vendor> vendors = new List<Vendor>();

                        foreach (var row in rows)
                        {

                            string ParentVendorName = row.Cell(7).GetValue<string>();
                            string StateName = row.Cell(8).GetValue<string>();
                            string CityName = row.Cell(9).GetValue<string>();
                            Vendor vendor = new Vendor()
                            {
                                CompanyName = row.Cell(1).GetValue<string>().ToString(),
                                VendorName = row.Cell(2).GetValue<string>().ToString(),
                                Email = row.Cell(3).GetValue<string>().ToString(),
                                MobileNumber = row.Cell(4).GetValue<string>().ToString(),
                                AlernateMobile = row.Cell(5).GetValue<string>().ToString(),
                                GSTIN = row.Cell(6).GetValue<string>().ToString(),
                                ParentVendor_Id = string.IsNullOrEmpty(ParentVendorName) ? 0 : ent.Vendors.Where(x => x.VendorName.ToLower() == ParentVendorName.ToLower()).FirstOrDefault()?.Id ?? 0,
                                StateMaster_Id = string.IsNullOrEmpty(StateName) ? 0 : ent.StateMasters.Where(x => x.StateName.ToLower() == StateName.ToLower()).FirstOrDefault()?.Id ?? 0,
                                CityMaster_Id = string.IsNullOrEmpty(CityName) ? 0 : ent.CityMasters.Where(x => x.CityName.ToLower() == CityName.ToLower()).FirstOrDefault()?.Id ?? 0,
                                FullAddress = row.Cell(10).GetValue<string>().ToString(),
                                PAN = row.Cell(11).GetValue<string>().ToString(),
                                CIN = row.Cell(12).GetValue<string>().ToString(),
                                IsActive =true,
                                CreateDate =DateTime.Now,
                            };
                            vendors.Add(vendor);
                        }
                        if (vendors.Any())
                        {
                            ent.Vendors.AddRange(vendors);
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