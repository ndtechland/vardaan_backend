using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;


namespace VardaanCab.APP.Controllers
{
    //[Route("api/Controller")]
    public class DriverController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        [HttpGet]
        [Route("api/Driver/GetDriverProfile")]
        public IHttpActionResult GetDriverProfile(int id)
        {
            var response = new Response<DriverProfileDTO>();
            try
            {
                var driver = ent.Drivers.SingleOrDefault(d => d.Id == id && d.IsActive);

                if (driver == null)
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound; // Not Found
                    response.Message = "Driver not found.";
                    return Content(HttpStatusCode.NotFound, response);
                }

                var model = new DriverProfileDTO
                {
                    Id = driver.Id,
                    DriverName = driver.DriverName,
                    Address = driver.DriverAddress,
                    Email = driver.Email,
                    MobileNumber = driver.MobileNumber,
                    DlImage = driver.DlImage,
                    DriverImage = driver.DriverImage,
                    DlNumber = driver.DlNumber,
                    CreateDate = driver.CreateDate,
                    AlternateNo1 = driver.AlternateNo1,
                };

                response.Succeeded = true;
                response.StatusCode = StatusCodes.Status200OK; // OK
                response.Data = model;
                response.Message = "Driver profile retrieved successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                response.Message = "An error occurred while retrieving the driver profile.";
                response.Error = ex.Message; // Optional: Include the exception message for debugging
                return Content(HttpStatusCode.BadRequest, response);
            }
        }

    }
}
