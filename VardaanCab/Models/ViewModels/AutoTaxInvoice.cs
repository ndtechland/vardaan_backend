using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class AutoTaxInvoice
    {
        public string TaxInvoiceNo { get; set; }
        public string PrevTaxInvoiceNo { get; set; }
        public DateTime PrevTaxInvoiceDate { get; set; }
    }

    public class tempUserBookingVM
    {
        public int BookingId { get; set; }
        public string username { get; set; }
       // public string CompanyName { get; set; }
        public string sessionId { get; set; }
       // public DateTime CreatedDate { get; set; }
    }
}