using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminPackingAndLabelController : AdminBaseController
    {
        // GET: Admin/AdminPackingAndLabel
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PrintDetail(Guid orderId)
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
        public JsonResult ShipSuccess(string orderCode, string note)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode,StringComparison.OrdinalIgnoreCase));
            }

            MemoryMessageBuss.PushCommand(new UpdateShippingStatusForShoppingCart(cart.Id,Enums.ShoppingCartShipStatus.ShippingSuccess, note, CurrentUserId, DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = cart.Id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShipFail(string orderCode, string note)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
            }

            MemoryMessageBuss.PushCommand(new UpdateShippingStatusForShoppingCart(cart.Id, Enums.ShoppingCartShipStatus.ShippingFail, note, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = cart.Id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckOrderCode(string orderCode)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
            }

            var cartId = cart?.Id ?? Guid.Empty;

            return Json(new { Ok = cart!=null, Data = new { Id =cartId }, Message =cart!=null? "Ok":"Not existed" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfrimPrint(Guid orderId)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == orderId);
            }
            MemoryMessageBuss.PushCommand(new PackAndPrintShoppingCart(orderId,cart.OrderCode, CurrentUserId, DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = orderId }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult List(Guid? categoryId, string orderKeywords, string productKeywords,
            int? skip, int? take, string sortField, string orderBy
            , DateTime? fromDate, DateTime? toDate
            , int? orderStatus, int? payStatus, DateTime? rtFromDate, DateTime? rtToDate)
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

            orderStatus = (short) Enums.ShoppingCartStatus.OrderConfirmed;
            payStatus = (short) Enums.ShoppingCartPayStatus.PaymentSuccess;
            //var packingDone = (short) Enums.ShoppingCartPackingStatus.PackingDone;
            List<AdminOrderPage.OrderItem> rows = new List<AdminOrderPage.OrderItem>();

            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.ShoppingCarts//.Where(i=>i.PackingStatus!=packingDone)
                    .Join(db.ShoppingCartShippingAddresses, c => c.Id, a => a.ShoppingCartId,
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
                    var xD = fromDate.Value;
                    query = query.Where(i => i.CreatedDate >= xD);
                }
                if (toDate != null)
                {
                    var xD = toDate.Value;
                    query = query.Where(i => i.CreatedDate <= xD);
                }
                if (orderStatus!=null && orderStatus >= 0)
                {
                    query = query.Where(i => i.OrderStatusId == orderStatus);
                }
                if (payStatus!=null &&payStatus >= 0)
                {
                    query = query.Where(i => i.PayStatusId == payStatus);   
                }
                if (rtFromDate != null)
                {
                    var xD = rtFromDate.Value;
                    query = query.Where(i => i.ReceivingTime >= xD);
                }
                if (rtToDate != null)
                {
                    var xD = rtToDate.Value;
                    query = query.Where(i => i.ReceivingTime <= xD);
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

                rows = query.OrderByDescending(i => i.ReceivingTime).ThenByDescending(i => i.CreatedDate)
                    .Skip(xskip).Take(xtake)
                    .ToList();
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }


    }
}