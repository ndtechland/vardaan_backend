using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;

namespace VardaanCab.Models.DTO
{
    public class CabDTO
    {
        public int Id { get; set; }
        [Required]
        public int Vendor_Id { get; set; }
        [Required]
        public int VehicleModel_Id { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string VehicleNumber { get; set; }
        [Required]
        public double FuelEfficiency { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        [Required]
        public System.DateTime FitnessVality { get; set; }
        [Required]
        public System.DateTime PolutionValidity { get; set; }
        [Required]
        public System.DateTime InsurranceValidity { get; set; }
        public string FitnessDoc { get; set; }
        public string PolutionDoc { get; set; }
        public string InsuranceDoc { get; set; }
        public string RCDoc { get; set; }
        public bool IsActive { get; set; }
        public HttpPostedFileBase FitnessDocFile { get; set; }
        public HttpPostedFileBase PolutionDocFile { get; set; }
        public HttpPostedFileBase InsuranceDocFile { get; set; }
        public HttpPostedFileBase RCDocFile { get; set; }
        public SelectList VehicleModels { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string ModelName { get; set; }
        public bool IsAvailable { get; set; }
        public string PermitNo { get; set; }
        public string PermitDoc { get; set; }
        public HttpPostedFileBase PermitDocFile  { get; set; }
        [Required]
        public System.DateTime PermitValidity { get; set; }
        public string RcNumber { get; set; }
        public System.DateTime? RcValidity { get; set; }
        public System.DateTime? RcIssueDate { get; set; }
        public int MenuId { get; set; }
    }
}