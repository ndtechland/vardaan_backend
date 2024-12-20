using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;
namespace Vardaan.Services.IContractApi
{
    public interface ICommon
    {
        Task<List<BannerMaster>> GetBanner(string role);
        Task<List<StateMaster>> GetStates();
        Task<List<CityMaster>> GetCity(int StateId);
        Task<Support> GetSupport();
        Task<List<TripType>> GetTriptype();
        Task<List<ShiftMaster>> GetPickupShifttimes();
        Task<List<ShiftMaster>> GetDropShifttimes();
        Task<List<TripMaster>> GetShiftTypes();
        Task<bool> GetUpdateEmployeeAndDriverDeviceId(UpdateDeviceDTO model);
    }
}
