using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.Implementation
{
    public class DepartmentImplementation:IDepartment
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities(); 
        public async Task<bool> AddDepartments(DepartmentMasterDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var department = new DepartmentMaster()
                    {
                        CompanyId = model.CompanyId,
                        DepartmentName = model.DepartmentName,
                        IsActive = true,
                        Created = DateTime.Now

                    };
                    ent.DepartmentMasters.Add(department);
                }
                else
                {
                    var data = ent.DepartmentMasters.Find(model.Id);
                    data.Id = model.Id;
                    data.CompanyId = model.CompanyId;
                    data.DepartmentName = model.DepartmentName;
                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> DeleteDepartment(int id)
        {
            try
            {
                var data = ent.DepartmentMasters.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<DepartmentList>> DepartmentList()
        {
            try
            {
                var data = (from dm in ent.DepartmentMasters
                            join c in ent.Customers on dm.CompanyId equals c.Id
                            orderby dm.Id descending
                            where dm.IsActive == true
                            select new DepartmentList
                            {
                                Id = dm.Id,
                                DepartmentName = dm.DepartmentName,
                                CompanyName = c.CompanyName,
                                CreatedDate = dm.Created,
                            }
                           ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
