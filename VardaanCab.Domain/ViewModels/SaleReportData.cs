using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class SaleReportData
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public double Amount { get; set; }
    }

    public class SaleReportVm
    {
        public IEnumerable<SaleReportData> SaleData { get; set; }
        public double Total { get; set; }
        public DateTime  sDate { get; set; }
        public DateTime  eDate { get; set; }
        public string NRDType { get; set; }
        public string Term { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int SrNo { get; set; }
    }

    public class VendorCommisionData
    {
        public int VendorId { get; set; }
        public string CompanyName { get; set; }
        public double Total { get; set; }
    }

    public class VendorCommisionReportVm
    {
        public IEnumerable<VendorCommisionData> Data { get; set; }
        public double Total { get; set; }
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
    }

    public class CabReportModel
    {
        public IEnumerable<CabReportData> Data { get; set; }
        public double Total { get; set; }
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public string Term { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int MenuId { get; set; }
        public int SrNo { get; set; }
    }

    public class DriverReportModel
    {
        public IEnumerable<DriverReportData> Data { get; set; }
        public double Total { get; set; }
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public string Term { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int MenuId { get; set; }
        public int SrNo { get; set; }
    }
}