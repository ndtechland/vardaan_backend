using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Http;
using Vardaan.Services.IContractApi;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;
using static Azure.Core.HttpHeader;

namespace VardaanCab.APP.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private readonly IAdmin _admin;
        Vardaan_AdminEntities ent =new Vardaan_AdminEntities();
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
        public async Task<IHttpActionResult> AvailableDrivers(string Transpostcode, int VendorId)
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
                var (checkindriversList, vendorName,vendor_Id) = await _admin.GetCheckInDrivers(Transpostcode, VendorId);

                if (checkindriversList != null && checkindriversList.Any())
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Check In Drivers retrieved successfully.", VendorName = vendorName, vendor_Id, Data = checkindriversList });

                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "No drivers found.";
                    return Content(HttpStatusCode.OK, response);
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
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Vehicles retrieved successfully.", VendorName = vendorName, Data = vehiclesList });

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
        public async Task<IHttpActionResult> DriverMobNumbers(int VendorId)
        {
            try
            {
                var response = new Response<GetMobileNumbers>();
                var mob = await _admin.GetDriverMobNo(VendorId);

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
        public async Task<IHttpActionResult> VehicleNumbers(int VendorId)
        {
            try
            {
                var response = new Response<VehicleNumbers>();
                var vehicleno = await _admin.GetVehicleNo(VendorId);

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
        [HttpPost]
        [Route("AddRemark")]
        public async Task<IHttpActionResult> AddRemark(DriverCheckoutRemarkModel model)
        {
            try
            {
                var response = new Response<DriverCheckoutRemarkModel>();
                bool isCreated = await _admin.AddDriverCheckoutRemark(model);
                if(isCreated)
                {
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Remark added successfully.";
                    return Content(HttpStatusCode.OK, response);
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Failed to add remark.";
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPut]
        [Route("UpdateCheckinDriverEntity")]
        public async Task<IHttpActionResult> UpdateCheckinDriverEntity(AvailableDriverDTO model)
        {
            try
            {
                var response = new Response<AvailableDriverDTO>();
                var vehicleinfo= ent.Cabs.Where(x=>x.VehicleNumber==model.VehicleNumber).FirstOrDefault();
                if(vehicleinfo==null)
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Invalid vehicle number.";
                    return Content(HttpStatusCode.NotFound, response);
                }
                bool isCreated = await _admin.UpdateCheckinDriver(model);
                if (isCreated)
                {
                    response.Succeeded = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Updated successfully.";
                    return Content(HttpStatusCode.OK, response);
                }
                else
                {
                    response.Succeeded = false;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Failed to update.";
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("CheckInDriverVehicle")]
        public async Task<IHttpActionResult> CheckInDriverVehicle(AvailableDriverDTO model)
        {
            var response = new Response<AvailableDriverDTO>();

            try
            {
                if (string.IsNullOrEmpty(model.VehicleNumber) || model.DriverId <= 0)
                {
                    response.Succeeded = false;
                    response.Status = "Failed";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Failed to check in. Please provide both a valid Vehicle Number and Driver ID.";
                    return Content(HttpStatusCode.BadRequest, response);
                }

                var driverinfo = ent.Drivers.FirstOrDefault(d => d.Id == model.DriverId);
                if (driverinfo == null)
                {
                    response.Succeeded = false;
                    response.Status = "Failed";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Failed to check in. Invalid driver.";
                    return Content(HttpStatusCode.NotFound, response);
                }

                var cab = ent.Cabs.FirstOrDefault(x => x.VehicleNumber == model.VehicleNumber && x.IsActive);
                if (cab == null)
                {
                    response.Succeeded = false;
                    response.Status = "Failed";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Failed to check in. Invalid Vehicle number.";
                    return Content(HttpStatusCode.NotFound, response);
                }

                bool isCheckIn = await _admin.AddCheckinDriverVehicle(model);
                if (isCheckIn)
                {
                    response.Succeeded = true;
                    response.Status = "Success";
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Check in successfully.";
                    return Content(HttpStatusCode.OK, response);
                }
                else
                {
                    response.Succeeded = false;
                    response.Status = "Failed";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Failed to check in Driver & Vehicle.";
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Status = "Failed";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred during login.";
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpPost]
        [Route("VehicleInspection")]
        public async Task<IHttpActionResult> VehicleInspection(VehicleInspectionDTO model)
        {
            var response = new Response<VehicleInspectionDTO>();

            try
            {
                string isCreated = await _admin.AddVehicleInspection(model);
                if (isCreated != null)
                {
                    response.Succeeded = true;
                    response.Status = "Success";
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = isCreated;
                    return Content(HttpStatusCode.OK, response);
                }
                else
                {
                    response.Succeeded = false;
                    response.Status = "Failed";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Failed to add vehicle inspection.";
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Status = "Failed";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred during login.";
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpGet]
        [Route("TripTypeMaster")]
        public async Task<IHttpActionResult> TripTypeMaster()
        {
            try
            {
                var data = await _admin.GetTripTypeMaster();
                return Ok(new { StatusCode = 200, Status = "Success", Message = "Trip type retrieved successfully.", data = data });
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("VehicleNumbersByVendor")]
        public async Task<IHttpActionResult> VehicleNumbersByVendor(int VendorId)
        {
            try
            {
                var response = new Response<VehicleNumbers>();
                var vehicleno = await _admin.GetVehicleNoByVendor(VendorId);

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
        [Route("DriverNameByVehicle")]
        public async Task<IHttpActionResult> DriverNameByVehicle(string VehicleNumber)
        {
            try
            {
                var response = new Response<GetDriverName>();
                var name = await _admin.GetDriverNameByVehicleNumber(VehicleNumber);
                var vehId = ent.Cabs.Where(x => x.VehicleNumber == VehicleNumber).FirstOrDefault()?.Id??0;
                if (name != null)
                {
                    return Ok(new { Succeeded = true, StatusCode = 200, Message = "Retrieved successfully.", Data = name,VehicleNo= VehicleNumber,VehicleId= vehId });

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
        [Route("GetTripTypes")]
        public async Task<IHttpActionResult> GetTripTypes()
        {
            try
            {
                var response = new Response<TripType>();
                var triptypes = await _admin.GetTriptype();
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
    }
}
