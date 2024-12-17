using System.Web.Http;
using Unity;
using Unity.WebApi;
using Vardaan.Services.IContract;
using Vardaan.Services.Implementation;

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
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}