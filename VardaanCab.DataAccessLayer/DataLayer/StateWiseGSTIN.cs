//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VardaanCab.DataAccessLayer.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class StateWiseGSTIN
    {
        public int Id { get; set; }
        public int State_Id { get; set; }
        public string Gstin { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string BankName { get; set; }
        public string AC_No { get; set; }
        public string BranchAddress { get; set; }
        public string IFS_Code { get; set; }
        public string CompanyName { get; set; }
        public string CIN_No { get; set; }
        public string PIN_Code { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string CompanyRegAdd { get; set; }
        public string CompanyRegState { get; set; }
    }
}
