using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class BookingCartVM
    {
        public int Id { get; set; }
        public string BookingId { get; set; }
        public string CompanyName { get; set; }
        public string PackageTypeName { get; set; }
    }
}