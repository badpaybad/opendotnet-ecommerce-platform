using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class UserEmailMessageController : AdminBaseController
    {
        // GET: Admin/UserEmailMessage
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List(string keywords,
            int? skip, int? take, string sortField, string orderBy
            , int? status)
        {
            var xtake = 10;
            var xskip = 0;
            long total = 0;
            if (skip != null)
            {
                xskip = skip.Value;
            }
            if (take != null)
            {
                xtake = take.Value;
            }

            List<UserMessageTransaction> rows = new List<UserMessageTransaction>();
            var typeEmail = (short) Enums.UserMessageType.Email;

            Func<UserMessageTransaction, bool> predicate = i => i.Type==typeEmail;
            if (!string.IsNullOrEmpty(keywords))
            {
                predicate = predicate.And(i => i.To.Contains(keywords) || i.Subject.Contains(keywords)
                || i.ToName.Contains(keywords));
            }
            if (status != null && status >= 0)
            {
                predicate = predicate.And(i => i.Status == status);
            }
            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.UserMessageTransactions.Where(predicate);
                total = query.LongCount();
                rows= query.OrderByDescending(i=>i.SendDate).Skip(xskip).Take(xtake).ToList() ;
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Resend(Guid id)
        {
            MemoryMessageBuss.PushCommand(new ResendUserMessage(id, CurrentUserId,DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
    }
}