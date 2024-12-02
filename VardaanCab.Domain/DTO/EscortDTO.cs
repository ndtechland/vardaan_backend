using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VardaanCab.Domain.DTO
{
    public class EscortDTO
    {
        public int Id { get; set; }
        public string EscortName { get; set; }
        public string EscortFatheName { get; set; }
        public string EscortMobileNumber { get; set; }
        public string EscortAadhaarNumber { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string EscortEmployeeId { get; set; }
        public string Pincode { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string PermanentAddress { get; set; }
        public string EscortAddress { get; set; }
        public int MenuId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public SelectList Companies {  get; set; }
        public SelectList Vendors {  get; set; }
        public IEnumerable<Escorts> EscortList { get; set; }
    }
    public class Escorts
    {
        public int Id { get; set; }
        public string EscortName { get; set; }
        public string EscortFatheName { get; set; }
        public string EscortMobileNumber { get; set; }
        public string EscortAadhaarNumber { get; set; }
        public string VendorName { get; set; }
        public string Gender { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeFullName { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string EscortEmployeeId { get; set; }
        public string Pincode { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string PermanentAddress { get; set; }
        public string EscortAddress { get; set; }
        public int MenuId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
