using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class RentalTypeDTO
    {
        public int Id { get; set; }
        [Required]
        public string RentalTypeName { get; set; }
        [Required]
        public int PackageType_Id { get; set; }
        public string PackageTypeName { get; set; }
        public int MenuId { get; set; }
        public IEnumerable<SelectListItem> PackageTypes { get; set; }
    }
}