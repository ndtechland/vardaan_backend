using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;

namespace Vardaan.Services.Implementation
{
    public class CommonImplementation : ICommon
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public async Task<List<BannerMaster>> GetBanner(string role)
        {
            try
            {
                List<BannerMaster> banner = await ent.BannerMasters.Where(b => b.Role == role).ToListAsync();
                return banner;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
    }
}
