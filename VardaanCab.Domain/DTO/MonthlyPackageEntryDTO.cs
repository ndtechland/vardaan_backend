using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class MonthlyPackageEntryDTO
    {
        public int Id { get; set; }
        [Required]
        public int MonthlyPackage_Id { get; set; }
        [Required]
        public System.DateTime EntryDate { get; set; }
        public string CabNo { get; set; }
        [Required]
        public int StKm { get; set; }
        public int EndKm { get; set; }
        public int ExtraKm { get; set; }
        [Required]
        public System.TimeSpan StartHr { get; set; }
        public System.TimeSpan EndHr { get; set; }
        public double ExtraHr { get; set; }
        [Required]
        public double NightRate { get; set; }
        public int TotalNight { get; set; }
        public double NightCharge { get; set; }
        public double TollCharge { get; set; }
        public double ParkingCharge { get; set; }
        public double MCD { get; set; }
        public double InterStateTax { get; set; }
        [Required]
        public int Cab_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdatedBy_Id { get; set; }
        public double TotalKm { get; set; }
        public double TotalHrs { get; set; }
        public double MinHrs { get; set; }
        public double ExtraHrsRate { get; set; }
        public double ExtraHrsCharge { get; set; }

        public double NetAmount { get; set; }
        public double Total { get; set; }
        [Required]
        public System.DateTime JourneyStartDate { get; set; }
        public System.DateTime  ? JourneyClosingDate { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        public int  Driver_Id { get; set; }
        public string VehicleModel { get; set; }
        public string UpdatedBy { get; set; }
        public string DriverName { get; set; }
        public int MenuId { get; set; }
        public string CustomerName { get; set; }
        public string PrevUrl { get; set; }
    }
}