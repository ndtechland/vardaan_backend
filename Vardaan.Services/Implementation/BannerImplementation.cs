using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.Implementation
{
    public class BannerImplementation : IBanner
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public async Task<List<BannerMaster>> BannerList()
        {
			try
			{
               var data = await ent.BannerMasters.ToListAsync();
                return data;
            }
			catch (Exception ex)
			{

				throw new Exception("Server Error : " + ex.Message);
			}
        }
        public async Task <bool> Addbanners(BannerDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var domainModel = new BannerMaster
                    {
                        Role = model.Role,
                        BannerImage = model.BannerImage,
                        CreatedDate = DateTime.Now
                    };
                    ent.BannerMasters.Add(domainModel);
                }
                else
                {
                    var data = ent.BannerMasters.Find(model.Id);
                    data.Role = model.Role;
                    if (model.BannerImage != null)
                    {
                        data.BannerImage = model.BannerImage;
                    }
                }
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
