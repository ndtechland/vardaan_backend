using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTO
{
    public class ExcelErrorModel
    {
        public string ErrorType { get; set; }

        public string Description { get; set; }
        public int AffectedRow { get; set; }
    }
}
