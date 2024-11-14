using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class SaleDetailModel
    {
        public IEnumerable<CorporateBillModel> Data { get; set; }
        //public IEnumerable<CorporateBillReportModel> Data { get; set; }
        public int MenuId { get; set; }
        public string NRDType { get; set; }
    }
    public class SaleDetailReportModel
    {
        //public IEnumerable<CorporateBillModel> Data { get; set; }
        public IEnumerable<CorporateBillReportModel> Data { get; set; }
        public int MenuId { get; set; }
        public string NRDType { get; set; }
        public string CompanyName { get; set; }
        public string  sMonth { get; set; }
    }
    public class VendorReportDetailModel
    {
        public IEnumerable<VendorBillModel> Data { get; set; }
        public int MenuId { get; set; }
        public string VendorName { get; set; }
        public string sMonth{ get; set; }
    }

    public class MonthlyBookingDetail
    {
        public IEnumerable<CorporateBillModel> Data { get; set; }
        public int MenuId { get; set; }
        public DateTime ? SDate { get; set; }
        public DateTime ? EDate { get; set; }
        public int CompanyId { get; set; }
        public string Type { get; set; }
        public string Heading { get; set; }
        public double Total { get; set; }
        public SelectList CompanyList { get; set; }
    }

   
}