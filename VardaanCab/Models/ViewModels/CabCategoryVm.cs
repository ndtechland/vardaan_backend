using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;

namespace VardaanCab.Models.ViewModels
{
    public class CabCategoryVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<CabCategory> Categories { get; set; }
    }
}