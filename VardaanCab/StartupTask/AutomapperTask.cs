using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace orps.StartupTask
{
    public class AutomapperTask
    {
        public static void RegisterAutomapper()
        {
            Mapper.CreateMap<StateMaster, StateMasterDTO>();
            Mapper.CreateMap<StateMasterDTO, StateMaster>();

            Mapper.CreateMap<CityMaster, CityMasterDTO>();
            Mapper.CreateMap<CityMasterDTO, CityMaster>();

            Mapper.CreateMap<Category, CategoryDTO>();
            Mapper.CreateMap<CategoryDTO, Category>();

            Mapper.CreateMap<MopMaster, MopMasterDTO>();
            Mapper.CreateMap<MopMasterDTO, MopMaster>();

            Mapper.CreateMap<CabCategory, CabCategoryDTO>();
            Mapper.CreateMap<CabCategoryDTO, CabCategory>();

            Mapper.CreateMap<Vendor, VendorDTO>();
            Mapper.CreateMap<VendorDTO, Vendor>();

            Mapper.CreateMap<Driver, DriverDTO>();
            Mapper.CreateMap<DriverDTO, Driver>();

            Mapper.CreateMap<Cab, CabDTO>();
            Mapper.CreateMap<CabDTO, Cab>();

            Mapper.CreateMap<Customer, CustomerDTO>();
            Mapper.CreateMap<CustomerDTO, Customer>();

            Mapper.CreateMap<Booking, BookingDTO>();
            Mapper.CreateMap<BookingDTO, Booking>();
            Mapper.CreateMap<Booking, ScheduleBookingDTO>();
            Mapper.CreateMap<ScheduleBookingDTO, Booking>();
            Mapper.CreateMap<CorporatePackage, CorporatePackageDTO>();
            Mapper.CreateMap<CorporatePackageDTO, CorporatePackage>();

            Mapper.CreateMap<NrgPackage, NrgPackageDTO>();
            Mapper.CreateMap<NrgPackageDTO, NrgPackage>();

            Mapper.CreateMap<VendorPackage, VendorPackageDTO>();
            Mapper.CreateMap<VendorPackageDTO, VendorPackage>();

            Mapper.CreateMap<CorporateBill, CorporateBillingDTO>();
            Mapper.CreateMap<CorporateBillingDTO, CorporateBill>();

            Mapper.CreateMap<VehicleModel, VehicleModelDTO>();
            Mapper.CreateMap<VehicleModelDTO, VehicleModel>();

            Mapper.CreateMap<StateWiseGSTIN, StateWiseGstinDTO>();
            Mapper.CreateMap<StateWiseGstinDTO, StateWiseGSTIN>();

            Mapper.CreateMap<StateWiseGSTIN, StateWiseGstinDTO>();
            Mapper.CreateMap<StateWiseGstinDTO, StateWiseGSTIN>();

            Mapper.CreateMap<RentalType, RentalTypeDTO>();
            Mapper.CreateMap<RentalTypeDTO, RentalType>();

            Mapper.CreateMap<UserLogin, UserLoginDTO>();
            Mapper.CreateMap<UserLoginDTO, UserLogin>();

            Mapper.CreateMap<UserLogin, RoleManageModel>();
            Mapper.CreateMap<RoleManageModel, UserLogin>();

            Mapper.CreateMap<VendorBill, CorporateBillingDTO>();
            Mapper.CreateMap<CorporateBillingDTO, VendorBill>();

            Mapper.CreateMap<ClientPackage, ClientPackageDTO>();
            Mapper.CreateMap<ClientPackageDTO, ClientPackage>();

            Mapper.CreateMap<VendorPersonalPackage, VendorPersonalPackageDTO>();
            Mapper.CreateMap<VendorPersonalPackageDTO, VendorPersonalPackage>();

            Mapper.CreateMap<DriverLeave, DriverLeaveDTO>();
            Mapper.CreateMap<DriverLeaveDTO, DriverLeave>();

            Mapper.CreateMap<MonthlyPackage, MonthlyPackageDTO>();
            Mapper.CreateMap<MonthlyPackageDTO, MonthlyPackage>();

            Mapper.CreateMap<MonthlyPackageEntry, MonthlyPackageEntryDTO>();
            Mapper.CreateMap<MonthlyPackageEntryDTO, MonthlyPackageEntry>();

            Mapper.CreateMap<MonthlyPackageBillDTO, MonthlyPackageBill>();
            Mapper.CreateMap<MonthlyPackageBill, MonthlyPackageBillDTO>();

            Mapper.CreateMap<MonthlyPackageRouteDTO, MonthlyPackageRoute>();
            Mapper.CreateMap<MonthlyPackageRoute, MonthlyPackageRouteDTO>();

            Mapper.CreateMap<MonthlyPackageRouteEntryDTO, MonthlyPackageRouteEntry>();
            Mapper.CreateMap<MonthlyPackageRouteEntry, MonthlyPackageRouteEntryDTO>();

            Mapper.CreateMap<MonthlyPackageRouteBill, MonthlyPackageRouteBillDTO>();
            Mapper.CreateMap<MonthlyPackageRouteBillDTO, MonthlyPackageRouteBill>();

            Mapper.CreateMap<MonthlyPackageMaster, MonthlyPackageMasterDTO>();
            Mapper.CreateMap<MonthlyPackageMasterDTO, MonthlyPackageMaster>();

            Mapper.CreateMap<MonthlyPackageRouteMaster, MonthlyPackageRouteMasterDTO>();
            Mapper.CreateMap<MonthlyPackageRouteMasterDTO, MonthlyPackageRouteMaster>();

            Mapper.CreateMap<ClientUser, ClientUserDTO>();
            Mapper.CreateMap<ClientUserDTO, ClientUser>();
        }
    }
}