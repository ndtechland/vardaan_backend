using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    }
}
