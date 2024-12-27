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
    public class ShiftImplementation:IShift
    {
        Vardaan_AdminEntities  ent=new Vardaan_AdminEntities(); 
        public async Task<List<GetShift>> GetShifts()
        {
			try
			{
                var data = (from sm in ent.ShiftMasters
                            join tt in ent.TripTypes on sm.TripTypeId equals tt.Id
                            join c in ent.Customers on sm.CompanyId equals c.Id
                            join dm in ent.DepartmentMasters on sm.DepartmentId equals dm.Id
                            //join cz in ent.CompanyZones on sm.CompanyZoneId equals cz.Id
                            orderby sm.Id descending
                            select new GetShift
                            {
                                Id = sm.Id,
                                TripType = tt.TripTypeName,
                                CompanyName = c.CompanyName,
                                //CompanyZoneName = cz.CompanyZone1,
                                DepartmentName = dm.DepartmentName,
                                ShiftBufferTime = sm.ShiftBufferTime,
                                ShiftTime = sm.ShiftTime
                            }
                       ).ToList();
                return data;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<bool> AddUpdateShift(ShiftMaster model)
        {
            try
            {
                if (model.Id == 0)
                {
                    var shift = new ShiftMaster()
                    {
                        TripTypeId = model.TripTypeId,
                        ShiftTime = model.ShiftTime,
                        CompanyId = model.CompanyId,
                        DepartmentId = model.DepartmentId,
                        ShiftBufferTime = model.ShiftBufferTime,
                        CompanyZoneId = model.CompanyZoneId
                    };
                    ent.ShiftMasters.Add(shift);
                }
                else
                {
                    var data = ent.ShiftMasters.Find(model.Id);
                    data.TripTypeId = model.TripTypeId;
                    data.ShiftTime = model.ShiftTime;
                    data.CompanyId = model.CompanyId;
                    data.DepartmentId = model.DepartmentId;
                    data.ShiftBufferTime = model.ShiftBufferTime;
                    data.CompanyZoneId = model.CompanyZoneId;

                }
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteShift(int id)
        {
            try
            {
                var data = ent.ShiftMasters.Find(id);
                ent.ShiftMasters.Remove(data);
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
