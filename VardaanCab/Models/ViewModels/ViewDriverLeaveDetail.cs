using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class ViewDriverLeaveDetail
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term {get; set;}
        public int SrNo { get; set; }
        public IEnumerable<DriverLeaveDTO> DriverLeaves { get; set; }
    }
}