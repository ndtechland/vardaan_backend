using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class MonthlyPackageRouteMasterDTO
    {
        public int Id { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        [Required]
        public string PickupLocation { get; set; }
        [Required]
        public string DropLocation { get; set; }
        [Required]
        public double Fare { get; set; }
        [Required]
        public int NoOfDays { get; set; }
        public SelectList VehicleModelList { get; set; }
        public int UpdatedBy_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public int MenuId { get; set; }
        public string ModelName { get; set; }
    }
}