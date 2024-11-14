using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class CityMasterDTO
    {
        public int Id { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        public SelectList States { get; set; }
        public string StateName { get; set; }
        public int MenuId { get; set; }
    }
}