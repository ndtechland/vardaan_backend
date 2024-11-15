using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class MonthlyPackageDTO
    {
        public int Id { get; set; }
        [Required]
        public int Customer_Id { get; set; }
        public int CreatedBy_Id { get; set; }
        public Nullable<int> UpdatedBy_Id { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [Required]
        public double Fare { get; set; }
        [Required]
        public string BookedBy { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        public int Cab_Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        [Required]
        public int NoOfDays { get; set; }
        [Required]
        public int FixedKms { get; set; }
        [Required]
        public int FixedHourPerDay { get; set; }
        [Required]
        public double ExtraKmRate { get; set; }
        [Required]
        public double ExtraHourRate { get; set; }
        [Required]
        public System.TimeSpan NightStartTime { get; set; }
        [Required]
        public System.TimeSpan NightEndTime { get; set; }
        [Required]
        public double NightCharge { get; set; }
        [Required]
        public bool IsClosed { get; set; }
        public SelectList ClientList { get; set; }
        public SelectList VehicleModelList { get; set; }
        public string CompanyName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ModelName { get; set; }
        public string ContactNo { get; set; }
        public string CustomerName { get; set; }
        public bool IsCancelled { get; set; }
        public int MenuId { get; set; }
    }
}