using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        // GET: ETS
        public ActionResult CreateRequest(int menuId = 0, int id = 0)
        {
            var model = new CreateRequestDTO();
            model.Companies = new SelectList(ent.Customers.ToList(), "Id", "CustomerName");
            ViewBag.menuId = menuId;
            return View(model);
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
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        Email = model.Email,
                        GuestContact = model.GuestContact,
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
                    data.FirstName = model.FirstName;
                    data.LastName = model.LastName;
                    data.Gender = model.Gender;
                    data.Email = model.Email;
                    data.GuestContact = model.GuestContact;
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

    }
}