﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Utilities
{
    public class VRCHelper
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public List<CustomerWithCity> getCustomerWithCity()
        {
            string cpbQuery = @"select c.Id,CompanyName as CustomerName,CompanyName+' - '+ci.CityName as CustomerNameWithCity from Customer c join CityMaster ci
                on c.City_Id=ci.Id";
            var data = ent.Database.SqlQuery<CustomerWithCity>(cpbQuery).ToList();
            return data;
        }
    }
}