using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class RemainingHourModel
    {
        public DateTime JourneyStartDate { get; set; }
        public DateTime JourneyEndDate { get; set; }
        public TimeSpan JourneyStartTime { get; set; }
        public TimeSpan JourneyEndTime { get; set; }
    }
}