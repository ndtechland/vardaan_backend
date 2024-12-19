using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;

namespace Vardaan.Services.Implementation
{
    public class AreaImplementation:IArea
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<bool> AddUpdateArea(AreaMasterDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var area = new AreaMaster()
                    {
                        StateMaster_Id = model.StateMaster_Id,
                        CityMaster_Id = model.CityMaster_Id,
                        AreaName = model.AreaName
                    };
                    ent.AreaMasters.Add(area);                    
                }
                else
                {
                    var data = ent.AreaMasters.Find(model.Id);
                    data.AreaName = model.AreaName;
                    data.StateMaster_Id = model.StateMaster_Id;
                    data.CityMaster_Id = model.CityMaster_Id;
                    
                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<List<AreaMasterDTO>> GetAreas()
        {
            try
            {
                var data = (from a in ent.AreaMasters
                            join c in ent.CityMasters on a.CityMaster_Id equals c.Id
                            join s in ent.StateMasters on a.StateMaster_Id equals s.Id
                            orderby a.Id descending
                            select new AreaMasterDTO
                            {
                                Id = a.Id,
                                AreaName = a.AreaName,
                                StateName = s.StateName,
                                CityName = c.CityName
                            }
                        ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteArea(int id)
        {
            try
            {
                var data = ent.AreaMasters.Find(id);
                ent.AreaMasters.Remove(data);
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
