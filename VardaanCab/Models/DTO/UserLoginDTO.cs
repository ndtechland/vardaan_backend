using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.DTO
{
    public class UserLoginDTO
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int MenuId { get; set; }
        public bool IsActive { get; set; }
    }
}