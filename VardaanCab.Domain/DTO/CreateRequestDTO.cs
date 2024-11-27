using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTO
{
    public class CreateRequestDTO
    {
        public string RequestType { get; set; }
        public string EmployeeId { get; set; } 
        public string Gender { get; set; } 
        public string Email { get; set; } 
        public string Unit { get; set; } 
        public string Department { get; set; } 
        public string RequestorEmployeeId { get; set; } 
        public string RequestorName { get; set; } 
        public DateTime? BookingDate { get; set; }
        public DateTime? StartRequestDate { get; set; }
        public DateTime? EndRequestDate { get; set; }
        public string TripType { get; set; }
        public string LocationType { get; set; }
        public string ShiftType { get; set; }
    }
}
