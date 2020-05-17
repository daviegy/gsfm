using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using iHealth2.CustomClasses;
namespace iHealth2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Add("BlogRoute", new SeoFriendlyRoute("Blog/post_details/{id}",
           new RouteValueDictionary(new { controller = "Blog", action = "post_details" }),
           new MvcRouteHandler()));
            routes.Add("businessRoute", new SeoFriendlyRoute("{custom_name}/Business/biz_profile/{id}",
         new RouteValueDictionary(new {custom_name = "all", controller = "Business", action = "biz_profile" }),
         new MvcRouteHandler()));
           //routes.Add("BusinessRoute", "{{Custom_Name}/Business/biz_profile/{id}}");
            //routes.MapRoute(
            //    name: "BusinessRoute",
            //    url: "{Custom_Name}/{controller}/{action}/{id}",
            //    defaults: new
            //    {
            //        controller = "Home",
            //        action = "Index",
            //        id = UrlParameter.Optional
            //    }
            //  );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
