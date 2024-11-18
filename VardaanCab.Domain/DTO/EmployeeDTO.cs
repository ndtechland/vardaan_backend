using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace VardaanCab.Domain.DTO
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public int Company_Id { get; set; }
        public string Company_location { get; set; }
        public string Employee_Id { get; set;}
        public string Employee_First_Name { get; set;}
        public string Employee_Middle_Name { get; set;}
        public string Employee_Last_Name { get; set;}
        public string MobileNumber { get; set;}
        public string Email { get; set;}
        public int StateId { get; set;}
        public int CityId { get; set;}
        public int Pincode { get; set;}
        public string EmployeeCurrentAddress { get; set;}
        public string LoginUserName { get; set;}
        public string WeekOff { get; set;}
        public string EmployeeGeoCode { get; set;}
        public string EmployeeBusinessUnit { get; set;}
        public string EmployeeDepartment { get; set;}
        public string EmployeeProjectName { get; set;}
        public string ReportingManager { get; set;}
        public int PrimaryFacilityZone { get; set;}
        public int HomeRouteName { get; set;}
        public int EmployeeDestinationArea { get; set;}
        public int EmployeeRegistrationType { get; set;}
    }
}
