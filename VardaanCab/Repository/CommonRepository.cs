using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;
using VardaanCab.Utilities;

namespace VardaanCab.Repository
{
    public class CommonRepository
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        public List<RentalTypeDTO> GetRentalTypeList()
        {
            var data = ent.Database.SqlQuery<RentalTypeDTO>(@"select rt.*,pt.PackageTypeName from RentalType rt left join PackageType pt on rt.PackageType_Id=pt.Id").ToList();
            return data;
        }

        public List<Driver> GetDriver()
        {
            string query = @"select * from Driver where id not in(select driver_id from DriverLeave where Convert(date,getdate()) between Convert(date,DateFrom) and Convert(date,DateTo))
and IsAvailable=1 and IsActive=1 order by DriverName asc";
            var driver = ent.Database.SqlQuery<Driver>(query).ToList();
            return driver;
        }

        public string GetParentCategories(int categoryId)
        {
            string q = @"execute getAllParents @categoryId=" + categoryId;
            var data = ent.Database.SqlQuery<Category>(q).Select(a => a.CategoryName).ToList();
            string categories = string.Join("=>", data);
            return categories;
        }

        public IEnumerable<CategoryDTO> GetCategoryWithParents()
        {
            var categories = ent.Categories.ToList();
            var cats = new List<CategoryDTO>();
            foreach(var cat in categories)
            {
                var c = new CategoryDTO {
                    CategoryName=GetParentCategories(cat.Id),
                    Id=cat.Id,
                    ParentCategory_Id=cat.ParentCategory_Id
                };
                cats.Add(c);
            }
            return cats;
        }

        public string GenerateBookingId(string bookingType)
        {
            string bookingId;
            string finStr = getCurrentFinYear();
            if (bookingType.ToLower() == "regular") 
            {
                bookingId = "VRC"+finStr+"-1";
                string query = @"select top 1 *,Convert(int,SUBSTRING(bookingid,7,LEN(bookingid)))               
as BookingNo from booking  where bookingtype='regular' and FinYear="+finStr+" order by Convert(int, SUBSTRING(bookingid, 7, LEN(bookingid))) desc";
                var lRecord = ent.Database.SqlQuery<BookingDTO>(query).FirstOrDefault();
                // var lRecord = ent.Bookings.OrderByDescending(a => a.Id).Where(a => a.BookingId != null && a.BookingType== "regular").FirstOrDefault();
                if (lRecord != null)
                {
                    int length = lRecord.BookingId.Length;
                    string iPart = lRecord.BookingId.Substring(6, length - 6);
                    int new_i = int.Parse(iPart) + 1;
                    bookingId = "VRC"+finStr+"-" + new_i;
                }
            }
            else
            {
                bookingId = "NRD"+finStr+"-1";
                string query1 = @"select top 1 *,Convert(int,SUBSTRING(bookingid,7,LEN(bookingid)))
as BookingNo from booking  where bookingtype='nrd' and FinYear="+finStr+" order by BookingNo desc";
                var lRecord = ent.Database.SqlQuery<BookingDTO>(query1).FirstOrDefault();
                //var lRecord = ent.Bookings.OrderByDescending(a => a.Id).Where(a => a.BookingId != null && a.BookingType == "nrd").FirstOrDefault();
                if (lRecord != null)
                {
                    int length = lRecord.BookingId.Length;
                    string iPart = lRecord.BookingId.Substring(6, length - 6);
                    int new_i = int.Parse(iPart) + 1;
                    bookingId = "NRD"+finStr+"-" + new_i;
                }
            }
            return bookingId;
        }

        public int GetUserDetailId()
        {
            int id =string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)?0: int.Parse(HttpContext.Current.User.Identity.Name);

