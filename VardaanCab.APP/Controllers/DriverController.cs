using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;
using AutoMapper;

namespace VardaanCab.APP.Controllers
{
    public class DriverController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        [HttpGet]
        [Route("GetDriverProfile")]
        public IHttpActionResult GetDriverProfile(int id)
        {
            var driver = ent.Drivers.SingleOrDefault(d => d.Id == id && d.IsActive);

            if (driver == null)
            {
                return NotFound();
            }
            var model = Mapper.Map<DriverProfileDTO>(driver); 

            return Ok(new {Status=200,Message="Driver profile retrieve successfully."});
        }

    }
}
