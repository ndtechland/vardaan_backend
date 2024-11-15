using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Domain.ViewModels
{
    public class CreateDriverModel
    {
        [Required]
        public string DriverName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
       
        public string Address { get; set; }
    }

    public class CreateCabModel
    {
        [Required]
        public int Vendor_Id { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string VehicleNumber { get; set; }
      
        
    }

    public class CreateCabDriverModel
    {
        [Required]
        public string DriverName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        
        public string Address { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string VehicleNumber { get; set; }
    }
}