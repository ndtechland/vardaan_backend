using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class MopMasterDTO
    {
        public int Id { get; set; }
        [Required]
        public string Mop { get; set; }
    }
}