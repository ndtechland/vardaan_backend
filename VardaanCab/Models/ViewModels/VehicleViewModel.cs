using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class VehicleViewModel
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<VehicleModel> VModels  { get; set; }
    }
}