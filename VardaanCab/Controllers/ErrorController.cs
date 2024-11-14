using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Home
        public ActionResult NotFound(string aspxerrorpath)
        {
            return View();
        }

        public ActionResult Error(string aspxerrorpath)
        {
            ViewBag.path = aspxerrorpath;
            return View();
        }
        public ActionResult testingsendmail()
        {
            @ViewBag.Message = "Mail Test";
                return View();
        }
        [HttpPost]
        public ActionResult testingsendmail(string from)
        {
            string msg = $@"Dear Sir / Madam,<br/><br/>
                    Greetings from VARDAAN Car Rental Services !!!! <br/><br/>";
            string errmsg= EmailOperation.SendEmailStr("bhupeshbisht15@gmail.com", "Vardaan Car Rental Booking Confirmation", msg, true, "");
            @ViewBag.Message ="Error--"+ errmsg;
            return View();
        }
    }
}