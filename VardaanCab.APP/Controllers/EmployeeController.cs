using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Forms;
using Vardaan.Services.IContractApi;
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
        private readonly IEmployee _employee;
        public EmployeeController(IEmployee employee)
        {
            _employee = employee;
        }

        [HttpGet]
        [Route("GetEmployeeProfile")]
        public async Task<IHttpActionResult> GetEmployeeProfile(int id)
        {
            try
            {
                var response = new Response<EmployeeProfileDTO>();
                var data= await _employee.GetProfileDetail(id);
                
                if (data != null)
                {
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
        public async Task<IHttpActionResult> AddEmployeeHelp(HelpEmployee model)
        {
            try
            {
                bool isCreated= await _employee.Addhelp(model);
                if (isCreated)
                {
                    return Ok(new { StatusCode = 200, Succeed = true, Message = "Added successfully." });
                }
                else
                {
                    return BadRequest("Failed.");
                }                
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
        [HttpGet]
        [Route("UpCommingCabByEmpId")]
        public async Task<IHttpActionResult> UpCommingCabByEmpId(string employeeId)
        {
            try
            {
                var response = new Response<EmployeeBookingDTO>();
                var data = await _employee.GetCabUpcommingListByEmployeeId(employeeId);

                return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = data });
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("LiveCabByEmpId")]
        public async Task<IHttpActionResult> LiveCabByEmpId(string employeeId)
        {
            try
            {
                var response = new Response<LiveCabs>();
                var data = await _employee.GetLiveCabByEmployeeId(employeeId);

                return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = data });
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("FinishCabBookingHistoryByEmpId")]
        public async Task<IHttpActionResult> FinishCabBookingHistoryByEmpId(string employeeId)
        {
            try
            {
                var response = new Response<FinishCabs>();
                var data = await _employee.GetFinishCabBookingHisByEmployeeId(employeeId);

                return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = data });

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
