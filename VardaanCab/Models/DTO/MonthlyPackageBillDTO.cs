using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class MonthlyPackageBillDTO
    {
        public int Id { get; set; }
        [Required]
        public int MonthlyPackage_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdatedBy_Id { get; set; }
        public double TotalKm { get; set; }
        public double TotalHr { get; set; }
        public int ExtraKm { get; set; }
        public double ExtraKmRate { get; set; }
        public double ExtraKmCharge { get; set; }
        public double ExtraHr { get; set; }
        public double ExtraHrRate { get; set; }
        public double ExtraHrCharge { get; set; }
        public double NightRate { get; set; }
        public double TotalNights { get; set; }
        public double NightCharge { get; set; }
        public double TollCharge { get; set; }
        public double ParkingCharge { get; set; }
        public double MCD { get; set; }
        public double InterStateTax { get; set; }
        public string BillFile { get; set; }
        public string BillNumber { get; set; }
        public bool IsCancelled { get; set; }
        public double NetAmount { get; set; }
        public double GstInPercent { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        public double Fare { get; set; }
        public int NoOfDays { get; set; }
        public double FixedKm { get; set; }
        [Required]
        public int BillingState_Id { get; set; }
        public DateTime ? sDate { get; set; }
        public DateTime ? eDate { get; set; }
        public string term { get; set; }
        public int page { get; set; }
        public SelectList StateWiseGstinList { get; set; }
        public int MenuId { get; set; }
    }
}