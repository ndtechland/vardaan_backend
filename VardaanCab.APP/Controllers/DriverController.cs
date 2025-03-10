using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Vardaan.Services.IContract;
using Vardaan.Services.IContractApi;
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
        private readonly IDriver _driver;
        public DriverController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet]
        [Route("GetDriverProfile")]
        public async Task<IHttpActionResult> GetDriverProfile(int id)
        {
            var response = new Response<DriverProfileDTO>();
            try
            {
                var data = await _driver.GetProfile(id);
                if (data != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Driver profile retrieved successfully.", Role = "Driver", Data = data });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Profile detail not found.";
                    return Content(HttpStatusCode.NotFound, response);
                }

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
        public async Task<IHttpActionResult> UpdateDriverProfile(DriverProfileDTO model)
        {
            try
            {
                bool isUpdated = await _driver.UpdateProfile(model);
                if(isUpdated)
                {
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

        [HttpPut]
        [Route("UpdateDriverActiveStatus")]
        public async Task<IHttpActionResult> UpdateDriverActiveStatus(DriverDTO model)
        {
            try
            {
                bool isUpdated= await _driver.UpdateActiveStatus(model);
                if (isUpdated)
                {
                    if (model.IsOnline == true)
                    {
                        return Ok(new { StatusCode = 200, Message = "You are online now." });
                    }
                    else
                    {
                        return Ok(new { StatusCode = 200, Message = "You are offline now." });
                    }
                }
                else
                {
                    return BadRequest("Driver not found.");
                }
                             
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Internal server error."));
            }
        }
        [HttpGet]
        [Route("TrackCabEmployeesListByDriverId")]
        public async Task<IHttpActionResult> TrackCabEmployeePickupListByDriverId(long driverId)
        {
            try
            {
                var response = new Response<TrackCabEmployeePickupViewModel>();
                var data = await _driver.GetTrackCabEmployee(driverId);

                if (data.Count() > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = data });
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
        [HttpGet]
        [Route("TrackEmployeeLocationByDriverIdAndId")]
        public async Task<IHttpActionResult> TrackEmployeeLocationByDriverIdAndId(long driverId,int Id)
        {
            try
            {
                var response = new Response<TrackCabEmployeePickupViewModel>();
                var data = await _driver.GetTrackEmployeeLocationIndividually(driverId,Id);

                if (data.Count() > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = data });
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
        [HttpPost]
        [Route("AddCabMeterReadings")]
        public async Task<IHttpActionResult> AddCabMeterReadings(CabReadingDTO model)
        {
            try
            {
                if(!ent.AllRoutes.Any(x=>x.Id==model.RouteId))
                {
                    return BadRequest("Route not exists.");
                }
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                string imagePath = ConfigurationManager.AppSettings["ImagePath"];
                var profilepic = FileOperation.UploadFileWithBase64(imagePath, model.MeterReadingImage, model.MeterReadingImageBase64, allowedExtensions);

                if (profilepic == "not allowed")
                {
                    return BadRequest("Only png, jpg, jpeg files are allowed as Profile picture.");
                }
                model.MeterReadingImage = profilepic;
                bool isCreated = await _driver.AddCabMeterReading(model);
                if (isCreated)
                {
                    return Ok(new { StatusCode = 200, Message = "Added successfully." });
                }
                else
                {
                    return BadRequest("Failed.");
                }

            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Internal server error."));
            }
        }
    }
}
