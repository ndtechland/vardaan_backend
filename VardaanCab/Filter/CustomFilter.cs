using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Filter
{
    public class CustomFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //exceute
            try
            {
                var url = filterContext.HttpContext.Request.Url;
                if (HttpContext.Current.Session["bookinglst"] != null)
                {
                    if (!(url.ToString().Contains("CorporateInvoiceList") || url.ToString().Contains("addBookingId") || url.ToString().Contains("getAutoNPreviousTaxInvoiceNo") || url.ToString().Contains("CreateInvoice")))
                    {
                        
                        HttpContext.Current.Session["bookinglst"] = null;
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}