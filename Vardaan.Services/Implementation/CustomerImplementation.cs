using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.Implementation
{
    public class CustomerImplementation:ICustomer
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<bool> CreateOrgName(createorg model)
        {
            try
            {
                var customerinfo = ent.Customers.Where(c => c.IsActive && c.Id == model.CompanyId).FirstOrDefault();

                if (customerinfo != null)
                {
                    customerinfo.OrgName = model.OrgName;
                    ent.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
