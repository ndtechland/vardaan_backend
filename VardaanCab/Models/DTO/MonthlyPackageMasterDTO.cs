using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class MonthlyPackageMasterDTO
    {
        public int Id { get; set; }
        public int CreatedBy_Id { get; set; }
        public Nullable<int> UpdatedBy_Id { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        [Required]
        public double ExtraKmRate { get; set; }
        [Required]
        public double ExtraHourRate { get; set; }
        [Required]
        public double NightCharge { get; set; }
        [Required]
        public double Fare { get; set; }
        public SelectList VehicleModelList { get; set; }
        public int MenuId { get; set; }
        public string ModelName { get; set; }
        public string UpdatedBy { get; set; }
    }
}