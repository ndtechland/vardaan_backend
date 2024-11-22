using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VardaanCab.Domain.DTO
{
    public class BannerDTO
    {
        public int Id { get; set; }
        public string BannerImage { get; set; }
        public string Role { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
