using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.Implementation
{
    public class CompanyZoneImplementation: ICompanyZone
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<bool> AddUpdateZone(CompanyZoneDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var zone = new CompanyZone()
                    {
                        CompanyId = model.CompanyId,
                        CompanyZone1 = model.CompanyZone,
                        CreatedDate = DateTime.Now,
                        Zonelatlong = model.Zonelatlong
                    };
                    ent.CompanyZones.Add(zone);
                }
                else
                {
                    var data = ent.CompanyZones.Find(model.Id);
                    data.CompanyZone1 = model.CompanyZone;
                    data.CompanyId = model.CompanyId;
                    data.Zonelatlong = model.Zonelatlong;


                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<List<CompanyZoneList>> GetZones()
        {
            try
            {
                var data = (from cz in ent.CompanyZones
                            join c in ent.Customers on cz.CompanyId equals c.Id
                            orderby cz.Id descending
                            select new CompanyZoneList
                            {
                                Id = cz.Id,
                                CompanyZone = cz.CompanyZone1,
                                CompanyName = c.CompanyName,
                                CreatedDate = cz.CreatedDate
                            }
                        ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteZone(int id)
        {
            try
            {
                var data = ent.CompanyZones.Find(id);
                ent.CompanyZones.Remove(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddUpdateHomeRoute(EmployeeHomeRouteDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var zone = new CompanyZoneHomeRoute()
                    {
                        CompanyZoneId = model.CompanyZoneId,
                        HomeRouteName = model.HomeRouteName,
                        CreatedDate = DateTime.Now
                    };
                    ent.CompanyZoneHomeRoutes.Add(zone);
                }
                else
                {
                    var data = ent.CompanyZoneHomeRoutes.Find(model.Id);
                    data.CompanyZoneId = model.CompanyZoneId;
                    data.HomeRouteName = model.HomeRouteName;

                }
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<HomeRouteList>> GetHomeRoutes()
        {
            try
            {
                var data = (from hr in ent.CompanyZoneHomeRoutes
                            join cz in ent.CompanyZones on hr.CompanyZoneId equals cz.Id
                            orderby cz.Id descending
                            select new HomeRouteList
                            {
                                Id = hr.Id,
                                CompanyZone = cz.CompanyZone1,
                                HomeRouteName = hr.HomeRouteName,
                                CreatedDate = hr.CreatedDate
                            }
                         ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteHomeRoute(int id)
        {
            try
            {
                var data = ent.CompanyZoneHomeRoutes.Find(id);
                ent.CompanyZoneHomeRoutes.Remove(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddUpdateDestinationArea(EmployeeDestinationAreaDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var zone = new EmployeeDestinationArea()
                    {
                        CompanyZoneHomeRouteId = model.HomeRouteId,
                        DestinationAreaName = model.DestinationAreaName,
                        CreatedDate = DateTime.Now
                    };
                    ent.EmployeeDestinationAreas.Add(zone);
                }
                else
                {
                    var data = ent.EmployeeDestinationAreas.Find(model.Id);
                    data.CompanyZoneHomeRouteId = model.HomeRouteId;
                    data.DestinationAreaName = model.DestinationAreaName;

                }
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<DestinationAreaList>> GetDestinationAreas()
        {
            try
            {
                var data = (from da in ent.EmployeeDestinationAreas
                            join c in ent.CompanyZoneHomeRoutes on da.CompanyZoneHomeRouteId equals c.Id
                            orderby da.Id descending
                            select new DestinationAreaList
                            {
                                Id = da.Id,
                                DestinationAreaName = da.DestinationAreaName,
                                HomeRouteName = c.HomeRouteName,
                                CreatedDate = da.CreatedDate
                            }
                         ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteDestinationArea(int id)
        {
            try
            {
                var data = ent.EmployeeDestinationAreas.Find(id);
                ent.EmployeeDestinationAreas.Remove(data);
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
