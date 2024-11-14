using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class NrgPackageDTO
    {
        public int Id { get; set; }
        public int RentalType_Id { get; set; }
        public int VehicleModel_Id { get; set; }
        public double Fare { get; set; }
        public int MinKm { get; set; }
        public int MinHrs { get; set; }
        public int ExtraPerHour { get; set; }
        public int ExtraPerKm { get; set; }
        public int NightCharges { get; set; }
        public int ChauffeurTaDa { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public double InterStateCharge { get; set; }
        public double OutStationCharge { get; set; }
        public string ModelName { get; set; }
        public string RentalType { get; set; }
        public string UpdatedByUser { get; set; }
        public SelectList RentalTypes { get; set; }
        public SelectList VehicleModels { get; set; }
    }
}