using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class CorporatePackageDTO
    {
        public int Id { get; set; }
        [Required]
        public int RentalType_Id { get; set; }
        [Required]
        public double Fare { get; set; }
        [Required]
        public int MinKm { get; set; }
        [Required]
        public int MinHrs { get; set; }
        [Required]
        public double ExtraPerHour { get; set; }
        [Required]
        public double ExtraPerKm { get; set; }
        [Required]
        public double NightCharges { get; set; }
        public int ChauffeurTaDa { get; set; }
        [Required]
        public double InterStateCharge { get; set; }
        [Required]
        public double OutStationCharge { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public SelectList VehicleModels { get; set; }
        public SelectList PackageTypes { get; set; }
        public IEnumerable<SelectListItem> RentalTypes { get; set; }
        public string ModelName { get; set; }
        public string RentalType { get; set; }
        public string UpdatedByUser { get; set; }
        public int MenuId { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int NoOfDays { get; set; }
        public int PackageType_Id { get; set; }
        public string PackageTypeName { get; set; }
    }
}