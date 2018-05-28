using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Core.FrontEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            routes.MapRoute(
                name: "CategoryUrlFriendly_frontend",
                url: "category/{urlsegment}",
                defaults: new { controller = "Category", action = "Index", urlsegment = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "NewsUrlFriendly_frontend",
                url: "news/{urlsegment}",
                defaults: new { controller = "News", action = "Detail", urlsegment = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductUrlFriendly_frontend",
                url: "product/{urlsegment}",
                defaults: new { controller = "Product", action = "Detail", urlsegment = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default_frontend",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );

        
        }
    }
}
