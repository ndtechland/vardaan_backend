using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.IContractApi
{
    public interface IEmployee
    {
        Task<EmployeeProfileDTO> GetProfileDetail(int id);
        Task<bool> UpdateProfile(EmployeeProfileDTO model);
        Task<bool> Addhelp(HelpEmployee model);
        Task<bool> AddFeedback(FeedBackEmployee model);
    }
}
