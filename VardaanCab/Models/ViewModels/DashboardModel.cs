using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;

namespace VardaanCab.Models.ViewModels
{
    public class DashboardModel
    {
        public IEnumerable<SoftwareLink> Data { get; set; }
    }
}