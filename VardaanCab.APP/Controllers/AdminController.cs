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
using VardaanCab.Domain.DTOAPI;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private readonly IAdmin _admin;
        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }
        [HttpGet]
        [Route("AdminProfile")]
        public async Task<IHttpActionResult> AdminProfile(int Id)
        {
            try
            {
                var response = new Response<AdminLoginDTO>();
                var data = await _admin.GetProfile(Id);
                if (data != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Profile retrieved successfully.", Data = data });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
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
