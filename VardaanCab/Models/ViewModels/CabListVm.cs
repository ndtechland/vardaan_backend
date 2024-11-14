using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class CabListVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public IEnumerable<CabDTO> Cabs { get; set; }
        public int SrNo { get; set; }
    }

    public class CabExcelModel
    {
        public string ModelName { get; set; }
        public string VehicleNumber { get; set; }
        public double FuelEfficiency { get; set; }
        public string VendorName { get; set; }
        public DateTime PolutionValidity { get; set; }
        public DateTime InsurranceValidity { get; set; }
        public DateTime FitnessVality { get; set; }
        public string RcNumber { get; set; }
        public DateTime? RcIssueDate { get; set; }
        public DateTime? RcValidity { get; set; }
        public DateTime PermitValidity { get; set; }
        public string PermitNo { get; set; }
        public DateTime CreateDate { get; set; }
    }
}