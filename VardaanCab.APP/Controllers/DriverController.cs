using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;
using static System.Net.WebRequestMethods;
using static VardaanCab.APP.Utilities.EmailOperation;


namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Driver")]
    public class DriverController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        [HttpGet]
        [Route("GetDriverProfile")]
        public IHttpActionResult GetDriverProfile(int id)
        {
            var response = new Response<DriverProfileDTO>();
            try
            {
                var driver = ent.Drivers.SingleOrDefault(d => d.Id == id && d.IsActive);

                if (driver == null)
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
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

        [HttpPut]
        [Route("UpdateDriverProfile")]
        public IHttpActionResult UpdateDriverProfile(DriverProfileDTO model)
        {
            try
            {
                var checkdriver = ent.Drivers.Find(model.Id);
                if(checkdriver != null)
                {
                    checkdriver.DriverName = model.DriverName;
                    checkdriver.Email = model.Email;
                    checkdriver.DriverAddress = model.Address;
                    checkdriver.AlternateNo1 = model.AlternateNo1;
                    ent.SaveChanges();
                    return Ok(new { Status = 200, Message = "Profile updated successfully." });
                }
                else
                {
                    return BadRequest("Driver profile not found.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        [Route("UpdateDriverProfilePic")]
        public IHttpActionResult UpdateDriverProfilePic(DriverProfileDTO model)
        {
            try
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var data = ent.Drivers.Find(model.Id);

                if (data != null)
                {
                    //var imagePath = @"\VardaanCab\Images";
                   // var imagePath = @"/admin.vardaan.ndinfotech.com/images"; 
                    string imagePath = ConfigurationManager.AppSettings["ImagePath"];
                    var profilepic = FileOperation.UploadFileWithBase64(imagePath, model.DriverImage, model.DriverImageBase64, allowedExtensions);

                    if (profilepic == "not allowed")
                    {
                        return BadRequest("Only png, jpg, jpeg files are allowed as Profile picture.");
                    }
                    model.DriverImage = profilepic;

                    data.DriverImage = model.DriverImage;
                    ent.SaveChanges();
                    return Ok(new { Status = 200, Message = "Profile picture updated successfully." });
                }
                else
                {
                    return BadRequest("Driver profile not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error occurred. Please check the details.", ex);
                //return BadRequest($"error.{ex}");
            }
        }


    }
}
