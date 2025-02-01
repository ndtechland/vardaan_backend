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
                var data = await _admin.GetAvailableDrivers(Transpostcode, VendorId);
                if (data != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Available drivers retrieved successfully.", Data = data });
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
    }
}
