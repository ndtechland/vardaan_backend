using System.Web.Http;
using Unity;
using Unity.WebApi;
using Vardaan.Services.IContractApi;
using Vardaan.Services.ImplementationApi;

namespace VardaanCab.APP
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
             container.RegisterType<ICommon, CommonImplementation>();
             container.RegisterType<IDriver, DriverImplementation>();
             container.RegisterType<IEmployee, EmployeeImplementation>();
             container.RegisterType<IAdminAccount, AdminAccountImplementation>();
             container.RegisterType<IAdmin, AdminImplementation>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}