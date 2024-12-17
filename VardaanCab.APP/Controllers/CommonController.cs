using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using Vardaan.Services.IContract;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly ICommon _common;
        public CommonController(ICommon common)
        {
            _common = common;
        }
        [HttpGet]
        [Route("GetBanner/{role}")]
        public async Task<IHttpActionResult> GetBanner(string role)
        {
            var response = new Response<List<BannerMaster>>();
            List<BannerMaster> banner = await _common.GetBanner(role);
            if (banner.Count() > 0)
            {

                response.Succeeded = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Status = "Success";
                response.Message = "Banner retrieved successfully...!";
                response.Data = banner;
                return Ok(response);
            }
            else
            {
                response.Succeeded = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Error = "Failed";
                response.Message = "Banner list not available...!";
                response.Data = banner;
                return Content(HttpStatusCode.NotFound, response);
            }
        }
        [HttpGet]
        [Route("GetStates")]
        public IHttpActionResult GetStates()
        {
            try
            {
                var response = new Response<StateMaster>();
                var states = ent.StateMasters.ToList();
                if (states.Count > 0) {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "States retrieved successfully.", data = states });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "States not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetCityByStateId")]
        public IHttpActionResult GetCityByStateId(int StateId)
        {
            try
            {
                var response = new Response<CityMaster>();
                var city = ent.CityMasters.Where(x=>x.StateMaster_Id==StateId).ToList();
                if (city.Count > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Cities retrieved successfully.", data = city });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Cities not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetSupport")]
        public IHttpActionResult GetSupport()
        {
            try
            {
                var response = new Response<Support>();
                var states = ent.Supports.FirstOrDefault();
                if (states!=null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Support retrieved successfully.", data = states });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Support not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetTripTypes")]
        public IHttpActionResult GetTripTypes()
        {
            try
            {
                var response = new Response<TripType>();
                var triptypes = ent.TripTypes.Where(x=>x.TripMasterId==1).ToList();
                if (triptypes.Count > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Trip types retrieved successfully.", data = triptypes });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Trip types not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetPickupShiftTime")]
        public IHttpActionResult GetPickupShiftTime()
        {
            try
            {
                var response = new Response<ShiftMaster>();
                var shifttime = ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList();
                if (shifttime.Count > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Pickup shift time retrieved successfully.", data = shifttime });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Pickup shift time not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetDropShiftTime")]
        public IHttpActionResult GetDropShiftTime()
        {
            try
            {
                var response = new Response<ShiftMaster>();
                var shifttime = ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList();
                if (shifttime.Count > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Drop shift time retrieved successfully.", data = shifttime });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Drop shift time not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("GetShiftType")]
        public IHttpActionResult GetShiftType()
        {
            try
            {
                var response = new Response<TripMaster>();
                var Shifttypes = ent.TripMasters.Where(x => x.Id == 1).ToList();
                if (Shifttypes.Count > 0)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Shift types retrieved successfully.", data = Shifttypes });
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Shift types not available.";
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        [Route("UpdateEmployeeAndDriverDeviceId")]
        public IHttpActionResult UpdateEmployeeAndDriverDeviceId(UpdateDeviceDTO model)
        {
            try
            {
                var response = new Response<UpdateDeviceDTO>();
                if (model.RoleName != "" && model.Id>0)
                {
                    if(model.RoleName== "Employee")
                    {
                        var data = ent.Employees.Find(model.Id);
                        if (data != null)
                        {
                            data.DeviceId = model.DeviceId;
                            ent.SaveChanges();
                            response.Succeeded = true;
                            response.StatusCode = StatusCodes.Status200OK;
                            response.Message = "Employee device id updated successfully.";
                            return Content(HttpStatusCode.OK, response);
                        }
                        else 
                        {
                            response.Succeeded = false;
                            response.StatusCode = StatusCodes.Status404NotFound;
                            response.Message = "Employee not found.";
                            return Content(HttpStatusCode.NotFound, response);
                        }
                    }
                    else
                    {
                        var data = ent.Drivers.Find(model.Id);
                        if (data != null)
                        {
                            data.DeviceId = model.DeviceId;
                            ent.SaveChanges();
                            response.Succeeded = true;
                            response.StatusCode = StatusCodes.Status200OK;
                            response.Message = "Driver device id updated successfully.";
                            return Content(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            response.Succeeded = false;
                            response.StatusCode = StatusCodes.Status404NotFound;
                            response.Message = "Driver not found.";
                            return Content(HttpStatusCode.NotFound, response);
                        }
                    }
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Your request is incomplete. Please provide the necessary details.";
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