            var user = ent.UserLogins.FirstOrDefault(a => a.Id == id);
            return user.Id;
        }

        public bool CreateLog(string table,string action)
        {
            try
            {
                int userId = GetUserDetailId();
                var user = ent.UserLogins.Find(userId);
                var log = new Log
                {
                    UserLogin_Id = user.Id,
                    UpdateDate = DateTime.Now,
                    Description = table+" is "+action+" by " + user.Email
                };
                ent.Logs.Add(log);
                ent.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<DiscountTypeModel> GetDiscountTypes()
        {
            var dt = new List<DiscountTypeModel> {
                new DiscountTypeModel { Text="Flat",Value="Flat"},
                new DiscountTypeModel { Text="Percentage",Value="Percentage"},
            };
            return dt;
        }

        public List<VendorPersonalPackageDTO> GetUnassignedVendorPackages(int vendorId)
        {
            string query = @"select vp.*,vp.Id as VendorPackage_Id,rt.RentalTypeName,pt.PackageTypeName,vm.ModelName from VendorPackage vp
join RentalType rt on vp.RentalType_Id=rt.Id
join VehicleModel vm on vp.VehicleModel_Id= vm.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
where vp.Id not in (select VendorPackage_Id from VendorPersonalPackage where Vendor_Id=" + vendorId + ")";
            var packages = ent.Database.SqlQuery<VendorPersonalPackageDTO>(query).ToList();
            return packages;
        }

        public List<VendorPersonalPackageDTO> GetAllVendorPackages()
        {
            string query = @"select vp.*,vp.Id as VendorPackage_Id,pt.PackageTypeName,rt.RentalTypeName,vm.ModelName from VendorPackage vp
join RentalType rt on vp.RentalType_Id=rt.Id
left join PackageType pt on rt.PackageType_Id=pt.Id
join VehicleModel vm on vp.VehicleModel_Id= vm.Id";
            var packages = ent.Database.SqlQuery<VendorPersonalPackageDTO>(query).OrderBy(a=>a.ModelName).ToList();
            return packages;
        }

        public List<StatusBooking> GetBookingStatus()
        {
            var bookingStatus = Enum.GetValues(typeof(BookingStatus)).Cast<BookingStatus>().ToList();
            var st = bookingStatus.Select(a => new StatusBooking { Id = (int)a, StatusName = a.ToString() }).ToList();
            return st;
        }

        public List<string> GetNonMenuLinks()
        {
            int userId = string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? 0 : int.Parse(HttpContext.Current.User.Identity.Name);
            var query = @"select Title from SoftwareLink where  IsMenu=0 and Id in (select SoftwareLink_Id from User_SoftwareLink where UserId=" + userId + ")";
            var data = ent.Database.SqlQuery<string>(query).ToList();
            return data;
        }

        public List<BookingDTO> GetAllBookingWithPType()
        {
            var que = @"select pt.PackageTypeName,b.* from Booking b
left join PackageType pt on b.PackageType_Id = pt.Id";
            var booking = ent.Database.SqlQuery<BookingDTO>(que).ToList();
            return booking;
        }

        public List<CabDTO> GetAvailableCabs()
        {
            var data = (from c in ent.Cabs
                        join vm in ent.VehicleModels
     on c.VehicleModel_Id equals vm.Id
                        join v in ent.Vendors on c.Vendor_Id equals v.Id into c_v
                        from cv in c_v.DefaultIfEmpty()
                        where c.IsAvailable && c.IsActive
                        orderby c.Id descending
                        select new CabDTO
                        {
                            Id = c.Id,
                            CreateDate = c.CreateDate,
                            Company = c.Company,
                            CompanyName = c.Company,
                            ModelName = vm.ModelName,
                            FitnessVality = c.FitnessVality,
                            RCDoc = c.RCDoc,
                            RcIssueDate = c.RcIssueDate,
                            RcValidity = c.RcValidity,
                            RcNumber = c.RcNumber,
                            PermitValidity = c.PermitValidity,
                            IsActive = c.IsActive,
                            FitnessDoc = c.FitnessDoc,
                            InsuranceDoc = c.InsuranceDoc,
                            InsurranceValidity = c.InsurranceValidity,
                            PolutionDoc = c.PolutionDoc,
                            PolutionValidity = c.PolutionValidity,
                            VehicleModel_Id = c.VehicleModel_Id,
                            VendorName = cv.CompanyName,
                            VehicleNumber = c.VehicleNumber,
                            Vendor_Id = c.Vendor_Id,
                            PermitDoc = c.PermitDoc,
                            PermitNo = c.PermitNo,
                            IsAvailable = c.IsAvailable,
                            FuelEfficiency = c.FuelEfficiency
                        }
                      ).ToList();
            return data;
        }

        public List<Driver> GetAvailableDrivers()
        {
            var data = ent.Drivers.Where(a => a.IsAvailable && a.IsActive).OrderBy(a => a.DriverName).ToList();
            return data;
        }

        public DateTime GetISTDate()
        {
            var istdate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
             TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }

        public string getCurrentFinYear()
        {
            string result = string.Empty;
            if (DateTime.Now.Month > 3)
            {
                result = DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length-2);
            }
            else if (DateTime.Now.Month <4)
            {
                result = DateTime.Now.AddYears(-1).Year.ToString().Substring(DateTime.Now.AddYears(-1).Year.ToString().Length - 2);
            }
            return result;
        }

    }
}