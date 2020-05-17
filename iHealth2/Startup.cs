using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Dashboard;

[assembly: OwinStartupAttribute(typeof(iHealth2.Startup))]
namespace iHealth2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configuration
               .UseSqlServerStorage("ihealthJob");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            app.UseHangfireServer();
        }
    }
}
