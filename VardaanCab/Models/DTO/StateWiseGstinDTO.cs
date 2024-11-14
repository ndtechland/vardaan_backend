using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VardaanCab.Models.DTO
{
    public class StateWiseGstinDTO
    {
        public int Id { get; set; }
        [Required]
        public int State_Id { get; set; }
        [Required]
        [MinLength(15,ErrorMessage ="GSTIN must have 15 characters long only"),MaxLength(15, ErrorMessage = "GSTIN must have 15 characters long only")]
        public string Gstin { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string AC_No { get; set; }
        [Required]
        public string BranchAddress { get; set; }
        [Required]
        public string IFS_Code { get; set; }
        [Required]
        public string CompanyName { get; set; }        
        public string CompanyRegAdd { get; set; }
        public string CompanyRegState { get; set; }
        [Required]
        public string CIN_No { get; set; }
        [Required]
        public string PIN_Code { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public string StateName { get; set; }
        public string DisplayName { get; set; }
        public SelectList StateList { get; set; }
        public int MenuId { get; set; }
    }
}