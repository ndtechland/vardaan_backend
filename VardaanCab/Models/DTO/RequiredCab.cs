using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.DTO
{
    public class RequiredCab
    {
        public string CityName { get; set; }
        public string ModelName { get; set; }
        public int BookingCount { get; set; }
    }
}