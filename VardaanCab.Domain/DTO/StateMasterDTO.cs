using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class StateMasterDTO
    {
        public int Id { get; set; }
        [Required]
        public string StateName { get; set; }
        [Required]
        public string StateCode { get; set; }
        public double GST { get; set; }
        public int MenuId { get; set; }
    }
}