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
using VardaanCab.Utilities;
using static VardaanCab.Utilities.EmailOperation;
namespace VardaanCab.APP.Controllers
{
    public class AccountController : ApiController
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly Random _random = new Random();
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
                var EmployeeResult = ent.Employees.Where(x => x.OTP == model.OTP || x.Password == model.Password  && x.MobileNumber == model.UserName && x.IsActive == true).FirstOrDefault();
                var DriverResult = ent.Drivers.Where(x => x.OTP == model.OTP || x.Password == model.Password && x.MobileNumber == model.UserName && x.IsActive == true).FirstOrDefault();
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
                        else
                        {
                            return BadRequest("Invalid OTP.");
                        }
                    }
                    else 
                    {
                        if (EmployeeResult.Password == model.Password)
                        {
                            return Ok(new { Status = HttpStatusCode.OK, Message = "Login Successfuly...!",Role= "Employee", Data = EmployeeResult });

                        }
                        else
                        {
                            return BadRequest("Invalid password.");
                        }
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
                            return Ok(new { Status = HttpStatusCode.OK,Message = "Login Successfuly...!", Role = "Driver", Data = DriverResult });
                        }
                        else
                        {
                            return BadRequest("Invalid OTP.");
                        }
                    }
                    else 
                    {
                        if (DriverResult.Password == model.Password)
                        {
                            return Ok(new { Status = HttpStatusCode.OK, Message = "Login Successfuly...!", Data = DriverResult });

                        }
                        else
                        {
                            return BadRequest("Invalid password.");
                        }
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
        [Route("api/Account/EmployeeForgotPassword")]
        public IHttpActionResult EmployeeForgotPassword(ForgetPasswordDTO model)
        {
            var emp = ent.Employees.Where(a => a.Email == model.EmailId).FirstOrDefault();

            if (emp == null)
            {
                return NotFound();  
            } 
            var otp = GenerateRandomOtp(); 


            if (emp == null)
            {
                return NotFound();
            }
            emp.Password = otp;
            ent.SaveChanges();

            EmailEF ef = new EmailEF()
            {
                Email = model.EmailId,
                Subject = "Change password",

                Message = @"<!DOCTYPE html>
<html>
<head>
    <title>Change password request.</title>
</head>
<body> 
    
    <ul>
        <li><strong>Your new password is:</strong> " + otp + @"</li> 
    </ul>
    <p>You can now login with your new password.</p>
</body>
</html>"
            };

            EmailOperation.SendEmail(ef);

            return Ok(new {Status=200,Message= "Check your email for your new password. You can now log in with it" });
        }
        private string GenerateRandomOtp()
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        [Route("api/Account/EmployeeChangePassword")]
        public IHttpActionResult EmployeeChangePassword(ChangePasswordDTO model)
        {
            try
            {
                var emp = ent.Employees.Find(model.Id);

                if (emp != null)
                {
                    if (emp.Password == model.OldPassword)
                    {
                        if (model.Password == model.ConfirmPassword)
                        {
                            if (emp.Password == model.Password)
                            { 
                                return BadRequest("New password cannot be the same as the old password.");
                            }

                            emp.Password = model.Password;
                            ent.SaveChanges();
                            return Ok(new {Status=200,Message= "Password has been updated successfully." });
                        }
                        else
                        {
                            return BadRequest("Confirm Password not matched.");
                        }
                    }
                    else
                    {
                        return BadRequest("Old password is incorrect.");
                    }
                }
                else
                {
                    return BadRequest("Invalid User Id");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
