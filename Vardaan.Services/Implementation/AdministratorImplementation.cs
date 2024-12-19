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
    public class AdministratorImplementation: IAdministrator
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<bool> AddUpdateAssignAccess(AccessAssignDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var DomainModel = new AccessAssign()
                    {
                        CompanyId = model.CompanyId,
                        UserRoleId = model.UserRoleId,
                        EmployeeId = model.EmployeeId,
                        IsActive = true,
                        CreatedDate = DateTime.Now

                    };
                    ent.AccessAssigns.Add(DomainModel);
                }
                else
                {
                    var data = ent.AccessAssigns.Find(model.Id);
                    data.CompanyId = model.CompanyId;
                    data.UserRoleId = model.UserRoleId;
                    data.EmployeeId = model.EmployeeId;
                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<List<ListAccessAssign>> GetAccessAssigns()
        {
            try
            {
                var data = (from aa in ent.AccessAssigns
                            join e in ent.Employees on aa.EmployeeId equals e.Id
                            join c in ent.Customers on aa.CompanyId equals c.Id
                            join r in ent.UserRoles on aa.UserRoleId equals r.Id
                            where aa.IsActive == true
                            orderby aa.Id descending
                            select new ListAccessAssign
                            {
                                Id = aa.Id,
                                CompanyName = c.CompanyName,
                                RoleName = r.RoleName,
                                EmployeeName = e.Employee_First_Name + " " + e.Employee_Middle_Name + " " + e.Employee_Last_Name,
                                Mobile = e.MobileNumber,
                                EmployeeId = e.Employee_Id,
                                Email = e.Email,
                                UserName = e.LoginUserName,
                            }
                          ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<UserRoleList>> GetRoles()
        {
            try
            {
                var data = (from r in ent.UserRoles
                               join c in ent.Customers on r.CompanyId equals c.Id
                               where r.IsActive == true
                               orderby r.Id descending
                               select new UserRoleList
                               {
                                   Id = r.Id,
                                   CompanyName = c.CompanyName,
                                   OrgName = c.OrgName,
                                   RoleName = r.RoleName
                               }
                       ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddupdateRole(UserRoleDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var check = ent.UserRoles.Where(u => u.RoleName.ToLower() == model.RoleName.ToLower()).FirstOrDefault();
                    if (check != null)
                    {
                        return false;
                    }
                    var EmpReq = new UserRole()
                    {
                        CompanyId = model.CompanyId,
                        RoleName = model.RoleName,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        IsReadChecked = string.Join(",", model.IsReadChecked),
                        IsWriteChecked = string.Join(",", model.IsWriteChecked),
                        IsSubReadChecked = string.Join(",", model.IsSubReadChecked),
                        IsSubWriteChecked = string.Join(",", model.IsSubWriteChecked),
                        IsAllRead = model.IsAllRead,
                        IsAllWrite = model.IsAllWrite
                    };
                    ent.UserRoles.Add(EmpReq);
                    ent.SaveChanges();
                    return true;
                }
                else
                {
                    var check = ent.UserRoles.Where(u => u.RoleName.ToLower() == model.RoleName.ToLower() && u.Id != model.Id).FirstOrDefault();
                    if (check != null)
                    {
                        return false;
                    }
                    var data = ent.UserRoles.Find(model.Id);
                    data.CompanyId = model.CompanyId;
                    data.RoleName = model.RoleName;
                    data.IsReadChecked = string.Join(",", model.IsReadChecked);
                    data.IsWriteChecked = string.Join(",", model.IsWriteChecked);
                    data.IsSubReadChecked = string.Join(",", model.IsSubReadChecked);
                    data.IsSubWriteChecked = string.Join(",", model.IsSubWriteChecked);
                    data.IsAllRead = model.IsAllRead;
                    data.IsAllWrite = model.IsAllWrite;
                    ent.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteRole(int id)
        {
            try
            {
                var data = ent.UserRoles.Find(id);
                data.IsActive = false;
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
