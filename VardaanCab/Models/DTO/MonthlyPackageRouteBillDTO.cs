using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class MonthlyPackageRouteBillDTO
    {
        public int Id { get; set; }
        public int MonthlyPackageRoute_Id { get; set; }
        public string BillFile { get; set; }
        public string BillNumber { get; set; }
        public bool IsCancelled { get; set; }
        public double Fare { get; set; }
        public int NoOfDays { get; set; }
        public double NetAmount { get; set; }
        public double GstInPercent { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        [Required]
        public int BillingState_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdatedBy_Id { get; set; }
        public System.DateTime BillDate { get; set; }
        public string  UpdatedBy { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public string term { get; set; }
        public int page { get; set; }
        public SelectList StateWiseGstinList { get; set; }
    }
}