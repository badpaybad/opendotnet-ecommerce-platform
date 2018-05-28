using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminPaymentController : AdminBaseController
    {

        public ActionResult Index()
        {
            var model = new AdminPageList<FePaymentMethod>();
            List<ContentLanguage> contentLanguages;
            using (var db = new CoreEcommerceDbContext())
            {
                model.Items = db.PaymentMethods.Select(i => new FePaymentMethod()
                {
                    Id = i.Id,
                    Name = i.Name,
                    AssemblyType = i.AssemblyType
                }).ToList();
                var ids = model.Items.Select(i => i.Id).ToList();
                contentLanguages = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }
            foreach (var itm in model.Items)
            {
                itm.Description = contentLanguages.GetValue(itm.Id, "Description");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateDescription(string description, Guid id)
        {

            MemoryMessageBuss.PushCommand(new UpdatePaymentMethod(id, description, LanguageId, CurrentUserId, DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
    }
}