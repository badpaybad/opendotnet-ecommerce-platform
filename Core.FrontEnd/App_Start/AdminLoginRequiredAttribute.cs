using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd
{
    public class LoginRequireAttribute : ActionFilterAttribute
    {
        private bool _returnTypeInJson=false;
        public LoginRequireAttribute(bool returnTypeInJson)
        {
            _returnTypeInJson = returnTypeInJson;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UserSessionContext.CurrentUser() == null || UserSessionContext.CurrentUserId() == Guid.Empty)
            {
                HttpContextBase context = filterContext.HttpContext;
                var url = context.Request.Url.ToString().ToLower();

                var customerLoginUrl = UserSessionContext.UrlFrontEndLogin + "?url=" + HttpUtility.UrlEncode(url);

                if (_returnTypeInJson)
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new {Message = "Require logedin : " +url},
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult(customerLoginUrl);
                }
            }

            base.OnActionExecuting(filterContext);
        }


    }

    public class AdminLoginRequiredAttribute : ActionFilterAttribute
    {
        private readonly bool _allowAnonymous;

        public AdminLoginRequiredAttribute(bool allowAnonymous = false)
        {
            _allowAnonymous = allowAnonymous;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_allowAnonymous)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (UserSessionContext.CurrentUserIsSysAdmin() == true)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            HttpContextBase context = filterContext.HttpContext;
            var url = context.Request.Url.ToString().ToLower();

            var customerLoginUrl = UserSessionContext.UrlAdminLogin + "?url=" + HttpUtility.UrlEncode(url);

            if (UserSessionContext.CurrentUser() == null)
            {
                filterContext.Result = new RedirectResult(customerLoginUrl);
            }
            else
            {
                // check url routing to match with rights
                var controller = filterContext.RouteData.GetRequiredString("controller");
                var action = filterContext.RouteData.GetRequiredString("action");

                var keyName = $"/{controller}/{action}/";

                var rights = UserSessionContext.CurrentUserRights();

                var found = rights.FirstOrDefault(i => i.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (found == null)
                {
                    var xxx = UserSessionContext.ListAllRights();
                    Right x;
                    if (!xxx.TryGetValue(keyName.ToLower(), out x))
                    {
                        filterContext.Result = new RedirectResult(customerLoginUrl);
                    }
                    else
                    {
                        if (x.ReturnType.Equals(typeof(JsonResult).FullName, StringComparison.OrdinalIgnoreCase))
                        {
                            filterContext.Result = new JsonResult()
                            {
                                Data = new { Message = "Require logedin : "+url }
                                ,
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(customerLoginUrl);
                        }
                    }
                }

            }

            base.OnActionExecuting(filterContext);
        }


    }
}