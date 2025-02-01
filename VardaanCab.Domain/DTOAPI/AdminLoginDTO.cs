using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class AdminLoginDTO
    {
        public int Id { get; set; }
        public string TransportCode { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string UserRole { get; set; }
        public string EmployeeName { get; set; }
        public string Password { get; set; }
    }
}
