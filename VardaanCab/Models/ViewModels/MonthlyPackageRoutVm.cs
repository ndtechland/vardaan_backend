using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class MonthlyPackageRoutVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<MonthlyPackageRouteDTO> Packages { get; set; }

    }
        public class MonthlyRouteMasterVm
        {
            public int Page { get; set; }
            public int NumberOfPages { get; set; }
            public string Term { get; set; }
            public DateTime? sDate { get; set; }
            public DateTime? eDate { get; set; }
            public int MenuId { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<MonthlyPackageRouteMasterDTO> Packages { get; set; }
        }
    }