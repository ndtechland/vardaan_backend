using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class EmployeeImplementation:IEmployee
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<EmployeeProfileDTO> GetProfileDetail(int id)
        {
			try
			{
                var empdata = ent.Employees.SingleOrDefault(x => x.Id == id && x.IsActive == true);

                if (empdata != null)
                {
                    var getstate = ent.StateMasters.Where(s => s.Id == empdata.StateId).FirstOrDefault();
                    var getcity = ent.CityMasters.Where(s => s.Id == empdata.CityId).FirstOrDefault();

                    var data = new EmployeeProfileDTO()
                    {
                        Id = empdata.Id,
                        Employee_First_Name = empdata.Employee_First_Name,
                        Employee_Middle_Name = empdata.Employee_Middle_Name,
                        Employee_Last_Name = empdata.Employee_Last_Name,
                        Email = empdata.Email,
                        MobileNumber = empdata.MobileNumber,
                        EmergencyContactNumber = empdata.AlternateNumber,
                        Employee_Id = empdata.Employee_Id,
                        EmployeeDepartment = empdata.EmployeeDepartment,
                        StateId = empdata.StateId,
                        CityId = empdata.CityId,
                        StateName = getstate.StateName,
                        CityName = getcity.CityName,
                        Pincode = empdata.Pincode,
                        CreatedDate = empdata.CreatedDate,
                        EmployeeCurrentAddress = empdata.EmployeeCurrentAddress,
                        Gender = empdata.Gender,
                        Company_Id = empdata.Company_Id,
                    };
                    return data;                }
                else
                {
                    return null;
                }
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> UpdateProfile(EmployeeProfileDTO model)
        {
            try
            {
                var emp = ent.Employees.Find(model.Id);
                if (emp != null)
                {
                    emp.Employee_First_Name = model.Employee_First_Name;
                    emp.Employee_Middle_Name = model.Employee_Middle_Name;
                    emp.Employee_Last_Name = model.Employee_Last_Name;
                    emp.Email = model.Email;
                    //emp.MobileNumber = model.MobileNumber;
                    emp.EmployeeCurrentAddress = model.EmployeeCurrentAddress;
                    emp.StateId = model.StateId;
                    emp.CityId = model.CityId;
                    emp.Pincode = model.Pincode;
                    emp.AlternateNumber = model.EmergencyContactNumber;
                    ent.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> Addhelp(HelpEmployee model)
        {
            try
            {
                var data = new HelpEmployee()
                {
                    Employee_id = model.Employee_id,
                    PhoneNumber = model.PhoneNumber,
                    Reason = model.Reason,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                ent.HelpEmployees.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AddFeedback(FeedBackEmployee model)
        {
            try
            {
                var data = new FeedBackEmployee()
                {
                    Employee_Id = model.Employee_Id,
                    Feedback = model.Feedback,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                ent.FeedBackEmployees.Add(data);
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
