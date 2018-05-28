using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.CorePermission;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminPageSideBarController : AdminBaseController
    {
        public JsonResult CmsCategoryTree()
        {
            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(true,LanguageId,new List<Enums.CategoryType>()
            {
                Enums.CategoryType.News,Enums.CategoryType.NewsAndProduct
            }, true,false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EcommerceCategoryTree()
        {
            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(false, LanguageId, new List<Enums.CategoryType>()
            {
                Enums.CategoryType.Product,Enums.CategoryType.NewsAndProduct
            }, true, false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}