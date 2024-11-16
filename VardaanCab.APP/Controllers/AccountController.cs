using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;
namespace VardaanCab.APP.Controllers
{
    public class AccountController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        [HttpPost]
        public IHttpActionResult LoginMaster(MasterLoginDTO model)
        {
            try
            {
                var CustomerResult = ent.Customers.Where(x => x.ContactNo == model.UserName && x.IsActive == true).FirstOrDefault();
                var DriverResult = ent.Drivers.Where(x => x.MobileNumber == model.UserName && x.IsActive == true).FirstOrDefault();
                if(CustomerResult.IsActive == true && CustomerResult != null)
                {
                    if("IsOtp" == "True")
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the OTP...!", Data = CustomerResult });
                    }
                    else
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the Password...!", Data = CustomerResult });
                    //dfgfdgfd
                    }
                }
                else if(DriverResult.IsActive == true && DriverResult != null)
                {
                    if ("IsOtp" == "True")
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the OTP...!", Data = CustomerResult });
                    }
                    else
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the Password...!", Data = CustomerResult });
                    }

                }
                else
                {
                    return Ok(new { Status = HttpStatusCode.NonAuthoritativeInformation, Message = "You Are not a Authrized Person...!" });
                } 
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
    }
}
