using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VardaanCab.Domain.DTOAPI
{
    public class EmployeeProfileDTO
    {
        public int Id { get; set; }
        public string Employee_Id { get; set; }
        public string Employee_First_Name { get; set; }
        public string Employee_Middle_Name { get; set; }
        public string Employee_Last_Name { get; set; }
        public string MobileNumber { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> Pincode { get; set; }
        public string EmployeeCurrentAddress { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeePic { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
