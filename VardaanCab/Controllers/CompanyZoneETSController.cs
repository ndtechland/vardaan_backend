using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.MetaAttributes;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class CompanyZoneETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly ICompanyZone _companyZone;
        public CompanyZoneETSController(ICompanyZone companyZone)
        {
            _companyZone = companyZone;
        }
        // GET: CompanyZoneETS
        public ActionResult AddCompanyZone(int menuId = 0, int id = 0)
        {
            var model=new CompanyZoneDTO();
            model.Companies = new SelectList(ent.Customers.Where(x=>x.IsActive==true).OrderByDescending(x=>x.Id).ToList(), "Id", "OrgName");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.CompanyZones.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.CompanyId = existdata.CompanyId;
                model.CompanyZone = existdata.CompanyZone1;
                model.Zonelatlong = existdata.Zonelatlong;
                ViewBag.Heading = "Update Comapny Zone";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.CompanyZone = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Comapny Zone";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddCompanyZone(CompanyZoneDTO model)
        {
          
            try
            {
                model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if(ent.CompanyZones.Any(c=>c.CompanyId == model.CompanyId && c.CompanyZone1==model.CompanyZone && c.Id != model.Id))
                {
                    TempData["errormsg"] = $"Zone already exist for the company {model.CompanyName}.";

                    return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
                }
                bool isCreated = await _companyZone.AddUpdateZone(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["errormsg"] = "Failed to save the record. Please try again.";
                }

                return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error. Please try again later.";
                return RedirectToAction("AddCompanyZone", new { menuId = model.MenuId });
            }
        }

        public async Task<ActionResult> AllCompanyZones(string term = "", int page = 1, int menuId = 0)
        {
            var model = new CompanyZoneDTO();    
            ViewBag.menuId = menuId;
            model.CompanyZoneLists = await _companyZone.GetZones();
            return View(model);
        }
        public async Task<ActionResult> DeleteCompanyZone(int id)
        {
            try
            {
                bool isDeleted= await _companyZone.DeleteZone(id);               
                TempData["dltmsg"] ="Deleted successfully.";
                return RedirectToAction("AllCompanyZones");

            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult HomeRoute(int menuId = 0, int id = 0)
        {
            var model = new EmployeeHomeRouteDTO();
            model.CompanyZones = new SelectList(ent.CompanyZones.ToList(), "Id", "CompanyZone1");
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");

            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.CompanyZoneHomeRoutes.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.CompanyZoneId = existdata.CompanyZoneId;
                model.Company_Id = existdata.Company_Id;
                model.HomeRouteName = existdata.HomeRouteName;
                ViewBag.CompanyZoneId = existdata.CompanyZoneId;
                ViewBag.Heading = "Update Home Route";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyZoneId = 0;
                ViewBag.CompanyZoneId = 0;
                model.HomeRouteName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Home Route";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> HomeRoute(EmployeeHomeRouteDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (ent.CompanyZoneHomeRoutes.Any(c => c.CompanyZoneId == model.CompanyZoneId && c.HomeRouteName == model.HomeRouteName && c.Id!=model.Id))
                {
                    TempData["errormsg"] = $"Home route already exist for the company zone {model.HomeRouteName}.";

                    return RedirectToAction("HomeRoute", new { menuId = model.MenuId });
                }
                bool isCreated = await _companyZone.AddUpdateHomeRoute(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["errormsg"] = "Failed to save the record. Please try again.";
                }


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("HomeRoute", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> AllHomeRoute(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeHomeRouteDTO();
           
            ViewBag.menuId = menuId;
            model.HomeRouteLists = await _companyZone.GetHomeRoutes();
            return View(model);
        }
        public async Task<ActionResult> DeleteHomeRoute(int id)
        {
            try
            {
                bool isDeleted = await _companyZone.DeleteZone(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("AllHomeRoute");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult EmployeeDestinationArea(int menuId = 0, int id = 0)
        {
            var model = new EmployeeDestinationAreaDTO();
            //model.HomeRoutes = new SelectList(ent.CompanyZoneHomeRoutes.ToList(), "Id", "HomeRouteName");
            model.Companies = new SelectList(ent.Customers.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).ToList(), "Id", "OrgName");

            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var existdata = ent.EmployeeDestinationAreas.Where(x => x.Id == id).FirstOrDefault();
                model.Id = existdata.Id;
                model.Company_Id = existdata.Company_Id;
                model.CompanyZoneId = existdata.CompanyZoneId;
                model.HomeRouteId = existdata.CompanyZoneHomeRouteId;
                model.DestinationAreaName = existdata.DestinationAreaName;
                ViewBag.HomeRouteId = existdata.CompanyZoneHomeRouteId;
                ViewBag.CompanyZoneId = existdata.CompanyZoneId;
                ViewBag.Heading = "Update Destination Area";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.HomeRouteId = 0;
                model.Company_Id = 0;
                model.CompanyZoneId = 0;
                ViewBag.PrimaryFacilityZone = 0;
                ViewBag.HomeRouteName = 0;
                model.DestinationAreaName = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Destination Area";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> EmployeeDestinationArea(EmployeeDestinationAreaDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (ent.EmployeeDestinationAreas.Any(c => c.CompanyZoneHomeRouteId == model.HomeRouteId && c.DestinationAreaName == model.DestinationAreaName && c.Id != model.Id))
                {
                    TempData["errormsg"] = $"Destination Area already exist for the Home route {model.HomeRouteName}.";

                    return RedirectToAction("EmployeeDestinationArea", new { menuId = model.MenuId });
                }
                bool isCreated = await _companyZone.AddUpdateDestinationArea(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0
                        ? "Record has been updated successfully."
                        : "Record has been added successfully.";
                }
                else
                {
                    TempData["errormsg"] = "Failed to save the record. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("EmployeeDestinationArea", new { menuId = model.MenuId });
        }
        public async Task<ActionResult> AllEmployeeDestinationArea(string term = "", int page = 1, int menuId = 0)
        {
            var model = new EmployeeDestinationAreaDTO();

            ViewBag.menuId = menuId;
            model.DestinationAreaLists = await _companyZone.GetDestinationAreas();
            return View(model);
        }
        public async Task<ActionResult>DeleteEmployeeDestinationArea(int id)
        {
            try
            {
                bool isDeleted = await _companyZone.DeleteDestinationArea(id);
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("AllEmployeeDestinationArea");

            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult HomeRouteExportToExcel()
        {
            // Create DataTable and add columns
            DataTable dt = new DataTable();
            dt.Columns.Add("Company");
            dt.Columns.Add("CompanyZone");
            dt.Columns.Add("HomeRouteName");
            dt.Rows.Add("Dummy Company","ABC Zone", "Abc Home Route");
           
            using (var workbook = new XLWorkbook())
            { 
                var worksheet = workbook.Worksheets.Add("Home Route Data");
                worksheet.Cell(1, 1).InsertTable(dt);

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HomeRoute.xlsx");
                }
            }
        }
        [HttpPost]
        public async Task<ActionResult> ImportHomeRouteData(HttpPostedFileBase file)
        {
            try
            {

                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<CompanyZoneHomeRoute> Homeroutes = new List<CompanyZoneHomeRoute>();
                        var count = 0;
                        List<ExcelErrorModel> excelErrorModels = new List<ExcelErrorModel>();

                        foreach (var row in rows)
                        {
                            count++;
                            //ExcelErrorModel excelError = new ExcelErrorModel();
                            var companyName = row.Cell(1).GetValue<string>();
                            //string companyName = splitParts[0].Trim();
                            int companyId = ent.Customers
                            .Where(x => x.OrgName.ToLower() == companyName.ToLower())
                            .FirstOrDefault()?.Id ?? 0;


                            var Zone = row.Cell(2).GetValue<string>() ?? string.Empty;
                            int? zoneId = ent.CompanyZones
                                .Where(c => c.CompanyZone1.ToLower() == Zone.ToLower() && c.CompanyId == companyId)
                                .Select(c => c.Id)
                                .FirstOrDefault();
                            if (zoneId == 0)
                            {
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "Comapny Zone",
                                    AffectedRow = count,
                                    Description = $"Company zone {Zone} does not exist for the company {companyName} in master."
                                });
                            }
                            var HomeRoute = row.Cell(3).GetValue<string>() ?? string.Empty;
                            var HomeRouteId = ent.CompanyZoneHomeRoutes.FirstOrDefault(a => a.CompanyZoneId == zoneId && a.Company_Id == companyId);
                            
                            if (ent.CompanyZoneHomeRoutes.Any(x => x.Company_Id == companyId && x.CompanyZoneId == zoneId && x.HomeRouteName.ToLower() == HomeRoute.ToLower()))
                            {
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "Duplicate Home Route",
                                    AffectedRow = count,
                                    Description = $"Home route {HomeRoute} already exists for company {companyName} and zone {Zone}."
                                });
                            }
                            // Check if the HomeRoute already exists in the list being processed
                            if (Homeroutes.Any(x => x.Company_Id == companyId && x.CompanyZoneId == zoneId && x.HomeRouteName.ToLower() == HomeRoute.ToLower()))
                            {
                                continue; // Skip duplicate entries within the same import
                            }
                            if (excelErrorModels.Any(e => e.AffectedRow == count)) continue;

                            CompanyZoneHomeRoute homeroute = new CompanyZoneHomeRoute
                            {
                                Company_Id = companyId,
                                CompanyZoneId = zoneId,
                                HomeRouteName = row.Cell(3).GetValue<string>() ?? string.Empty,
                                CreatedDate=DateTime.Now
                            };

                            Homeroutes.Add(homeroute);

                        }


                        if (excelErrorModels.Any())
                        {
                            Session["HasErrors"] = true;
                            Session["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                            return RedirectToAction("HomeRoute");
                        }

                        if (Homeroutes.Any())
                        {
                            ent.CompanyZoneHomeRoutes.AddRange(Homeroutes);
                            ent.SaveChanges();
                            Session["HasErrors"] = false;
                            TempData["msg"] = "Data imported successfully!";
                            return RedirectToAction("HomeRoute");
                        }

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
        public ActionResult DestinationAreaExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Company");
            dt.Columns.Add("CompanyZone");
            dt.Columns.Add("HomeRouteName");
            dt.Columns.Add("Destination Area");
            dt.Rows.Add("Dummy Company", "ABC Zone", "Abc Home Route","dummy area");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Destination Area Data");
                worksheet.Cell(1, 1).InsertTable(dt);

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DestinationArea.xlsx");
                }
            }
        }
        [HttpPost]
        public async Task<ActionResult> ImportDestinationAreaData(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var workbook = new XLWorkbook(file.InputStream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed().Skip(1);
                        List<EmployeeDestinationArea> Homeroutes = new List<EmployeeDestinationArea>();
                        var count = 0;
                        List<ExcelErrorModel> excelErrorModels = new List<ExcelErrorModel>();

                        foreach (var row in rows)
                        {
                            count++;
                            var companyName = row.Cell(1).GetValue<string>();
                            int companyId = ent.Customers
                            .Where(x => x.OrgName.ToLower() == companyName.ToLower())
                            .FirstOrDefault()?.Id ?? 0;


                            var Zone = row.Cell(2).GetValue<string>() ?? string.Empty;
                            int? zoneId = ent.CompanyZones
                                .Where(c => c.CompanyZone1.ToLower() == Zone.ToLower() && c.CompanyId == companyId)
                                .Select(c => c.Id)
                                .FirstOrDefault();
                            if (zoneId == 0)
                            {
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "Comapny Zone",
                                    AffectedRow = count,
                                    Description = $"Company zone {Zone} does not exist for the company {companyName} in master."
                                });
                            }
                            var HomeRoute = row.Cell(3).GetValue<string>() ?? string.Empty;
                            var Area = row.Cell(4).GetValue<string>() ?? string.Empty;
                            var HomeRouteId = ent.CompanyZoneHomeRoutes.Where(a => a.CompanyZoneId == zoneId && a.HomeRouteName.ToLower() == HomeRoute.ToLower()).FirstOrDefault()?.Id ?? 0;
                            var AreaID = ent.EmployeeDestinationAreas.FirstOrDefault(a => a.CompanyZoneHomeRouteId == HomeRouteId && a.DestinationAreaName.ToLower() == Area.ToLower());
                            if (HomeRouteId == 0)
                            {
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "Home Route",
                                    AffectedRow = count,
                                    Description = $"Home Route {Zone} does not exist for the company zone {Zone} in master."
                                });
                            }
                            if (ent.EmployeeDestinationAreas.Any(x => x.Company_Id == companyId && x.CompanyZoneHomeRouteId == HomeRouteId && x.DestinationAreaName.ToLower() == Area.ToLower()))
                            {
                                excelErrorModels.Add(new ExcelErrorModel
                                {
                                    ErrorType = "Duplicate Home Route",
                                    AffectedRow = count,
                                    Description = $"Destination area {Area} already exists for company {companyName} and zone {Zone}."
                                });
                            }
                            // Check if the Area already exists in the list being processed
                            if (Homeroutes.Any(x => x.Company_Id == companyId && x.CompanyZoneHomeRouteId == HomeRouteId && x.DestinationAreaName.ToLower() == Area.ToLower()))
                            {
                                continue; // Skip duplicate entries within the same import
                            }
                            if (excelErrorModels.Any(e => e.AffectedRow == count)) continue;

                            EmployeeDestinationArea homeroute = new EmployeeDestinationArea
                            {
                                Company_Id = companyId,
                                CompanyZoneHomeRouteId = HomeRouteId,
                                CompanyZoneId = zoneId,
                                DestinationAreaName = Area,
                                CreatedDate = DateTime.Now
                            };

                            Homeroutes.Add(homeroute);
                        }


                        if (excelErrorModels.Any())
                        {
                            Session["HasErrors"] = true;
                            Session["ExcelErrors"] = Newtonsoft.Json.JsonConvert.SerializeObject(excelErrorModels);
                            return RedirectToAction("EmployeeDestinationArea");
                        }

                        if (Homeroutes.Any())
                        {
                            ent.EmployeeDestinationAreas.AddRange(Homeroutes);
                            ent.SaveChanges();
                            Session["HasErrors"] = false;
                            TempData["msg"] = "Data imported successfully!";
                            return RedirectToAction("EmployeeDestinationArea");
                        }

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