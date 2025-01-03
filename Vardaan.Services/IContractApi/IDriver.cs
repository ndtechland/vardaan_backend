﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.DTOAPI;

namespace Vardaan.Services.IContractApi
{
    public interface IDriver
    {
        Task<DriverProfileDTO> GetProfile(int id);
        Task<bool> UpdateProfile(DriverProfileDTO model);
        Task<bool> UpdateActiveStatus(DriverDTO model);
    }
}
