using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public string StateName  { get; set; }
        public string ProfilePic { get; set; }
        public System.DateTime CreateDate { get; set; }
        [Required]
        [MaxLength(250, ErrorMessage ="Address can't exceed 250 characters")]
        public string FullAddress { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactNo { get; set; }
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string AlternateNo { get; set; }
        public int ? ParentCustomer_Id { get; set; }
        public string OrgName { get; set; }
        public SelectList CustomerList { get; set; }
        public SelectList StateList  { get; set; }

        public SelectList CityList { get; set; }
        public string ParentCustomer { get; set; }
        [Required]
        public string CompanyName  { get; set; }
        [MaxLength(15, ErrorMessage = "GSTIN must have 15 characters long only")]
        public string GSTIN { get; set; }
        public int State_Id { get; set; }
        public int City_Id { get; set; }

        public List<ClientPackageDTO> Packages { get; set; }
        public string StateCode { get; set; }
        public int MenuId { get; set; }
        public string City_Name { get; set; }
        public string GeoLocation { get; set; }
        

    }

    public class CustomerExcelModel
    {
        public string CustomerName { get; set; }
        public string ContactPerson { get; set; }
        public string ParentCustomer { get; set; }
        public string ContactNo { get; set; }
        public string AlternateNo { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }
        public string FullAddress { get; set; }
        public string GSTIN { get; set; }
        public string StateCode { get; set; }
        public string City_Name { get; set; }
    }

    public class CustomerWithCity
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameWithCity { get; set; }
    }

    public class createorg
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int MenuId { get; set; }
        public string OrgName { get; set; }
        public SelectList Companies { get; set; }
        public IEnumerable<Customer> OrgNameList { get; set; }

    }
}