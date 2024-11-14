using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class MonthlyPackageVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public DateTime ? sDate { get; set; }
        public DateTime ? eDate { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<MonthlyPackageDTO> Packages { get; set; }
        public int MenuId { get; set; }
    }


    public class MonthlyPackageMasterVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public int MenuId { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<MonthlyPackageMasterDTO> Packages { get; set; }
    }
}