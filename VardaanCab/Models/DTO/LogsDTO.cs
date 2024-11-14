using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.DTO
{
    public class LogsDTO
    {
        public int Id { get; set; }
        public int UserLogin_Id { get; set; }
        public string Description { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}