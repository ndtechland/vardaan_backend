using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class ChangePasswordDTO
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
