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
    
    public partial class UserRole
    {
        public int Id { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string IsReadChecked { get; set; }
        public string IsWriteChecked { get; set; }
        public string IsSubReadChecked { get; set; }
        public string IsSubWriteChecked { get; set; }
        public Nullable<bool> IsAllRead { get; set; }
        public Nullable<bool> IsAllWrite { get; set; }
    }
}
