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
    public class EscortImplementation:IEscort
    {
        Vardaan_AdminEntities ent =new Vardaan_AdminEntities();
        public async Task<bool> AddUpdateEscort(EscortDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var domainmodel = new Escort()
                    {
                        EscortFatheName = model.EscortFatheName,
                        CompanyId = model.CompanyId,
                        EscortName = model.EscortName,
                        EscortMobileNumber = model.EscortMobileNumber,
                        EscortAadhaarNumber = model.EscortAadhaarNumber,
                        VendorId = model.VendorId,
                        DOB = model.DOB,
                        EscortEmployeeId = model.EscortEmployeeId,
                        Pincode = model.Pincode,
                        PermanentAddress = model.PermanentAddress,
                        EscortAddress = model.EscortAddress,
                        IsActive = true,
                        CreatedDate = DateTime.Now

                    };
                    ent.Escorts.Add(domainmodel);
                }
                else
                {
                    var data = ent.Escorts.Find(model.Id);
                    data.EscortFatheName = model.EscortFatheName;
                    data.CompanyId = model.CompanyId;
                    data.EscortName = model.EscortName;
                    data.EscortMobileNumber = model.EscortMobileNumber;
                    data.EscortAadhaarNumber = model.EscortAadhaarNumber;
                    data.VendorId = model.VendorId;
                    data.DOB = model.DOB;
                    data.EscortEmployeeId = model.EscortEmployeeId;
                    data.Pincode = model.Pincode;
                    data.PermanentAddress = model.PermanentAddress;
                    data.EscortAddress = model.EscortAddress;

                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> DeleteEscort(int id)
        {
            try
            {
                var data = ent.Escorts.Find(id);
                data.IsActive = false;
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<Escorts>> GetEscorts()
        {
            try
            {
                var data = (from er in ent.Escorts
                            join e in ent.Employees on er.EscortEmployeeId equals e.Employee_Id
                            join c in ent.Customers on er.CompanyId equals c.Id
                            join v in ent.Vendors on er.VendorId equals v.Id
                            where er.IsActive == true
                            orderby er.Id descending
                            select new Escorts
                            {
                                Id = er.Id,
                                CompanyName = c.CompanyName,
                                EscortName = er.EscortName,
                                EscortMobileNumber = er.EscortMobileNumber,
                                EscortAadhaarNumber = er.EscortAadhaarNumber,
                                EscortFatheName = er.EscortFatheName,
                                EmployeeFullName = e.Employee_First_Name + " " + e.Employee_Middle_Name + " " + e.Employee_Last_Name,
                                VendorName = v.VendorName,
                                DOB = er.DOB,
                                EscortEmployeeId = er.EscortEmployeeId,
                                Pincode = er.Pincode,
                                PermanentAddress = er.PermanentAddress,
                                EscortAddress = er.EscortAddress,
                                CreatedDate = er.CreatedDate
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
