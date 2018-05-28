using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Controllers
{
    public class CmsBaseController : Controller
    {
        public Guid LanguageId
        {
            get { return DomainDrivenDesign.Core.EngineeCurrentContext.LanguageId; }
        }

        public string CurrentIpAddress
        {
            get { return HttpHelper.GetIpAddress(System.Web.HttpContext.Current); }
        }

        public string SiteDomainUrl
        {
            get { return HttpHelper.GetRootWeb(); }
        }
    }
}