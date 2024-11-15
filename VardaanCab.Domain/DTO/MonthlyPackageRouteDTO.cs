using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class MonthlyPackageRouteDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please select client")]
        public int Customer_Id { get; set; }
        public int UpdatedBy_Id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        [Required]
        public string BookedBy { get; set; }
        [Required(ErrorMessage ="Please select vehicle model")]
        public int VehicleModel_Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        [Required]
        public int NoOfDays { get; set; }
        [Required]
        public double Amount { get; set; }
        public bool IsClosed { get; set; }
        public int CreatedBy_Id { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public string ContactNo { get; set; }
        public string CustomerName { get; set; }
        public string ModelName { get; set; }
        public SelectList ClientList { get; set; }
        public SelectList VehicleModelList { get; set; }
        [Required (ErrorMessage ="Please select Location")]
        public int MonthlyRoutePackageMaster_Id { get; set; }
        [Required]
        public System.DateTime BookingStartDate { get; set; }
       
        public System.DateTime BookingEndDate { get; set; }
        [Required]
        public System.TimeSpan PickupTime { get; set; }

    }
}