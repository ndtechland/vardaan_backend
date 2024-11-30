using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class ETSController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly string efConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;

        // GET: ETS
        public ActionResult CreateRequest(int menuId = 0, int id = 0)
        {
            var model = new CreateRequestDTO();
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            model.TripTypes = new SelectList(ent.TripTypes.Where(x=>x.TripMasterId==1).ToList(), "Id", "TripTypeName");
            model.ShiftTypes = new SelectList(ent.TripMasters.Where(x=>x.Id==1).ToList(), "Id", "TripName");
            model.PickUpshiftTimes = new SelectList(ent.ShiftMasters.Where(x=>x.TripTypeId==1).ToList(), "Id", "ShiftTime");
            model.DropshiftTimes = new SelectList(ent.ShiftMasters.Where(x=>x.TripTypeId==2).ToList(), "Id", "ShiftTime");
            ViewBag.menuId = menuId;
            if (id > 0)
            {
                var data = ent.EmployeeRequests.Where(x => x.Id == id).FirstOrDefault();
                model.Id = data.Id;
                model.RequestType = data.RequestType;
                model.CompanyId = data.CompanyId;
                model.EmployeeId = data.EmployeeId;
                model.Unit = data.Unit;
                model.Department = data.Department;
                model.CostCentre = data.CostCentre;
                model.ExpenseCode = data.ExpenseCode;
                model.RequestorEmpId = data.RequestorEmpId;
                model.RequestorName = data.RequestorName;
                model.RequestorContacts = data.RequestorContacts;
                model.BookingReceivedDate = data.BookingReceivedDate;
                model.RequestTripType = data.RequestTripType;
                model.DestinationRequestMethod = data.DestinationRequestMethod;
                model.LocationType = data.LocationType;
                model.StartRequestDate = data.StartRequestDate;
                model.EndRequestDate = data.EndRequestDate;               
                model.TripType = data.TripType;
                model.ShiftType = data.ShiftType;
                model.SMSTriggeredLocation = data.SMSTriggeredLocation;
                model.PickupShiftTimeId = data.PickupShiftTimeId;
                model.DropShiftTimeId = data.DropShiftTimeId;
                ViewBag.Heading = "Update Employee Request";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.CompanyId = 0;
                model.RequestType = "";
                model.EmployeeId = "";
                model.Unit = "";
                model.Department = "";
                model.CostCentre = "";
                model.ExpenseCode = "";
                model.RequestorEmpId = "";
                model.RequestorName = "";
                model.RequestorContacts = "";
                model.BookingReceivedDate = null;
                model.RequestTripType = null;
                model.DestinationRequestMethod = null;
                model.LocationType = null;
                model.StartRequestDate = null;
                model.EndRequestDate = null;
                model.TripType = null;
                model.ShiftType = null;
                model.SMSTriggeredLocation = null;
                model.PickupShiftTimeId = 0;
                model.DropShiftTimeId = 0;
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Create New Request";
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult CreateRequest(CreateRequestDTO model)
        {
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (model.Id == 0)
                {
                    var EmpReq = new EmployeeRequest()
                    {
                        RequestType = model.RequestType,
                        CompanyId = model.CompanyId,
                        EmployeeId = model.EmployeeId,
                        Unit = model.Unit,
                        Department = model.Department,
                        CostCentre = model.CostCentre,
                        ExpenseCode = model.ExpenseCode,
                        RequestorEmpId = model.RequestorEmpId,
                        RequestorName = model.RequestorName,
                        RequestorContacts = model.RequestorContacts,
                        BookingReceivedDate = model.BookingReceivedDate,
                        RequestTripType = model.RequestTripType,
                        DestinationRequestMethod = model.DestinationRequestMethod,
                        LocationType = model.LocationType,
                        StartRequestDate = model.StartRequestDate,
                        EndRequestDate = model.EndRequestDate,
                        TripType = model.TripType,
                        ShiftType = model.ShiftType,
                        SMSTriggeredLocation = model.SMSTriggeredLocation,
                        PickupShiftTimeId = model.PickupShiftTimeId,
                        DropShiftTimeId = model.DropShiftTimeId,
                        CreatedDate = DateTime.Now
                        
                    };
                    ent.EmployeeRequests.Add(EmpReq);
                }
                else
                {
                    var data = ent.EmployeeRequests.Find(model.Id);
                    data.RequestType = model.RequestType;
                    data.CompanyId = model.CompanyId;
                    data.EmployeeId = model.EmployeeId;
                    data.Unit = model.Unit;
                    data.Department = model.Department;
                    data.CostCentre = model.CostCentre;
                    data.ExpenseCode = model.ExpenseCode;
                    data.RequestorEmpId = model.RequestorEmpId;
                    data.RequestorName = model.RequestorName;
                    data.RequestorContacts = model.RequestorContacts;
                    data.BookingReceivedDate = model.BookingReceivedDate;
                    data.RequestTripType = model.RequestTripType;
                    data.DestinationRequestMethod = model.DestinationRequestMethod;
                    data.LocationType = model.LocationType;
                    data.StartRequestDate = model.StartRequestDate;
                    data.EndRequestDate = model.EndRequestDate;
                    data.TripType = model.TripType;
                    data.ShiftType = model.ShiftType;
                    data.SMSTriggeredLocation = model.SMSTriggeredLocation;
                    data.PickupShiftTimeId = model.PickupShiftTimeId;
                    data.DropShiftTimeId = model.DropShiftTimeId;

                }
                ent.SaveChanges();
                TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";


            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("CreateRequest", new { menuId = model.MenuId });
        }
        public JsonResult GetEmployeeDetails(string id)
        {
            var employee = ent.Employees.FirstOrDefault(e => e.Employee_Id == id);

            if (employee != null)
            {
                var employeeData = new
                {
                    firstName = employee.Employee_First_Name,
                    lastName = employee.Employee_Last_Name, 
                    gender = employee.Gender,
                    email = employee.Email,
                    guestContactNumber = employee.MobileNumber
                };

                return Json(employeeData,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null);
            }
        }

        public ActionResult EmployeeRequestList(int menuId = 0)
        {
            try
            {
                var model = new CreateRequestDTO();
                var data = (from er in ent.EmployeeRequests
                            join e in ent.Employees on er.EmployeeId equals e.Employee_Id
                            join c in ent.Customers on er.CompanyId equals c.Id
                            join tt in ent.TripTypes on er.TripType equals tt.Id
                            join st in ent.TripMasters on er.ShiftType equals st.Id
                            orderby er.Id descending
                            select new EmployeeRequests
                            {
                                Id = er.Id,
                                CompanyName = c.CustomerName,
                                RequestType = er.RequestType,
                                EmployeeId = er.EmployeeId,
                                FirstName = e.Employee_First_Name,
                                LastName = e.Employee_Last_Name,
                                Gender = e.Gender,
                                Email = e.Email,
                                GuestContact = e.MobileNumber,
                                Unit = er.Unit,
                                Department = er.Department,
                                CostCentre = er.CostCentre,
                                ExpenseCode = er.ExpenseCode,
                                RequestorEmpId = er.RequestorEmpId,
                                RequestorName = er.RequestorName,
                                RequestorContacts = er.RequestorContacts,
                                BookingReceivedDate = er.BookingReceivedDate,
                                RequestTripType = er.RequestTripType,
                                DestinationRequestMethod = er.DestinationRequestMethod,
                                LocationType = er.LocationType,
                                StartRequestDate = er.StartRequestDate,
                                EndRequestDate = er.EndRequestDate,
                                TripType = tt.TripTypeName,
                                ShiftType = st.TripName,
                                SMSTriggeredLocation = er.SMSTriggeredLocation,
                                CreatedDate = er.CreatedDate
                            }
                            ).ToList();

                ViewBag.menuId = menuId;
                model.EmployeeRequestList = data;
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult DeleteEmployeeRequest(int id)
        {
            try
            {
                var dt = ent.EmployeeRequests.Find(id);
                ent.EmployeeRequests.Remove(dt);
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("EmployeeRequestList");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ExportToExcel()
        {
            DataTable dt = GetTableData(); // Get data from the database

            var columnsToRemove = new List<string> { "Id", "RequestType", "CompanyId", "FirstName", "LastName", "Gender", "Email", "GuestContact" };

            foreach (string columnName in columnsToRemove)
            {
                if (dt.Columns.Contains(columnName))
                {
                    dt.Columns.Remove(columnName);
                }
            }

            // Create a new DataTable with only the column names
            DataTable dtWithColumnNames = new DataTable();

            // Add columns to the new DataTable
            foreach (DataColumn column in dt.Columns)
            {
                dtWithColumnNames.Columns.Add(column.ColumnName);
            }

            // Add a single row with column names as values
            DataRow headerRow = dtWithColumnNames.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                headerRow[i] = dt.Columns[i].ColumnName;
            }
            dtWithColumnNames.Rows.Add(headerRow);

            // Export to Excel
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dtWithColumnNames, "EmployeeRequest");
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeRequestData.xlsx");
                }
            }
        }

        private DataTable GetTableData()
        {
            var entityBuilder = new EntityConnectionStringBuilder(efConnectionString);
            string sqlConnectionString = entityBuilder.ProviderConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeRequest", con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }
    }
}