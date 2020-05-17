using iHealth2.CustomClasses;
using System.Web;
using System.Web.Mvc;

namespace iHealth2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute { View = "ErrorAlert" });
            filters.Add(new ElmahMVCErrorFilter());
        }
    }
}
