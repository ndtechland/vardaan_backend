using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Booking")]
    public class BookingController : ApiController
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities(); 

        [HttpPost,Route("CreateRequest")]         
        public IHttpActionResult CreateRequest(EmployeeRequest model)
        {
            try
            {
                var response = new Response<EmployeeRequest>();
                var empinfo = ent.Employees.Where(e => e.Employee_Id == model.EmployeeId).FirstOrDefault();
                if(empinfo!=null)
                {
                    var data = new EmployeeRequest()
                    {
                        EmployeeId = model.EmployeeId,
                        RequestType = "EMPLOYEE",
                        CompanyId = model.CompanyId,
                        ShiftType = model.ShiftType,
                        TripType = model.TripType,
                        StartRequestDate = model.StartRequestDate,
                        EndRequestDate = model.EndRequestDate,
                        PickupShiftTimeId = model.PickupShiftTimeId,
                        DropShiftTimeId = model.DropShiftTimeId,
                        CreatedDate = DateTime.Now
                    };
                    ent.EmployeeRequests.Add(data);
                    ent.SaveChanges();
                    return Ok(new { Succeeded=true, StatusCode = 200, Message = "Request created successfully." });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Employee not found. Please ensure the Employee ID is correct and try again.";
                    return Content(HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
