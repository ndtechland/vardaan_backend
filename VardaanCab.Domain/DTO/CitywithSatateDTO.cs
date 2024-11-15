using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class CitywithSatateDTO 
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public int StateMaster_Id { get; set; }
        public string StateName { get; set; }
    }
}