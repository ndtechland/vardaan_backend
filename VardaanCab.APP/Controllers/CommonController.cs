using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        [HttpGet]
        [Route("GetBanner")]
        public IHttpActionResult GetBanner(string role)
        {
            var response = new Response<BannerMaster>();
            var banner = ent.BannerMasters.Where(b=>b.Role== role).ToList();
            if(banner!=null)
            {
                return Ok(new { StatusCode = 200, Message = "Banner retrieved successfully", data = banner });
            }
            else
            {
                response.Succeeded = false;
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Banner list not available.";
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
    }
}
