﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class UpdateDeviceDTO
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string RoleName { get; set; }
    }
}