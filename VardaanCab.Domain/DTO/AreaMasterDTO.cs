using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class AreaMasterDTO
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public Nullable<int> CityMaster_Id { get; set; }
        public Nullable<int> StateMaster_Id { get; set; }
        public int MenuId { get; set; }
        public SelectList Cities { get; set; }
        public SelectList States { get; set; }
    }
}
