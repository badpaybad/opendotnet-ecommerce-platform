using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Events;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminMonitorController : AdminBaseController, IEventHandle<ShoppingCartConfirmedByCustomer>, IEventHandle<ShoppingCartUpdatedPayStatus>
    {
        // GET: Admin/AdminMonitor
        public ActionResult Index()
        {
            return View();
        }

        public void Handle(ShoppingCartConfirmedByCustomer e)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();

            hubContext.Clients.All.broadCastMonitorMessage(new NotificationMessage()
            {
                DataType = "MonitorBroadCastMessage",
                DataJson = JsonConvert.SerializeObject(new { type = "confirmedOrder", orderCode = e.OrderCode })
            });
        }

        public void Handle(ShoppingCartUpdatedPayStatus e)
        {
            if (e.PayStatus == (short) Enums.ShoppingCartPayStatus.PaymentSuccess)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();

                hubContext.Clients.All.broadCastMonitorMessage(new NotificationMessage()
                {
                    DataType = "MonitorBroadCastMessage",
                    DataJson = JsonConvert.SerializeObject(new { type = "refreshList", orderCode = e.OrderCode })
                });
            }
        }
    }
}