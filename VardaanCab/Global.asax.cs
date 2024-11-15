using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using orps.StartupTask;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Models;

namespace VardaanCab
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutomapperTask.RegisterAutomapper();
            JobScheduler.Start();
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string loginId = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        using (Vardaan_AdminEntities ent = new Vardaan_AdminEntities())
                        {
                            var user = ent.UserLogins.Find(int.Parse(loginId));
                            if(user!=null)
                            {
                                roles = user.Role;
                            }
                            else 
                            {
                            var cuser= ent.ClientUsers.Find(int.Parse(loginId));
                                roles = cuser.Role;
                            }
                            
                        }
                        //let us extract the roles from our own custom cookie


                        //Let us set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                          new System.Security.Principal.GenericIdentity(loginId, "Forms"), roles.Split(';'));
                    }
                    catch (Exception ex)
                    {
                        //somehting went wrong
                    }
                }
            }
        }

    }
}