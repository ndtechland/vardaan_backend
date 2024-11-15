using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Domain.ViewModels
{
    public class CIBillingListVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public IEnumerable<CIData> Bills { get; set; }
        public string BookingId { get; set; }
        public string InvoiceNo { get; set; }
        public string CompanyName { get; set; }
        public DateTime ? sDate { get; set; }
        public DateTime ? eDate { get; set; }
        public bool IsNrg { get; set; }
        public string Mobile { get; set; }
        public string CabNo { get; set; }
        public int SrNo { get; set; }
        public string Display { get; set; }
        public SelectList Cities { get; set; }
        public bool Export { get; set; }
        public int cityid { get; set; }
        public int routeno { get; set; }
    }


    public class CIBillingListVmEdit
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public IEnumerable<CIDataEdit> Bills { get; set; }
        public string BookingId { get; set; }
        public string InvoiceNo { get; set; }
        public string CompanyName { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public bool IsNrg { get; set; }
        public string Mobile { get; set; }
        public string CabNo { get; set; }
        public int SrNo { get; set; }
        public string Display { get; set; }
        public bool Export { get; set; }
        public int taxInvID { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public string TaxInvoiceDate { get; set; }
    }
}