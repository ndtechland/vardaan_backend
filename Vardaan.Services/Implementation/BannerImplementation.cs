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
    }
}
