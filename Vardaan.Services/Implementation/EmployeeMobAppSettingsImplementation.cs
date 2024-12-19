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
    public class EmployeeMobAppSettingsImplementation:IEmployeeMobAppSettings
    {
        Vardaan_AdminEntities ent =new Vardaan_AdminEntities();
        public async Task<List<CabReqDays>> GetCabRequestDayList()
        {
			try
			{
                var data = (from es in ent.EmployeeMobileAppSettings
                                join c in ent.Customers on es.CompanyId equals c.Id
                                where es.IsActive == true
                                orderby es.Id descending
                                select new CabReqDays
                                {
                                    Id = es.Id,
                                    CompanyName = c.CompanyName,
                                    CompanyId = es.CompanyId,
                                    CreatedDate = es.CreatedDate,
                                    CabRequestDays = es.CabRequestDays
                                }
                      ).ToList();
                return data;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> AddCabRequestDays(EmployeeMobAppSettingDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var checkinfo = ent.EmployeeMobileAppSettings.Where(x => x.CompanyId == model.CompanyId).FirstOrDefault();
                    if (checkinfo != null)
                    {
                        return false;
                    }
                    var EmpcabReqday = new EmployeeMobileAppSetting()
                    {
                        CompanyId = model.CompanyId,
                        CabRequestDays = model.CabRequestDays,
                        IsActive = true,
                        CreatedDate = DateTime.Now

                    };
                    ent.EmployeeMobileAppSettings.Add(EmpcabReqday);
                }
                else
                {
                    var data = ent.EmployeeMobileAppSettings.Find(model.Id);
                    data.CompanyId = model.CompanyId;
                    data.CabRequestDays = model.CabRequestDays;
                }
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeletedRequestDay(int id)
        {
            try
            {
                var data = ent.EmployeeMobileAppSettings.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
