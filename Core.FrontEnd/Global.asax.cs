using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Logs;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce;
using DomainDrivenDesign.CorePermission;
using DomainDrivenDesign.CoreUserMessage;
using MySql.Data.Entity;

namespace Core.FrontEnd
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static readonly bool ForceSsl = System.Configuration.ConfigurationManager.AppSettings["ForceSsl"] != null
            && bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ForceSsl"]);

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

           // new CreateDatabaseIfNotExists<DomainDrivenDesign.CoreEcommerce.Ef.CoreEcommerceDbContext>().InitializeDatabase(new DomainDrivenDesign.CoreEcommerce.Ef.CoreEcommerceDbContext());

            MemoryMessageBuss.AutoRegister();

            DomainDrivenDesign.Core.EngineeCurrentContext.Init();

            EngineePermission.Init();

            EngineeEcommerce.Init();

            EngineeEmailSender.Init();

            EngineeCommandWorkerQueue.Init();

            UnhandleExceptionLogs.Log("INIT-DONE-NON-ERROR");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                if (e.ExceptionObject != null)
                {
                    var ex = e.ExceptionObject as Exception;
                    if (ex != null)
                    {
                        var errMsg = ex.GetMessages();

                        UnhandleExceptionLogs.Log(errMsg);
                    }
                }
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex= Server.GetLastError();
            if (ex != null)
            {
                var errMsg = ex.GetMessages();

                UnhandleExceptionLogs.Log(errMsg);
                Server.ClearError();
            }
        }
        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            MemoryMessageBuss.RegisterAssembly(args.LoadedAssembly);
            EngineeEcommerce.RegisterPaymentMethodPlugin(args.LoadedAssembly);
            EngineeEcommerce.RegisterShippingMethodPlugin(args.LoadedAssembly);
            EngineeEcommerce.RegisterVouchẻMethodPlugin(args.LoadedAssembly);
        }

        void Application_BeginRequest()
        {
            if (!ForceSsl || Context.Request.IsSecureConnection) return;

            var url = Context.Request.Url.ToString();
            Response.Redirect(url.Replace("http:", "https:"));
        }
    }
}
