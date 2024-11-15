using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class VehicleModelDTO
    {
        public int Id { get; set; }
        public string ManufactureName { get; set; }
        [Required]
        public string ModelName { get; set; }
        public int MenuId { get; set; }
    }
}