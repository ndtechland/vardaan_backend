using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.DTO
{
    public class RoleManageModel
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        public int[] IsChecked { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public List<SoftwareLinkDTO2> Rights { get; set; }
    }
    public class SoftwareLinkDTO2
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsHeading { get; set; }
        public bool IsMenu { get; set; }
        public int? Parent_Id { get; set; }
        public bool IsChecked { get; set; }
        public IEnumerable<SoftwareLinkDTO2> ChildMenus { get; set; }
    }
    public  class CustomerUserMappingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }      
        public string CityName { get; set; }
        public string GSTIN { get; set; }
        public bool IsChecked { get; set; }
        
    }

    public class ClientUserMappingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string GSTIN { get; set; }
        public bool IsChecked { get; set; }

    }
}