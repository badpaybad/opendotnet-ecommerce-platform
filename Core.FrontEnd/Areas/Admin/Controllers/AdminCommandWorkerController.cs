using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminCommandWorkerController : AdminBaseController
    {
        // GET: Admin/CommandWorker
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AddPingData(string pingData, int? pingQuantity=1, bool? reset=false, bool? worker=false, string type="")
        {
            var xtype = typeof(PingWorker);
            if (!string.IsNullOrEmpty(type))
            {
                xtype = EngineeCommandWorkerQueue.GetType(type);
            }
            if (worker == true)
            {
                for (int i = 0; i < pingQuantity; i++)
                {
                    EngineeCommandWorkerQueue.AddAndStartWorker(xtype);
                }
            }
            else
            {
                if (reset == true)
                {
                    EngineeCommandWorkerQueue.ResetToOneWorker(xtype);
                }
                else
                {
                    for (int i = 0; i < pingQuantity; i++)
                    {
                        EngineeCommandWorkerQueue.Push(new PingWorker($"{i + 1}. {pingData}"));
                    }
                }
            }

            return Json(new { Ok = true, Data = new { }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatistic(string type)
        {
            var xtype = typeof(PingWorker);
            if (!string.IsNullOrEmpty(type))
            {
                xtype = EngineeCommandWorkerQueue.GetType(type);
            }
            int workerCount;
            int dataCount;
            EngineeCommandWorkerQueue.CountStatistic(xtype, out dataCount, out workerCount);
            return Json(new { Ok = true, Data = new { dataCount, workerCount }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List( string keywords,
            int? skip, int? take, string sortField, string orderBy)
        {
            var xtake = 10;
            var xskip = 0;
            long total = 0;

            var rows = EngineeCommandWorkerQueue.ListAllCommandName().Select(i=>new{Type=i}).ToList();
           
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}