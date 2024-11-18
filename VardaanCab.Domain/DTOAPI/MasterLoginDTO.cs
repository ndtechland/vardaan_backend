using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class MasterLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int OTP { get; set; }
    }
}
