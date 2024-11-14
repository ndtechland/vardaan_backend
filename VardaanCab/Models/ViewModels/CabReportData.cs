using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Models.ViewModels
{
    public class CabReportData
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public string ModelName { get; set; }
        public string CompanyName { get; set; }
        public int TotalRides { get; set; }
       
        public double TotalKms { get; set; }
        public double TotalHrs { get; set; }
        public double TotalAmt { get; set; }
    }

    public class DriverReportData
    {
        public int Id { get; set; }
        public int TotalRides { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public double TotalHrs { get; set; }

        public double TotalKms { get; set; }
        public int TotalNight { get; set; }
        public double TotalTA { get; set; }
        public int TotalDuty { get; set; }
        public double TotalAmt { get; set; }
    }
}