using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class DriverLeaveDTO
    {
        public int Id { get; set; }
        [Required]
        public int Driver_Id { get; set; }
        [Required]
        public System.DateTime DateFrom { get; set; }
        [Required]
        public System.DateTime DateTo { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public List<SelectListItem> Drivers { get; set; }
        public string DriverName { get; set; }
        public string Mobile { get; set; }
        public int MenuId { get; set; }
    }
}