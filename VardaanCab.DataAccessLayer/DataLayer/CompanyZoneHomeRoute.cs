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
    
    public partial class CompanyZoneHomeRoute
    {
        public int Id { get; set; }
        public Nullable<int> CompanyZoneId { get; set; }
        public string HomeRouteName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<int> Company_Id { get; set; }
    }
}
