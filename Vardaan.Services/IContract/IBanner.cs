using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;

namespace Vardaan.Services.IContract
{
    public interface IBanner
    {
        Task<List<BannerMaster>> BannerList();
    }
}
