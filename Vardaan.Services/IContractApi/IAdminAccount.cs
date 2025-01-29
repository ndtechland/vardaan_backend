using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.IContractApi
{
    public interface IAdminAccount
    {
        Task<AdminLoginDTO> AdminLogin(AdminLoginDTO model);
    }
}
