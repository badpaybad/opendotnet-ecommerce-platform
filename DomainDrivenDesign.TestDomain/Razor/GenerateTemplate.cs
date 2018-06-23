using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.Razor.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDrivenDesign.TestDomain.Razor
{
    [TestClass]
    public class GenerateTemplate
    {
        [TestMethod]
        public void GetHtmlFromTemplate()
        {
            string template = "<table>" +
                              "<tr><td>Name</td><td>Time</td></tr>" +
                              "<tr datanameloop style='width:123px;'><td>{{Name}}</td><td>{{Time}}</tr>" +
                              "</table>";

            List<dynamic> data = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                data.Add(new { Name = "hello " + i, Time = DateTime.Now.AddMinutes(i) });
            }

            var regexforNameLoop = new Regex("<tr datanameloop [^>](.+?)<\\/tr>");

            if (regexforNameLoop.IsMatch(template))
            {
                var firstMatch = regexforNameLoop.Match(template).Value;
               
                var rows = "";
                foreach (var d in data)
                {
                    var row = firstMatch;
                    row = row.Replace("{{Name}}", d.Name.ToString());
                    row = row.Replace("{{Time}}", d.Time.ToString());
                    rows = rows + row;
                }

                template = template.Replace(firstMatch, rows);
            }

        }

    }
    
    //public ActionResult ActionName (CustomRequest request) {

    //}

    //[ModelBinder(typeof(CustomModelBinder))]
    //public class CustomRequest
    //{
    //    public bool itemOnly;
    //    public string[] addedArticles;
    //}

    //public class CustomModelBinder : IModelBinder
    //{
    //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        var request = controllerContext.RequestContext.HttpContext.Request;
    //        var itemOnly =bool.Parse( request.QueryString["itemOnly"]);
    //        var addedArticles = request.QueryString["addedArticles"].Split(',');

    //        return new CustomRequest(){itemOnly=itemOnly,addedArticles= addedArticles};
    //    }
    //}
}
