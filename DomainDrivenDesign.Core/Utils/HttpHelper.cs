using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DomainDrivenDesign.Core.Utils
{
    public class HttpHelper
    {
        public static string GetRootWeb()
        {

            var domain = HttpContext.Current.Request.Url.Host.Trim('/');

            if (domain.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0 ||
                domain.IndexOf("127.0.0.1", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                domain = domain + ":" + HttpContext.Current.Request.Url.Port;
            }

            return "http://" + domain.Trim('/');

            //try
            //{
            //    var r = "";
            //    var cr = HttpContext.Current.Request;
            //    if (cr.Url.Port != 80)
            //    {
            //        r = cr.Url.Scheme + "://" + cr.Url.Host + ":" + cr.Url.Port.ToString();
            //    }
            //    else
            //    {
            //        r = cr.Url.Scheme + "://" + cr.Url.Host;
            //    }
            //    //if (cr.Url.ToString().IndexOf("local", StringComparison.OrdinalIgnoreCase) >= 0)
            //    //{
            //    //    if (System.Web.HttpContext.Current.Request.ApplicationPath != null)
            //    //        r = r.Trim('/') + "/" + System.Web.HttpContext.Current.Request.ApplicationPath.Trim('/');
            //    //}
            //    r = r.Trim('/') + "/";
            //    return r.ToLower();
            //}
            //catch
            //{
            //    return "/";
            //}
        }

        public static string GetIpAddress(HttpContext context)
        {
            if(context==null) context= HttpContext.Current;

            var forwardedFor = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            var userIpAddress = String.IsNullOrWhiteSpace(forwardedFor)
                ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                : forwardedFor.Split(',').Select(s => s.Trim()).First();

            return userIpAddress;
        }
    }
}
