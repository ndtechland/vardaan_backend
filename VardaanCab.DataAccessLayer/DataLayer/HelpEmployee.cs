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
    
    public partial class HelpEmployee
    {
        public int Id { get; set; }
        public Nullable<int> Employee_id { get; set; }
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}