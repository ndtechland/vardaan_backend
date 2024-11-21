using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Employee")]
    public class EmployeeController : ApiController
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();

        [HttpGet]
        [Route("GetEmployeeProfile")]
        public IHttpActionResult GetEmployeeProfile(int id)
        {
            try
            {
                var response = new Response<EmployeeProfileDTO>();
                var empdata=ent.Employees.SingleOrDefault(x => x.Id == id && x.IsActive==true);
                
                if (empdata != null)
                {
                    var getstate = ent.StateMasters.Where(s => s.Id == empdata.StateId).FirstOrDefault();
                    var getcity = ent.CityMasters.Where(s => s.Id == empdata.CityId).FirstOrDefault();

                    var data = new EmployeeProfileDTO()
                    {
                        Id=empdata.Id,
                        Employee_First_Name=empdata.Employee_First_Name,
                        Employee_Middle_Name=empdata.Employee_Middle_Name,
                        Employee_Last_Name=empdata.Employee_Last_Name,
                        Email=empdata.Email,
                        MobileNumber=empdata.MobileNumber,
                        Employee_Id=empdata.Employee_Id,
                        EmployeeDepartment=empdata.EmployeeDepartment,
                        StateId=empdata.StateId,
                        CityId=empdata.CityId,
                        StateName= getstate.StateName,
                        CityName= getcity.CityName,
                        Pincode=empdata.Pincode,
                        CreatedDate=empdata.CreatedDate,
                    };
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Data = data;
                    response.Message = "Employee profile retrieved successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Employee not found.";
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
