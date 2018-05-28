using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminSystemInfoController : AdminBaseController
    {
        public ActionResult AuditLogs()
        {
            return View();
        }

        public ActionResult ContactUsInfo()
        {
            return View();
        }

        public ActionResult SystemNotification()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult BroadCastMessage(string message, string type)
        {
            if (string.IsNullOrEmpty(type)) type = "info";

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();

            hubContext.Clients.All.boardCastMessage(new NotificationMessage()
            {
                DataType = "SystemBroadCast",
                DataJson = JsonConvert.SerializeObject(new { type = type, message = message })
            });

            var ok = true;
            return Json(new { Ok = ok, Data = new { }, Message = ok ? "Success" : "Fail" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListContactInfo(string keywords,
            int? skip, int? take, string sortField, string orderBy)
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
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(DomainDrivenDesign.Core.Implements.Models.ContactUsInfo.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            Expression<Func<ContactUsInfo, bool>> predicate = u => true;
            if (!string.IsNullOrEmpty(keywords))
            {
                predicate = predicate.And(i => i.FromPhone.Contains(keywords)
                || i.Title.Contains(keywords)
                || i.FromEmail.Contains(keywords)
                );
            }
            var rows = new List<ContactUsInfo>();

            using (var db = new CoreDbContext())
            {
                var contactUsInfos = db.ContactUsInfos.Where(predicate);
                total = contactUsInfos.LongCount();

                rows = contactUsInfos.OrderByDescending(i => i.CreatedDate)
                    .Skip(xskip).Take(xtake).ToList();
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegisterCommandsAndEvents()
        {
            //MemoryMessageBuss.AutoRegister();
            var model = new AdminSystemInfoPage();
            model.Commands = MemoryMessageBuss.GetCommands();
            model.Events = MemoryMessageBuss.GetEvents();
            return View(model);
        }

        [HttpPost]
        public JsonResult RegisterCommandsAndEventsAuto()
        {
            var ok = MemoryMessageBuss.AutoRegister();

            return Json(new { Ok = ok, Data = new { }, Message = ok ? "Success" : "Fail" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LanguageManagement()
        {
            return View();
        }

        public JsonResult ListLanguage(string keywords,
            int? skip, int? take, string sortField, string orderBy)
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
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(DomainDrivenDesign.Core.Implements.Models.ContactUsInfo.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            var rows = new List<Language>();
            using (var db=new CoreDbContext())
            {
                rows = db.Languages.ToList();
            }
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLanguage(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteLaguage(id, CurrentUserId,DateTime.Now));
            EngineeCurrentContext.RefreshListLanguage();

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLanguage(Guid id, string title, string code, string currencyCode, double currencyExchangeRate)
        {
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateLanguage(id, title,code, currencyCode, currencyExchangeRate, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateLanguage(id, title, code, currencyCode, currencyExchangeRate, CurrentUserId, DateTime.Now));
            }
           
            EngineeCurrentContext.RefreshListLanguage();

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListAuditLog( string keywords,
            int? skip, int? take, string sortField, string orderBy)
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
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(FeAuditLog.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            List<FeAuditLog> rows = new List<FeAuditLog>();

            Func<FeAuditLog, bool> predicate = m => true;
            if (!string.IsNullOrEmpty(keywords))
            {
                predicate = m => m.CommandType.Contains(keywords)|| m.CommandData.Contains(keywords)
                || m.Username.Contains(keywords)|| m.Email.Contains(keywords)|| m.Phone.Contains(keywords);
            }

            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.AuditLogs.Join(db.Users, a => a.UserId, u => u.Id, (a, u) => new {A = a, U = u})
                    .Select(m => new FeAuditLog()
                    {
                        Id=m.A.AlId,
                        CreatedDate=m.A.CreatedDate,
                        Username=m.U.Username,
                        Phone=m.U.Phone,
                        Email=m.U.Email,
                        UserId=m.U.Id,
                        CommandType=m.A.CommandType,
                        CommandData=m.A.CommandData
                    }).Where(predicate);

                total = query.LongCount();

                rows= query.OrderByDescending(i => i.CreatedDate).Skip(xskip).Take(xtake).ToList(); 
              
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}