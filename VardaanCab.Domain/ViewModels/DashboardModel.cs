using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.ViewModels
{
    public class DashboardModel
    {
        public IEnumerable<SoftwareLink> Data { get; set; }
    }
}