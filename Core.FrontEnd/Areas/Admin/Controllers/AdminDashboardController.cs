using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminDashboardController : AdminBaseController
    {
        public JsonResult OrderAndContactMsgCount()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            var confirmed = (short)Enums.ShoppingCartStatus.OrderConfirmed;
            var dnow = DateTime.Now.Date;
            var dateFeom = dnow.AddDays(-1);
            var toDate = dnow.AddDays(1);

            using (var db = new CoreEcommerceDbContext())
            {
                data["OrderConfirmedCount"] = db.ShoppingCarts.Count(i => i.Status == confirmed).ToString();
                data["ContactMessageTodayCount"] = db.ContactUsInfos.Count(i => i.CreatedDate > dateFeom && i.CreatedDate < toDate).ToString();
            }

            return Json(new { Ok = true, Data = data, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderStatusPieChart()
        {
            var osRes = new List<ChartKeyValueObject>();
            var psRes = new List<ChartKeyValueObject>();
            var shRes = new List<ChartKeyValueObject>();
            var opRes = new List<ChartKeyValueObject>();

            var orderStatus = new Dictionary<short, string>();
            orderStatus.Add((short)Enums.ShoppingCartStatus.OrderCanceled, Enums.ShoppingCartStatus.OrderCanceled.ToString());
            orderStatus.Add((short)Enums.ShoppingCartStatus.OrderClosed, Enums.ShoppingCartStatus.OrderClosed.ToString());
            orderStatus.Add((short)Enums.ShoppingCartStatus.OrderConfirmed, Enums.ShoppingCartStatus.OrderConfirmed.ToString());

            var payStatus = new Dictionary<short, string>();
            payStatus.Add((short)Enums.ShoppingCartPayStatus.PaymentCreated, Enums.ShoppingCartPayStatus.PaymentCreated.ToString());
            payStatus.Add((short)Enums.ShoppingCartPayStatus.PaymentFail, Enums.ShoppingCartPayStatus.PaymentFail.ToString());
            payStatus.Add((short)Enums.ShoppingCartPayStatus.PaymentSuccess, Enums.ShoppingCartPayStatus.PaymentSuccess.ToString());
            payStatus.Add((short)Enums.ShoppingCartPayStatus.PaymentProcess, Enums.ShoppingCartPayStatus.PaymentProcess.ToString());

            var shipStatus= new Dictionary<short, string>();
            shipStatus.Add((short)Enums.ShoppingCartShipStatus.ShippingCreated, Enums.ShoppingCartShipStatus.ShippingCreated.ToString());
            shipStatus.Add((short)Enums.ShoppingCartShipStatus.ShippingFail, Enums.ShoppingCartShipStatus.ShippingFail.ToString());
            shipStatus.Add((short)Enums.ShoppingCartShipStatus.ShippingSuccess, Enums.ShoppingCartShipStatus.ShippingSuccess.ToString());

            var cartVsOrder = new List<ChartKeyValueObject>();

            double orderCount = 0;
            double cartCount;
            double checkedOutCount;

            var oStatusNew = (short)Enums.ShoppingCartStatus.ShoppingCart;
            var oCheckedOut = (short)Enums.ShoppingCartStatus.Checkedout;

            using (var db = new CoreEcommerceDbContext())
            {
                orderCount = db.ShoppingCarts.Count(i => i.Status != oStatusNew && i.Status != oCheckedOut);
                cartCount = db.ShoppingCarts.Count(i => i.Status == oStatusNew);
                checkedOutCount = db.ShoppingCarts.Count(i => i.Status == oCheckedOut);

                foreach (var os in orderStatus)
                {
                    var count = db.ShoppingCarts.Count(i => i.Status == os.Key);
                    osRes.Add(new ChartKeyValueObject(os.Value + $" ({count})", count));
                }

                foreach (var ps in payStatus)
                {
                    var count = db.ShoppingCarts.Count(i => i.PaymentStatus == ps.Key && i.Status != oStatusNew && i.Status != oCheckedOut);
                    psRes.Add(new ChartKeyValueObject(ps.Value + $" ({count})", count));
                }

                foreach (var sh in shipStatus)
                {
                    var count = db.ShoppingCarts.Count(i => i.ShippingStatus == sh.Key && i.Status != oStatusNew && i.Status != oCheckedOut);
                    shRes.Add(new ChartKeyValueObject(sh.Value + $" ({count})", count));
                }

                foreach (var os in orderStatus)
                {
                    foreach (var ps in payStatus)
                    {
                        var count = db.ShoppingCarts.Count(i => i.Status == os.Key && i.PaymentStatus == ps.Key);
                        opRes.Add(new ChartKeyValueObject(os.Value + "_" + ps.Value + $" ({count})", count));
                    }
                }
            }

            double all;
            all = orderCount + cartCount + checkedOutCount;

            var percentO = Math.Round(((double)orderCount) / all, 2);
            var percentC = Math.Round(cartCount / all, 2);
            var percentCed = Math.Round(checkedOutCount / all, 2);

            cartVsOrder.Add(new ChartKeyValueObject($"Order ({orderCount})", percentO));
            cartVsOrder.Add(new ChartKeyValueObject($"ShoppingCart ({cartCount})", percentC));
            cartVsOrder.Add(new ChartKeyValueObject($"Checkedout ({checkedOutCount})", percentCed));

            foreach (var o in osRes)
            {
                o.Value = Math.Round(o.Value / orderCount, 2);
            }
            foreach (var o in psRes)
            {
                o.Value = Math.Round(o.Value / orderCount, 2);
            }
            foreach (var o in opRes)
            {
                o.Value = Math.Round(o.Value / orderCount, 2);
            }
            foreach (var o in shRes)
            {
                o.Value = Math.Round(o.Value / orderCount, 2);
            }

            return Json(new
            {
                Ok = true,
                Data = new
                {
                    OrderVsCart = cartVsOrder,
                    OrderStatus = osRes,
                    PayStatus = psRes,
                    ShippingStatus = shRes,
                    OrderAndPayStatus = opRes
                },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}