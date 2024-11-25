using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace VardaanCab.Controllers
{
    public class CommonController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        // GET: Common
        public ActionResult GetCityByState(int stateId)
        {
            var data = ent.CityMasters.Where(a => a.StateMaster_Id == stateId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCompanyZonesByID(int Customer_Id)
        {
            var data = ent.CompanyZones.Where(a => a.CompanyId == Customer_Id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetHomeRouteNameByID(int CompanyZone_ID)
        {
            var data = ent.CompanyZoneHomeRoutes.Where(a => a.CompanyZoneId == CompanyZone_ID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDestinationAreaByID(int DestinationArea_ID)
        {
            var data = ent.EmployeeDestinationAreas.Where(a => a.CompanyZoneHomeRouteId == DestinationArea_ID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRentalTypeByPackage(int packageId)
        {
            var data = ent.RentalTypes.Where(a => a.PackageType_Id == packageId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //updated by bhupesh
        public ActionResult GetCustomerByCity(int cityId)
        {
            var data = ent.Customers.Where(a => a.City_Id == cityId && a.IsActive==true).ToList().OrderBy(a => a.CompanyName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCustomerByClientId(int cuserId,int cityId)
        {
            string ptQuery = String.Empty;
           // var data = ent.Customers.Where(a => a.City_Id == cityId && a.IsActive == true).ToList().OrderBy(a => a.CompanyName);
            ptQuery = "select b.* from ClientUserMapping a join Customer b on a.CustomerId = b.Id where a.UserId="+ cuserId.ToString();
            var data = ent.Database.SqlQuery<Customer>(ptQuery).ToList().Where(a => a.City_Id == cityId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerByUserId(int userId, int cityId)
        {
            string ptQuery = String.Empty;
            // var data = ent.Customers.Where(a => a.City_Id == cityId && a.IsActive == true).ToList().OrderBy(a => a.CompanyName);
            ptQuery = "select b.* from CustomerUserMapping a join Customer b on a.CustomerId = b.Id where a.UserId=" + userId.ToString();
            var data = ent.Database.SqlQuery<Customer>(ptQuery).ToList().Where(a => a.City_Id == cityId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRentalTypeByPackageCustomerID(int packageId,int custId)
        {
            //var data = ent.RentalTypes.Where(a => a.PackageType_Id == packageId).ToList();
            string ptQuery = String.Empty;
            if (custId == 0)
            {
                ptQuery = "select * from RentalType where PackageType_Id="+packageId;
            }
            else 
            {
                var clint = ent.Customers.Where(a => a.Id == custId).FirstOrDefault();
                int custnewId = custId;
                if (clint.ParentCustomer_Id!=null)
                {
                    custnewId =Convert.ToInt32(clint.ParentCustomer_Id);
                }
                ptQuery = @"select distinct  rt.*  from clientpackage cp
                join RentalType rt on cp.RentalType_Id=rt.Id
                left join PackageType pt on rt.PackageType_Id=pt.Id
                where pt.Id='" + packageId + "' and cp.Customer_Id='" + custnewId + "' order by rt.RentalTypeName";
            }
            
            var data = ent.Database.SqlQuery<RentalType>(ptQuery).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getWorkingDayInAMonth(DateTime woDate)
        {
            string query2 = @"execute [getWorkingDayOfMonth] @workingDate";
            var total = ent.Database.SqlQuery<int>(query2,               
                new SqlParameter("@workingDate", woDate == null ? (object)DBNull.Value : woDate.ToString("yyyy-MM-dd"))
              
                ).FirstOrDefault();
            return Json(total, JsonRequestBehavior.AllowGet);
        }
        public ActionResult geVehicleManufatureName(int modelId)
        {
            string query2 = @"select ManufactureName from VehicleModel where Id="+modelId;
            var result = ent.Database.SqlQuery<string>(query2).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getBookedCabByDate()
        {
            string query2 = @"select cm.CityName,vm.ModelName,count(b.BookingId) as BookingCount from  
Booking b join VehicleModel vm on b.VehicleModel_Id=vm.Id
join CityMaster cm on b.City_Id=cm.Id where BookingStatus<>3 and CONVERT(VARCHAR, b.PickupDate, 101)=CONVERT(VARCHAR, DATEADD(DAY,1, GETDATE()), 101)
 group by cm.CityName,vm.ModelName";
            var result = ent.Database.SqlQuery<RequiredCab>(query2).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}