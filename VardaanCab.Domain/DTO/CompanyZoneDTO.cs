using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class CompanyZoneDTO
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyZone { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MenuId { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<CompanyZoneList> CompanyZoneLists { get; set; }
    }
    public class CompanyZoneList
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyZone { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
