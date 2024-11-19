using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using VardaanCab.APP.Utilities;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;
namespace VardaanCab.APP.Controllers
{
    public class AccountController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        [HttpPost]
        [Route("api/Account/LoginMaster")]
        public IHttpActionResult LoginMaster(MasterLoginDTO model)
        {
            try
            {
                var EmployeeResult = ent.Employees.Where(x => x.MobileNumber == model.UserName && x.IsActive == true).FirstOrDefault();
                var DriverResult = ent.Drivers.Where(x => x.MobileNumber == model.UserName && x.IsActive == true).FirstOrDefault();
                if(EmployeeResult != null)
                {
                    if(EmployeeResult.IsActive==false)
                    {
                        return BadRequest("Employee is not active.");
                    }
                    if(EmployeeResult.IsFirst == true)
                    {

                        Random ran = new Random();
                        int OTPNumber = ran.Next(1000, 9999);
                        string msg = "Hi " + EmployeeResult.Employee_First_Name + EmployeeResult.Employee_Middle_Name + EmployeeResult.Employee_Last_Name + ",\n Welcome to the Vardaan Employee Login. \n OTP :" + OTPNumber + "";
                        SmsOperation.SendSms(DriverResult.MobileNumber, msg);
                        DriverResult.OTP = OTPNumber;
                        ent.SaveChangesAsync();
                        return Ok(new { Status = 200, Message = "Otp Send SuccessFully...!", Data = EmployeeResult.IsFirst });
                    }
                    else
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the Password...!", Data = EmployeeResult.IsFirst });
                     
                    }
                }
                else if(DriverResult != null)
                {
                    if (DriverResult.IsActive == false)
                    {
                        return BadRequest("Driver is not active.");
                    }
                    if (DriverResult.IsFirst == true)
                    {
                        Random ran = new Random();
                        int OTPNumber = ran.Next(1000, 9999);
                        string msg = "Hi " + DriverResult.DriverName + ",\n Welcome to the Vardaan Driver Login. \n OTP :" + OTPNumber + "";
                        SmsOperation.SendSms(DriverResult.MobileNumber, msg);
                        DriverResult.OTP = OTPNumber;
                        ent.SaveChangesAsync();
                        return Ok(new { Status = 200, Message = "Otp Send SuccessFully...!", Data = DriverResult.IsFirst });
                    }
                    else
                    {
                        return Ok(new { Status = 200, Message = "Please Enter the Password...!", Data = DriverResult.IsFirst });
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

        [HttpPost]
        [Route("api/Account/VerifieyOtpOrPassword")]
        public IHttpActionResult VerifieyOtpOrPassword(MasterLoginDTO model)
        {
            try
            {
                var EmployeeResult = ent.Employees.Where(x => x.OTP == model.OTP && x.IsActive == true).FirstOrDefault();
                var DriverResult = ent.Drivers.Where(x => x.OTP == model.OTP && x.IsActive == true).FirstOrDefault();
                if (EmployeeResult != null)
                {
                    if (EmployeeResult.IsFirst == true)
                    {

                        if (EmployeeResult.OTP == model.OTP)
                        {
                            EmployeeResult.OTP = 0;
                            EmployeeResult.IsFirst = false;
                            return Ok(new { Status = HttpStatusCode.OK, Message = "Login Successfuly...!", Data = EmployeeResult });
                        }
                    }
                    else if (EmployeeResult.Password == model.Password)
                    {
                        return Ok(new { Status = HttpStatusCode.OK, Message = "Login Successfuly...!", Data = EmployeeResult });
                    }
                }
                else if (DriverResult != null)
                {  
                    if (DriverResult.IsFirst == true)
                    {
                        if(DriverResult.OTP == model.OTP)
                        {
                            DriverResult.OTP = 0;
                            DriverResult.IsFirst = false;
                            ent.SaveChanges();
                            return Ok(new { Status = HttpStatusCode.OK,Message = "Login Successfuly...!", Data = DriverResult });
                        }
                    }
                    else if(DriverResult.Password == model.Password)
                    {
                        return Ok(new { Status = HttpStatusCode.OK, Message = "Login Successfuly...!", Data = DriverResult });
                    }
                }
                else
                {
                    return Ok(new { Status = HttpStatusCode.NonAuthoritativeInformation, Message = "You Are not a Authrized Person...!" });
                }
                return Ok();
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
    }
}
