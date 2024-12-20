using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class DriverImplementation:IDriver
    {
        Vardaan_AdminEntities ent=new Vardaan_AdminEntities();
        public async Task<DriverProfileDTO> GetProfile(int id)
        {
			try
			{
                var driver = ent.Drivers.SingleOrDefault(d => d.Id == id && d.IsActive);
                var model = new DriverProfileDTO
                {
                    Id = driver.Id,
                    DriverName = driver.DriverName,
                    Address = driver.DriverAddress,
                    Email = driver.Email,
                    MobileNumber = driver.MobileNumber,
                    DlImage = driver.DlImage,
                    DriverImage = driver.DriverImage,
                    DlNumber = driver.DlNumber,
                    CreateDate = driver.CreateDate,
                    AlternateNo1 = driver.AlternateNo1,
                };
                return model;
            }
            catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> UpdateProfile(DriverProfileDTO model)
        {
            try
            {
                var checkdriver = ent.Drivers.Find(model.Id);
                if (checkdriver != null)
                {
                    checkdriver.DriverName = model.DriverName;
                    checkdriver.Email = model.Email;
                    checkdriver.DriverAddress = model.Address;
                    checkdriver.AlternateNo1 = model.AlternateNo1;
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
        public async Task<bool> UpdateActiveStatus(DriverDTO model)
        {
            try
            {
                var data = ent.Drivers.Where(d => d.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.IsOnline = model.IsOnline;
                    ent.SaveChanges();
                    if (model.IsOnline == true)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
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
    }
}
