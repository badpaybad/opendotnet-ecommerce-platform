using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Reflection;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;
using DomainDrivenDesign.CorePermission;
using Microsoft.Ajax.Utilities;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminOrderController : AdminBaseController
    {
        // GET: Admin/AdminOrder
        public ActionResult Index(short? status)
        {
            var model = new AdminOrderPage();
            
            model.Status = status;

            return View(model);
        }

        public ActionResult Detail(Guid orderId)
        {
            ShoppingCart cart;
            PaymentTransaction payTrans;
            List<ContentLanguage> contentLanguages;
            var model = new FeOrderPaymentPage();

            using (var db = new CoreEcommerceDbContext())
            {

                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == orderId);

                var cartPaymentMethodId = cart.PaymentMethodId;
                var cartShippingMethodId = cart.ShippingMethodId;
                
                model.Payment = db.PaymentMethods.Select(i => new FeShoppingCartCheckoutPage.FeIdAndDescription() { Id = i.Id, Name = i.Name })
                    .SingleOrDefault(i => i.Id == cartPaymentMethodId) ?? new FeShoppingCartCheckoutPage.FeIdAndDescription();
                model.Shipping = db.ShippingMethods.Select(i => new FeShoppingCartCheckoutPage.FeIdAndDescription() { Id = i.Id, Name = i.Name })
                    .SingleOrDefault(i => i.Id == cartShippingMethodId) ?? new FeShoppingCartCheckoutPage.FeIdAndDescription();

                model.Address = db.ShoppingCartShippingAddresses
                    .Where(i => i.ShoppingCartId == cart.Id).OrderByDescending(i => i.CreatedDate).FirstOrDefault()
                    ?? new ShoppingCartShippingAddress();

                model.OrderPromotion = db.OrderPromotions.Join(db.ShoppingCarts, pp => pp.Id, sc => sc.OrderPromotionId,
                        (pp, sc) => new { Pp = pp, Sc = sc })
                    .Select(m => new FeOrderPromotion()
                    {
                        Id = m.Pp.Id,
                        CreatedDate = m.Pp.CreatedDate,
                        Actived = m.Pp.Actived,
                        AmountToDiscount = m.Pp.AmountToDiscount,
                        DiscountAmount = m.Pp.DiscountAmount,
                        FreeShip = m.Pp.FreeShip,

                    }).FirstOrDefault();
                model.OrderPromotion = model.OrderPromotion ?? new FeOrderPromotion();

                contentLanguages = db.ContentLanguages
                    .Where(i => i.Id == cartPaymentMethodId || i.Id == cartShippingMethodId || i.Id == model.OrderPromotion.Id).ToList();
            }
            model.Order = cart;
            model.OrderPromotion.Description = contentLanguages.GetValue(model.OrderPromotion.Id, "Description");

            model.Shipping.Description = contentLanguages.GetValue(model.Shipping.Id, "Description");
            model.Payment.Description = contentLanguages.GetValue(model.Payment.Id, "Description");
            model.OrderPromotion.Description = contentLanguages.GetValue(model.OrderPromotion.Id, "Description");

            return View(model);
        }

        public ActionResult OrderListItem(string orderCode)
        {
            List<MonitorOrderItem> rows;
            List<ContentLanguage> contentLangs;
            using (var db = new CoreEcommerceDbContext())
            {
                rows = db.ShoppingCartItems.Join(db.Products, ci => ci.ProductId, p => p.Id, (ci, p) => new { Ci = ci, P = p })
                    .Join(db.ShoppingCarts,m=>m.Ci.ShoppingCartId, o=>o.Id,(m,o)=>new{m.Ci,m.P, O=o})
                    .Where(m => m.O.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase))
                    .Select(i => new MonitorOrderItem()
                    {
                        ProductCode = i.P.ProductCode,
                        ProductId = i.P.Id,
                        Quantity = i.Ci.Quantity
                    })
                    .ToList();
                var ids = rows.Select(i => i.ProductId).ToList();
                contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }
            foreach (var item in rows)
            {
                item.Title = contentLangs.GetValue(item.ProductId, "Title");
                item.UrlImage = contentLangs.GetValue(item.ProductId, "UrlImage");
            }
            //var total = rows.Count;

            return View(rows);
        }

        public JsonResult QueryDr(string orderCode)
        {
            PaymentTransaction pt;
            PaymentMethod pm;
            using (var db = new CoreEcommerceDbContext())
            {
                pt = db.PaymentTransactions
                    .SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
                pm = db.PaymentMethods.SingleOrDefault(i => i.Id == pt.PaymentMethodId);
            }

            if (pt == null || pm == null)
            {
                return Json(new { Ok = true, Data = new { Id = pt.Id }, Message = "Not found" }, JsonRequestBehavior.AllowGet);
            }

            var type = AssemblyExtesions.FindType(pm.AssemblyType);

            if (type == null) throw new Exception("Can not load assembly " + pm.AssemblyType);
            var p = Activator.CreateInstance(type) as IPaymentMethod;

            var result = p.QueryDr(pt.Id);

            return Json(new
            {
                Ok = result == Enums.ShoppingCartPayStatus.PaymentSuccess,
                Data = new { Id = pt.Id, Status = result }
            ,
                Message = result.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminConfirmOrder(Guid id)
        {
            MemoryMessageBuss.PushCommand(new ConfirmShoppingCartByAdmin(id, UserSessionContext.CurrentUsername()+ " call to customer to confirm", CurrentUserId, DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminCloseOrder(Guid id)
        {
            MemoryMessageBuss.PushCommand(new AdminCloseOrderForShoppingCart(id, UserSessionContext.CurrentUserId(), DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminCancelOrder(Guid id)
        {
            MemoryMessageBuss.PushCommand(new AdminCancelOrderForShoppingCart(id, UserSessionContext.CurrentUserId(), DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminConfirmPaySuccess(string orderCode)
        {
            Guid id;

            using (var db = new CoreEcommerceDbContext())
            {
                id = db.PaymentTransactions.Where(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase)).Select(i => i.Id)
                    .SingleOrDefault();
            }

            MemoryMessageBuss.PushCommand(new AdminSuccessPaymentTransaction(id, UserSessionContext.CurrentUserId(), DateTime.Now));
            return Json(new { Ok = true, Data = new { OrderCode = orderCode }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminConfirmPayFail(string orderCode)
        {
            Guid id;

            using (var db = new CoreEcommerceDbContext())
            {
                id = db.PaymentTransactions.Where(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase)).Select(i => i.Id)
                    .SingleOrDefault();
            }

            MemoryMessageBuss.PushCommand(new AdminFailPaymentTransaction(id, UserSessionContext.CurrentUserId(), DateTime.Now));
            return Json(new { Ok = true, Data = new { OrderCode = orderCode }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List(Guid? categoryId, string orderKeywords, string productKeywords,
            int? skip, int? take, string sortField, string orderBy
            , DateTime? fromDate, DateTime? toDate
            , int orderStatus, int payStatus, DateTime? rtFromDate, DateTime? rtToDate)
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

            List<AdminOrderPage.OrderItem> rows = new List<AdminOrderPage.OrderItem>();

            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.ShoppingCarts.Join(db.ShoppingCartShippingAddresses, c => c.Id, a => a.ShoppingCartId,
                        (c, a) => new { Order = c, Address = a })
                        .Join(db.PaymentMethods, m => m.Order.PaymentMethodId, p => p.Id
                        , (m, p) => new { m.Order, m.Address, Payment = p })
                    .Join(db.ShippingMethods, m => m.Order.ShippingMethodId, s => s.Id
                    , (m, s) => new { m.Order, m.Address, m.Payment, Shipping = s })
                    .Select(m => new AdminOrderPage.OrderItem()
                    {
                        Id = m.Order.Id,
                        CreatedDate = m.Order.CreatedDate,
                        ReceivingTime = m.Order.ReceivingTime,
                        OrderCode = m.Order.OrderCode,
                        ShippingMethodId = m.Order.ShippingMethodId,
                        ShippingMethodName = m.Shipping.Name,
                        PaymentMethodId = m.Order.PaymentMethodId,
                        PaymentMethodName = m.Payment.Name,
                        VoucherCode = m.Order.VoucherCode,
                        VoucherValue = m.Order.VoucherValue,
                        CartTotal = m.Order.CartTotal,
                        OrderStatusId = m.Order.Status,
                        ShippingFee = m.Order.ShippingFee,
                        CartSubTotal = m.Order.CartSubTotal,
                        CustomerName = m.Address.AddressName,
                        Note = m.Address.Message,
                        AddressLongitude = m.Address.AddressLongitude,
                        PayStatusId = m.Order.PaymentStatus,
                        ShipStatusId = m.Order.ShippingStatus,
                        PackingStatusId = m.Order.PackingStatus,
                        AddressLatitude = m.Address.AddressLatitude,
                        CustomerPhone = m.Address.Phone,
                        CartDiscount = m.Order.CartDiscount,
                        CustomerEmail = m.Address.Email,
                        CustomerAddress = m.Address.Address,

                    });
                if (fromDate != null)
                {
                    var xD = fromDate.Value.Date;
                    query = query.Where(i => i.CreatedDate >= xD);
                }
                if (toDate != null)
                {
                    var xD = toDate.Value.Date;
                    query = query.Where(i => i.CreatedDate <= xD);
                }
                if (orderStatus >= 0)
                {
                    query = query.Where(i => i.OrderStatusId == orderStatus);
                }
                if (payStatus >= 0)
                {
                    query = query.Where(i => i.PayStatusId == payStatus);
                }
                if (rtFromDate != null)
                {
                    var xD = rtFromDate.Value.Date;
                    query = query.Where(i => i.ReceivingTime >= xD);
                }
                if (rtToDate != null)
                {
                    var xD = rtToDate.Value.Date;
                    query = query.Where(i =>i.ReceivingTime <= xD);
                }
                if (string.IsNullOrEmpty(orderKeywords) == false)
                {
                    query = query.Where(i => i.CustomerAddress.Contains(orderKeywords)
                    || i.CustomerPhone.Contains(orderKeywords)
                    || i.CustomerName.Contains(orderKeywords)
                    || i.CustomerEmail.Contains(orderKeywords)
                    || i.OrderCode.Contains(orderKeywords)
                    || i.Note.Contains(orderKeywords)
                    || i.VoucherCode.Contains(orderKeywords)
                    );
                }

                if (string.IsNullOrEmpty(productKeywords) == false)
                {
                    query = query.Join(db.ShoppingCartItems, m => m.Id, sci => sci.ShoppingCartId,
                            (o, sci) => new { Order = o, CartItem = sci })
                           .Join(db.ContentLanguages, m => m.CartItem.ProductId, cl => cl.Id, (m, cl) => new { Order = m.Order, Cl = cl })
                           .Join(db.Products, m => m.Cl.Id, p => p.Id, (m, p) => new { Order = m.Order, Cl = m.Cl, P = p })
                           .Where(i => i.Cl.ColumnValue.Contains(productKeywords)
                           || i.P.ProductCode.Contains(productKeywords))
                        .Select(i => i.Order);
                }

                query = query.Distinct();

                total = query.LongCount();

                rows = query.OrderByDescending(i => i.ReceivingTime).ThenByDescending(i=>i.CreatedDate)
                    .Skip(xskip).Take(xtake)
                    .ToList();
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}