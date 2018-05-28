using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    [AdminLoginRequired()]
    public class AdminBaseController : Controller
    {
        public  Guid LanguageId
        {
            get { return DomainDrivenDesign.Core.EngineeCurrentContext.LanguageId; }
        }

        public Guid CurrentUserId { get { return UserSessionContext.CurrentUserId(); } }

        public string SiteDomainUrl
        {
            get { return HttpHelper.GetRootWeb(); }
        }
    }
}