using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace VardaanCab.Domain.DTO
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Select ")]
        [Display(Name ="Select Company")]
        public int Company_Id { get; set; }
        [Display(Name = "Select Company Location")]
        public string Company_location { get; set; }
        [Display(Name = "Employee ID")]
        public string Employee_Id { get; set;}
        [Display(Name = "Employee First Name")]
        public string Employee_First_Name { get; set;}
        [Display(Name = "Employee Middle Name")]
        public string Employee_Middle_Name { get; set;}
        [Display(Name = "Employee Last Name")]
        public string Employee_Last_Name { get; set;}
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set;}
        [Display(Name = "Email ID")]
        public string Email { get; set;}
        [Display(Name = "Select State")]
        public int StateId { get; set;}
        [Display(Name = "Select City")]
        public int CityId { get; set;}
        [Display(Name = "Pin Code")]
        public int Pincode { get; set;}
        [Display(Name = "Employee Current Address")]
        public string EmployeeCurrentAddress { get; set;}
        [Display(Name = "Login User Name")]
        public string LoginUserName { get; set;}
        [Display(Name = "Select WeekOff")]
        public string WeekOff { get; set;}
        [Display(Name = "Employee Geo Code")]
        public string EmployeeGeoCode { get; set;}
        [Display(Name = "Employee Business Unit")]
        public string EmployeeBusinessUnit { get; set;}
        [Display(Name = "Employee Department")]
        public string EmployeeDepartment { get; set;}
        [Display(Name = "Employee Project Name")]
        public string EmployeeProjectName { get; set;}
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set;}
        [Display(Name = "Primary Company Zone")]
        public int PrimaryFacilityZone { get; set;}
        [Display(Name = "Home Route Name")]
        public int HomeRouteName { get; set;}
        [Display(Name = "Employee Destination Area")]
        public int EmployeeDestinationArea { get; set;}
        [Display(Name = "Employee Registration Type")]
        public int EmployeeRegistrationType { get; set;}
        public SelectList States { get; set; }
        public SelectList Customers { get; set; }
        public IEnumerable<SelectListItem> DayLists { get; set; }
        public List<string> WeekOffs { get; set; } = new List<string>();
    }
}
