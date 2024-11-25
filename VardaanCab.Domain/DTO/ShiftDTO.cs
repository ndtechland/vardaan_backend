using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class ShiftDTO
    {
        public int Id { get; set; }
        public Nullable<int> TripTypeId { get; set; }
        public string ShiftTime { get; set; }
        public IEnumerable<GetShift> ShiftList { get; set; }
    }
    public class GetShift
    {
        public int Id { get; set; }
        public Nullable<int> TripTypeId { get; set; }
        public string ShiftTime { get; set; }
        public string TripType { get; set; }
    }
}
