using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.DataAccessLayer.DataLayer;

namespace VardaanCab.Domain.DTO
{
    public class SoftwareLinkDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsHeading { get; set; }
        public bool IsMenu { get; set; }
        public int? Parent_Id { get; set; }
        public IEnumerable<SoftwareLink> ChildMenus { get; set; }
    }

    
}