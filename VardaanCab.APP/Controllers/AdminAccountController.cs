using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Vardaan.Services.IContractApi;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/AdminAccount")]
    public class AdminAccountController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly IAdminAccount _adminaccount;
        public AdminAccountController(IAdminAccount admin)
        {
            _adminaccount = admin;
        }
        [HttpPost]
        [Route("AdminLogin")]
        public async Task<IHttpActionResult> AdminLogin(AdminLoginDTO model)
        {
            try
            {
                var response = new Response<bool>();
                var compinfo = ent.Customers.Where(c => c.OrgName == model.TransportCode).FirstOrDefault();
                if (compinfo != null)
                {
                    var data = await _adminaccount.AdminLogin(model);
                    if (data != null)
                    {
                        return Ok(new { Succeeded = true, StatusCode = 200, Message = "Logged In successfully.", Data = data });
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "No access assigned or mobile number does not match.";
                        return Content(HttpStatusCode.NotFound, response);
                    }
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Please provide valid transport code.";
                    return Content(HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("AdminLoginWithMobPass")]
        public async Task<IHttpActionResult> AdminLoginWithMobPass(AdminLoginDTO model)
        {
            try
            {
                var response = new Response<bool>();
                var compinfo = ent.Customers.Where(c => c.OrgName == model.TransportCode).FirstOrDefault();
                var empinfo = ent.Employees.Where(x => x.MobileNumber == model.MobileNumber).FirstOrDefault();
                if(empinfo==null||empinfo.Password!=model.Password)
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Invalid mobile number or password.";
                    return Content(HttpStatusCode.NotFound, response);
                }
                if (compinfo != null)
                {
                    var data = await _adminaccount.AdminLoginWithMobandPass(model);
                    if (data != null)
                    {
                        return Ok(new { Succeeded = true, StatusCode = 200, Message = "Logged In successfully.", Data = data });
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "No access assigned.";
                        return Content(HttpStatusCode.NotFound, response);
                    }
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Please provide valid transport code.";
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
