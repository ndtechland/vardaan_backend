using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VardaanCab.Domain.ViewModels
{
    public class LoginHistoryViewModel
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<LoginHistoryDTO> History { get; set; }
    }

    public class LoginHistoryDTO 
    {
        public int Id  { get; set; }
        public int UserLogin_Id  { get; set; }
        public string UserName  { get; set; }
        public string Ip_Address  { get; set; }
        public DateTime UpdateDate { get; set; }
       
    }
}