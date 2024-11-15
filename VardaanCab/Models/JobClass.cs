using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;
using System.Data.Entity;

namespace VardaanCab.Models
{
    public class JobClass:IJob
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public void Execute(IJobExecutionContext context)
        {
            
        }

        
       


    }
}