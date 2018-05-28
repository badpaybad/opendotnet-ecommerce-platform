//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web;
//using DomainDrivenDesign.Core.Implements;
//using DomainDrivenDesign.Core.Implements.Models;
//using DomainDrivenDesign.Core.Utils;

//namespace Core.FrontEnd
//{
//    /// <summary>
//    /// Summary description for FriendlyUrlRewrite
//    /// </summary>
//    public class FriendlyUrlRewriter : IHttpModule
//    {

//        public FriendlyUrlRewriter()
//        {
//            //
//            // TODO: Add constructor logic here
//            //
//        }

//        public void Init(HttpApplication context)
//        {
//            context.BeginRequest += OnBeginRequest;
//        }

//        private void OnBeginRequest(object sender, EventArgs e)
//        {
//            try
//            {
//                var app = sender as HttpApplication;
//                if (app != null)
//                {
//                    var rootWeb = HttpHelper.GetRootWeb();

//                    var url = app.Context.Request.Url.ToString();
//                    url = url.Replace(rootWeb, "").Trim('/');
//                    UrlFriendly temp = null;
//                    using (var db = new FidgaDbContext())
//                    {
//                        temp = db.UrlFriendlys.FirstOrDefault(
//                           i => i.UrlSegment.Equals(url, StringComparison.OrdinalIgnoreCase));
//                    }
//                    if (temp != null)
//                    {
//                        app.Context.RewritePath($"/{temp.ControllerName}/{temp.ActionName}/{temp.Id}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void Dispose()
//        {

//        }
//    }


//}