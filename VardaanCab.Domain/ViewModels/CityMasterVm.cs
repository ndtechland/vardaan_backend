﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Domain.ViewModels
{
    public class CityMasterVm
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public int SrNo { get; set; }
        public IEnumerable<CityMasterDTO> Cities { get; set; }
    }
}