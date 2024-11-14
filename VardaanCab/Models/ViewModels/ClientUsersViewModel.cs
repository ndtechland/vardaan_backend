using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;

namespace VardaanCab.Models.ViewModels
{
    public class ClientUsersViewModel
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<ClientUserAll> clientUsers { get; set; }
    }
    public  class ClientUserAll
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ClientName { get; set; }
        public string CompanyName { get; set; }
    }
}