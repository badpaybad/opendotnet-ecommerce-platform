using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminVoucherController : AdminBaseController
    {

        public ActionResult Index()
        {
            var model = new AdminPageList<FeVoucherMethod>();
            List<ContentLanguage> contentLanguages;
            using (var db = new CoreEcommerceDbContext())
            {
                model.Items = db.VoucherMethods.Select(i => new FeVoucherMethod()
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

            MemoryMessageBuss.PushCommand(new UpdateVocherMethod(id, description, LanguageId, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ListCode(Guid? methodId)
        {
            var model = new AdminManageVoucherCodePage();
            model.SelectedMethodId = methodId;
            var voucherMethods = GetVoucherMethods();

            model.VoucherMethods = voucherMethods;
            return View(model);
        }

        private static List<FeShoppingCartCheckoutPage.FeIdAndDescription> GetVoucherMethods()
        {
            return CacheManager.GetOrSetIfNull("adminGetVoucherMethod", () =>
            {
                List<ContentLanguage> contentLangs;
                List<FeShoppingCartCheckoutPage.FeIdAndDescription> voucherMethods;
                using (var db = new CoreEcommerceDbContext())
                {
                    voucherMethods = db.VoucherMethods
                        .Select(i => new FeShoppingCartCheckoutPage.FeIdAndDescription() { Id = i.Id, Name = i.Name })
                        .ToList();
                    var ids = voucherMethods.Select(i => i.Id).ToList();
                    contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
                }
                foreach (var m in voucherMethods)
                {
                    m.Description = contentLangs.GetValue(m.Id, "Description");
                }
                return voucherMethods;
            });

        }

        public JsonResult DeleteVoucherCode(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                MemoryMessageBuss.PushCommand(new DeleteVoucherCode(id, UserSessionContext.CurrentUserId(), DateTime.Now));
            }

            return Json(new { Ok = true, Data = new { Ids = ids }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateVoucherCode(int quantity, long codeValue, Guid voucherMethodId)
        {
            Dictionary<Guid, string> codes = new Dictionary<Guid, string>();
            for (int i = 0; i < quantity; i++)
            {
                codes.Add(Guid.NewGuid(), VoucherCodeServices.GenerateCode());
            }
            var currentUserId = UserSessionContext.CurrentUserId();
            var createdDate = DateTime.Now;

            foreach (var code in codes)
            {
                MemoryMessageBuss.PushCommand(new CreateVoucherCode(code.Key, code.Value, codeValue, voucherMethodId, currentUserId, createdDate));
            }
            return Json(new { Ok = true, Data = new { Codes = codes.Select(i => new { Id = i.Key, Code = i.Value }) }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List(Guid? methodId, string voucherKeywords, string userKeywords, string orderKeywords,
            int? skip, int? take, string sortField, string orderBy
            //, DateTime? fromDate, DateTime? toDate
            , bool? isUsed)
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

            List<AdminManageVoucherCodePage.VoucherCodeDisplay> rows = new List<AdminManageVoucherCodePage.VoucherCodeDisplay>();
            var voucherMethods = GetVoucherMethods();

            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.VoucherCodes.Select(i => new AdminManageVoucherCodePage.VoucherCodeDisplay()
                {
                    Id = i.Id,
                    Value = i.Value,
                    CreatedDate = i.CreatedDate,
                    Code = i.Code,
                    AppliedForOrderCode = i.AppliedForOrderCode,
                    VoucherMethodId = i.VoucherMethodId,
                    Applied = i.Applied,
                    AppliedForUserId = i.AppliedForUserId
                });
                if (!string.IsNullOrEmpty(userKeywords))
                {
                    query = query.Join(db.Users, v => v.AppliedForUserId, u => u.Id, (v, u) => new { V = v, U = u })
                        .Where(m => m.U.Username.Contains(userKeywords)
                || m.U.Email.Contains(userKeywords)
                || m.U.Phone.Contains(userKeywords)).Select(i => i.V);
                }

                if (methodId != null)
                {
                    var xmid = methodId.Value;
                    query = query.Where(i => i.VoucherMethodId == xmid);
                }
                if (!string.IsNullOrEmpty(voucherKeywords))
                {
                    query = query.Where(i => i.Code.Contains(voucherKeywords));
                }
                if (!string.IsNullOrEmpty(orderKeywords))
                {
                    query = query.Where(i => i.AppliedForOrderCode.Contains(orderKeywords));
                }
                if (isUsed != null)
                {
                    var xstatus = isUsed.Value;
                    query = query.Where(i => i.Applied == xstatus);
                }
                query = query.Distinct();

                total = query.LongCount();
                rows = query.OrderByDescending(i => i.Code)
                    .Skip(xskip).Take(xtake)
                    .ToList();
            }

            foreach (var r in rows)
            {
                var temp = voucherMethods.FirstOrDefault(i => i.Id == r.VoucherMethodId) ??
                           new FeShoppingCartCheckoutPage.FeIdAndDescription();
                r.VoucherMethodName = temp.Name;
                r.VoucherMethodDescription = temp.Description;
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}