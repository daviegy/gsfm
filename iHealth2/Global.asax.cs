using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Web.Helpers;
using System.Data.Entity.SqlServer;
using Microsoft.SqlServer.Types;

namespace iHealth2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // private BackgroundJobServer _backgroundJobServer;
        protected void Application_Start()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            SqlProviderServices.SqlServerTypesAssemblyName = typeof(SqlGeography).Assembly.FullName;

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbContext, Configuration>());
            //new UsersContext().Database.Initialize(false); 

            //var migrator = new DbMigrator(new Migrations.Configuration());
            //migrator.Update();
            //GlobalConfiguration.Configuration
            //    .UseSqlServerStorage("ihealthCon");

            //_backgroundJobServer = new BackgroundJobServer();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpException ex = Server.GetLastError() as HttpException;
            Exception lastError = ex;
            if (ex.InnerException != null)
            {
                lastError = ex.InnerException;
                //Response.Clear();
                //Server.ClearError(); //make sure you log the exception first
                //Response.Redirect("~/Home/Index", true);
            }
            string lastErrorTypeName = lastError.GetType().ToString();
            string lastErrorMessage = lastError.Message;
            string lastErrorStackTrace = lastError.StackTrace;
        }
        //protected void Application_End(object sender, EventArgs e)
        //{
        //    _backgroundJobServer.Dispose();
        //}
    }
}
