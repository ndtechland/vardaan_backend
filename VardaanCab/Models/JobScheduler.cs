using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models;

namespace VardaanCab.Models
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job1 = JobBuilder.Create<JobClass>().Build();
            ITrigger trigger1 = TriggerBuilder.Create()
                       .WithIdentity("trigger1", "group1")
              .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(18,25)
              .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")))
              .Build();
            scheduler.ScheduleJob(job1, trigger1);
           
        }
    }

}