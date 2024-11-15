using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class VendorPersonalPackageDTO
    {
        public int Id { get; set; }
        public int Vendor_Id { get; set; }
       
        public int RentalType_Id { get; set; }
        public int VehicleModel_Id { get; set; }
        public double Fare { get; set; }
        public int MinKm { get; set; }
        public int MinHrs { get; set; }
        public double ExtraPerHour { get; set; }
        public double ExtraPerKm { get; set; }
        public double NightCharges { get; set; }
        public int ChauffeurTaDa { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public double InterStateCharge { get; set; }
        public double OutStationCharge { get; set; }
       
        public string ModelName { get; set; }
        public string RentalTypeName { get; set; }
        public int CorporatePackage_Id { get; set; }
        public string UpdatedByUser { get; set; }
        public int VendorPackage_Id { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int NoOfDays { get; set; }
        public string PackageTypeName { get; set; }
        public int MenuId { get; set; }
    }
}