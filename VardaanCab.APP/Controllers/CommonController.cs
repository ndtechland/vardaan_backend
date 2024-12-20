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
using Vardaan.Services.IContractApi;
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
        public async Task<IHttpActionResult> GetStates()
        {
            try
            {
                var response = new Response<StateMaster>();
                List<StateMaster> states = await _common.GetStates();
                if (states.Count() > 0) {
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
        public async Task<IHttpActionResult> GetCityByStateId(int StateId)
        {
            try
            {
                var response = new Response<CityMaster>();
                var city = await _common.GetCity(StateId);
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
        public async Task<IHttpActionResult> GetSupport()
        {
            try
            {
                var response = new Response<Support>();
                var states = await _common.GetSupport();
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
        public async Task<IHttpActionResult> GetTripTypes()
        {
            try
            {
                var response = new Response<TripType>();
                var triptypes = await _common.GetTriptype();
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
        public async Task<IHttpActionResult> GetPickupShiftTime()
        {
            try
            {
                var response = new Response<ShiftMaster>();
                var shifttime = await _common.GetPickupShifttimes();
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
        public async Task< IHttpActionResult> GetDropShiftTime()
        {
            try
            {
                var response = new Response<ShiftMaster>();
                var shifttime = await _common.GetDropShifttimes();
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
        public async Task<IHttpActionResult> GetShiftType()
        {
            try
            {
                var response = new Response<TripMaster>();
                var Shifttypes = await _common.GetShiftTypes();
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
        public async Task<IHttpActionResult> UpdateEmployeeAndDriverDeviceId(UpdateDeviceDTO model)
        {
            try
            {
                var response = new Response<UpdateDeviceDTO>();
                if (model.RoleName != "" && model.Id>0)
                {
                    bool isUpdated = await _common.GetUpdateEmployeeAndDriverDeviceId(model);
                    if (isUpdated)
                    {
                        response.Succeeded = true;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Message = $"{model.RoleName} device id updated successfully.";
                        return Content(HttpStatusCode.OK, response);
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = $"{model.RoleName} not found.";
                        return Content(HttpStatusCode.OK, response);
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
