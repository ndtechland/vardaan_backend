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
                DateTime currentDate = DateTime.Now.Date;

                DateTime lastDayOfMonth = new DateTime(
                    currentDate.Year,
                    currentDate.Month,
                    DateTime.DaysInMonth(currentDate.Year, currentDate.Month)
                );

                int daysDifference = (lastDayOfMonth - currentDate).Days;
                var response = new Response<EmployeeRequest>();
                var empinfo = ent.Employees.Where(e => e.Employee_Id == model.EmployeeId).FirstOrDefault();
                if(empinfo!=null)
                {
                    var Getcabreqday = ent.EmployeeMobileAppSettings
                        .Where(x => x.CompanyId == empinfo.Company_Id && x.IsActive == true)
                        .Select(x => x.CabRequestDays)
                        .FirstOrDefault() ?? 0;
                    if (Getcabreqday>= daysDifference)
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
                        return Ok(new { Succeeded = true, StatusCode = 200, Message = "Request created successfully." });
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.Message = $"Your booking request exceeds the allowed timeframe. You requested {Getcabreqday} days in advance, but {daysDifference} days remain in the current month.";
                        return Content(HttpStatusCode.BadRequest, response);
                    }
                    
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
