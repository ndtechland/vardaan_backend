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
        [HttpGet]
        [Route("AvailableDrivers")]
        public async Task<IHttpActionResult> AvailableDrivers(string Transpostcode,int VendorId)
        {
            try
            {
                var response = new Response<AdminLoginDTO>();
                var (availebledriversList, vendorName) = await _admin.GetAvailableDrivers(Transpostcode, VendorId);

                if (availebledriversList != null && availebledriversList.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Available Drivers retrieved successfully.", VendorName = vendorName, Data = availebledriversList });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No drivers found.";
                    return Content(HttpStatusCode.NotFound, response);

                }
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("CheckInDrivers")]
        public async Task<IHttpActionResult> CheckInDrivers(string Transpostcode, int VendorId)
        {
            try
            {
                var response = new Response<AdminLoginDTO>();
                var (checkindriversList, vendorName) = await _admin.GetCheckInDrivers(Transpostcode, VendorId);

                if (checkindriversList != null && checkindriversList.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Check In Drivers retrieved successfully.", VendorName = vendorName, Data = checkindriversList });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No drivers found.";
                    return Content(HttpStatusCode.NotFound, response);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> GetVendorsByTransportCode(string Transpostcode)
        {
            try
            {
                var response = new Response<AdminLoginDTO>();
                var data = await _admin.GetVendorByCompany(Transpostcode);
                if (data != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Vendors retrieved successfully.", Data = data });
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
        [Route("DriverByTransCodeVendorId")]
        public async Task<IHttpActionResult> DriverByTransCodeVendorId(string TransportCode, int VendorId)
        {
            try
            {
                var response = new Response<Drivers>();
                var (driversList, vendorName) = await _admin.GetDriverByTransportCodeVendorId(TransportCode, VendorId);

                if (driversList != null && driversList.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Drivers retrieved successfully.", VendorName = vendorName, Data = driversList });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No drivers found for the provided transport code and vendor ID.";
                    return Content(HttpStatusCode.NotFound, response);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("VehicleByTransCodeVendorId")]
        public async Task<IHttpActionResult> VehicleByTransCodeVendorId(string TransportCode, int VendorId)
        {
            try
            {
                var response = new Response<Vehicles>();
                var (vehiclesList, vendorName) = await _admin.GetVehiclesByTransportCodeVendorId(TransportCode, VendorId);

                if (vehiclesList != null && vehiclesList.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Vehicles retrieved successfully.", VendorName= vendorName, Data = vehiclesList });
                     
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No vehicles found for the provided transport code and vendor ID.";
                    return Content(HttpStatusCode.NotFound, response);
                     
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("DriverMobNumbers")]
        public async Task<IHttpActionResult> DriverMobNumbers()
        {
            try
            {
                var response = new Response<GetMobileNumbers>();
                var mob = await _admin.GetDriverMobNo();

                if (mob != null && mob.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = mob });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No data found.";
                    return Content(HttpStatusCode.NotFound, response);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("DriverByDriverId")]
        public async Task<IHttpActionResult> DriverByDriverId(int Driverid)
        {
            try
            {
                var response = new Response<GetDriverName>();
                var name = await _admin.GetDriverNameByDriverId(Driverid);

                if (name != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = name });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No data found.";
                    return Content(HttpStatusCode.NotFound, response);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("CheckInDriverDetail")]
        public async Task<IHttpActionResult> CheckInDriverDetail(int Id)
        {
            try
            {
                var response = new Response<AvailableDriverDTO>();
                var data = await _admin.GetCheckInDriverDetail(Id);
                if (data != null)
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
        [Route("VehicleNumbers")]
        public async Task<IHttpActionResult> VehicleNumbers()
        {
            try
            {
                var response = new Response<VehicleNumbers>();
                var vehicleno = await _admin.GetVehicleNo();

                if (vehicleno != null && vehicleno.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = vehicleno });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No data found.";
                    return Content(HttpStatusCode.NotFound, response);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("DriverDeviceIds")]
        public async Task<IHttpActionResult> DriverDeviceIds()
        {
            try
            {
                var response = new Response<VehicleNumbers>();
                var DeviceIds = await _admin.GetDeviceId();

                if (DeviceIds != null && DeviceIds.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = DeviceIds });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "No data found.";
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
