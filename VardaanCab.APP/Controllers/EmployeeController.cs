﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Windows.Forms;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;
using static System.Windows.Forms.AxHost;

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
                        EmergencyContactNumber=empdata.AlternateNumber,
                        Employee_Id=empdata.Employee_Id,
                        EmployeeDepartment=empdata.EmployeeDepartment,
                        StateId=empdata.StateId,
                        CityId=empdata.CityId,
                        StateName= getstate.StateName,
                        CityName= getcity.CityName,
                        Pincode=empdata.Pincode,
                        CreatedDate=empdata.CreatedDate,
                        EmployeeCurrentAddress=empdata.EmployeeCurrentAddress,
                        Gender=empdata.Gender,
                        Company_Id = empdata.Company_Id,
                    };
                    return Ok(new { Succeeded = true,StatusCode = 200, Message = "Employee profile retrieved successfully.", Role = "Employee", Data=data });
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
        [HttpPut]
        [Route("UpdateEmployeeProfile")]
        public IHttpActionResult UpdateEmployeeProfile(EmployeeProfileDTO model)
        {
            try
            {
                var response = new Response<EmployeeProfileDTO>();
                var emp = ent.Employees.Find(model.Id);
                if (emp != null)
                {
                    emp.Employee_First_Name = model.Employee_First_Name;
                    emp.Employee_Middle_Name = model.Employee_Middle_Name;
                    emp.Employee_Last_Name = model.Employee_Last_Name;
                    emp.Email = model.Email;
                    //emp.MobileNumber = model.MobileNumber;
                    emp.EmployeeCurrentAddress = model.EmployeeCurrentAddress;
                    emp.StateId = model.StateId;
                    emp.CityId = model.CityId;
                    emp.Pincode = model.Pincode;
                    emp.AlternateNumber = model.EmergencyContactNumber;
                    ent.SaveChanges();
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Employee profile updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Employee data not found.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("AddEmployeeHelp")]
        public IHttpActionResult AddEmployeeHelp(HelpEmployee model)
        {
            try
            {
                var data = new HelpEmployee()
                {
                    Employee_id=model.Employee_id,
                    PhoneNumber=model.PhoneNumber,
                    Reason=model.Reason,
                    CreatedDate=DateTime.Now,
                    IsActive=true
                };
                ent.HelpEmployees.Add(data);
                ent.SaveChanges();
                return Ok(new {StatusCode=200,Succeed=true,Message="Added successfully."});
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("AddEmployeeFeedback")]
        public IHttpActionResult AddEmployeeFeedback(FeedBackEmployee model)
        {
            try
            {
                var data = new FeedBackEmployee()
                {
                    Employee_Id = model.Employee_Id,
                    Feedback = model.Feedback,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                ent.FeedBackEmployees.Add(data);
                ent.SaveChanges();
                return Ok(new { StatusCode = 200, Succeed = true, Message = "Feedback added successfully." });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
