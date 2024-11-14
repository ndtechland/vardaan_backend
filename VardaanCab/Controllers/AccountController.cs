using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Controllers
{
    public class AccountController : Controller
    {
        DbEntities ent = new DbEntities();
        #region customerLogin
        public ActionResult CustomerLogin()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]
        public ActionResult CustomerLogin(LoginModel model)
        {
            try
            {
                var data = ent.ClientUsers.FirstOrDefault(a => (a.Email == model.Username || a.MobileNumber == model.Username) && a.Password == model.Password && a.IsActive == true);
                if (data == null)
                {
                    TempData["msg"] = "Invalid username or password";
                    return View(model);
                }
                FormsAuthentication.SetAuthCookie(data.Id.ToString(), true);
                string hostName = Dns.GetHostName();
                string ip = Dns.GetHostByName(hostName).AddressList[0].ToString();
                Session["uEmail"] = data.Email;
                Session["CUID"] =data.Id;
                var lh = new LoginHistory
                {
                    UserLogin_Id = data.Id,
                    Ip_Address = ip,
                    UpdateDate = DateTime.Now
                };
                ent.LoginHistories.Add(lh);
                ent.SaveChanges();

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "CabBooking", new { Area = "Clint" });
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error" + "StackTrace-" + ex.Message + ex.StackTrace + "innerExpes-" + ex.InnerException;
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("CustomerLogin", "Account");
        }
        #endregion
    }
}