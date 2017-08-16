using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HavaAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        //protected void Application_BeginRequest()
        //{
        //    string origin = HttpContext.Current.Request.Headers.Get("Origin");
        //    if (!string.IsNullOrEmpty(origin))
        //    {
        //        if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
        //        {
        //            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST,GET,OPTIONS,PUT,DELETE");
        //            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");
        //            HttpContext.Current.Response.End();
        //        }
        //        Response.Headers.Remove("Access-Control-Allow-Origin");

        //        Response.AddHeader("Access-Control-Allow-Origin", origin);

        //        Response.Headers.Remove("Access-Control-Allow-Credentials");
        //        Response.AddHeader("Access-Control-Allow-Credentials", "true");

        //        Response.Headers.Remove("Access-Control-Allow-Methods");
        //        Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        //    }


        //}
    }
}
