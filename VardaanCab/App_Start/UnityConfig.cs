using System;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Vardaan.Services.IContract;
using Vardaan.Services.Implementation;

namespace VardaanCab
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IController, Controller>();
            container.RegisterType<IBanner, BannerImplementation>();
            container.RegisterType<IDepartment, DepartmentImplementation>();
            container.RegisterType<IEmployee, EmployeeImplementation>();
            container.RegisterType<IEmployeeMobAppSettings, EmployeeMobAppSettingsImplementation>();
            container.RegisterType<IEscort, EscortImplementation>();
            container.RegisterType<IETS, ETSImplementation>();
            container.RegisterType<IShift, ShiftImplementation>();
            container.RegisterType<IArea, AreaImplementation>();           
            container.RegisterType<ICompanyZone, CompanyZoneImplementation>();           
            container.RegisterType<ICustomer, CustomerImplementation>();           
            container.RegisterType<IAdministrator, AdministratorImplementation>();           
        }
    }
}