using AutoMapper;
using System;
using Rotativa;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Models.ViewModels;
using VardaanCab.Repository;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    [AllowAnonymous]
    public class MyController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository cr = new CommonRepository();
        StateMasterGstinRepository stateWiseGstinRepo = new StateMasterGstinRepository();
        //public ActionResult BookingConfirmationInvoice()
        //{
        //    return View();
        //}

        //public ActionResult tn()
        //{
        //    DateTime sDate = DateTime.Now;
        //    DateTime eDate = DateTime.Now.AddDays(3);
        //    TimeSpan sTime = new TimeSpan(19,0,0);
        //    TimeSpan eTime = new TimeSpan(1, 0, 0);
        //    TimeSpan sNightTime = new TimeSpan(22, 0, 0);
        //    TimeSpan eNightTime = new TimeSpan(6, 0, 0);
        //    return View();
        //}

        public class M
        {
            public string[] Subjects { get; set; }
        }

        [AllowAnonymous]
        public ActionResult ck()
        {
            return View();
        }

      
        [AllowAnonymous]
        public ActionResult GeneratePdf()
        {
           
                var dt = DateTime.Now;
                string filename = "" + dt.Month + dt.Day + dt.Hour + dt.Year + dt.Minute + dt.Second + dt.Millisecond + Guid.NewGuid().ToString() + ".pdf";
                string path = Server.MapPath("/Files/") + filename;
                return new ActionAsPdf("ck") { FileName = filename, SaveOnServerPath = path };
        }

        [AllowAnonymous]
        public ActionResult Today()
        {
            return View();
        }


    }
}