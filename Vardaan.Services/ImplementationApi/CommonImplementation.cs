using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContractApi;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.ImplementationApi
{
    public class CommonImplementation : ICommon
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        public async Task<List<BannerMaster>> GetBanner(string role)
        {
            try
            {
                List<BannerMaster> banner = await ent.BannerMasters.Where(b => b.Role == role).ToListAsync();
                return banner;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<StateMaster>> GetStates()
        {
            try
            {
                var data = await ent.StateMasters.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<CityMaster>> GetCity(int StateId)
        {
            try
            {
                var data = ent.CityMasters.Where(x => x.StateMaster_Id == StateId).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<Support> GetSupport()
        {
            try
            {
                var data = ent.Supports.FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<TripType>> GetTriptype()
        {
            try
            {
                var data = ent.TripTypes.Where(x => x.TripMasterId == 1).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<ShiftMaster>> GetPickupShifttimes()
        {
            try
            {
                var data = ent.ShiftMasters.Where(x => x.TripTypeId == 1).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<ShiftMaster>> GetDropShifttimes()
        {
            try
            {
                var data = ent.ShiftMasters.Where(x => x.TripTypeId == 2).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<List<TripMaster>> GetShiftTypes()
        {
            try
            {
                var data = ent.TripMasters.Where(x => x.Id == 1).ToList();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        public async Task<bool> GetUpdateEmployeeAndDriverDeviceId(UpdateDeviceDTO model)
        {
            try
            {
                if (model.RoleName == "Employee")
                {
                    var data = ent.Employees.Find(model.Id);
                    if (data != null)
                    {
                        data.DeviceId = model.DeviceId;
                        ent.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var data = ent.Drivers.Find(model.Id);
                    if (data != null)
                    {
                        data.DeviceId = model.DeviceId;
                        ent.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
