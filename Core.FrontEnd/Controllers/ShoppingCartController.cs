using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CorePermission;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Events;
using DomainDrivenDesign.CoreEcommerce.Services;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Core.FrontEnd.Controllers
{
    public class ShoppingCartController : CmsBaseController, IEventHandle<ShoppingCartCheckedout>
        , IEventHandle<ShoppingCartAddedProduct>, IEventHandle<ShoppingCartRemovedProduct>, IEventHandle<ShoppingCartRemovedAllProduct>
    {
        public Guid GetShoppingCartId()
        {
            var httpContext = System.Web.HttpContext.Current;
            var currentUserId = UserSessionContext.CurrentUserId();

            var sessionCart = httpContext.Session["shoppingcartid"];
            string temp = string.Empty;

            if (sessionCart != null)
            {
                temp = sessionCart.ToString();
            }

            Guid cartId;
            if (!string.IsNullOrEmpty(temp))
            {
                return Guid.Parse(temp);
            }

            cartId = Guid.NewGuid();
            httpContext.Session["shoppingcartid"] = cartId.ToString();
            MemoryMessageBuss.PushCommand(new CreateShoppingCart(cartId, currentUserId, LanguageId, CurrentIpAddress, SiteDomainUrl));

            return cartId;
        }
        public void SetShoppingCartId(Guid cartId)
        {
            var httpContext = System.Web.HttpContext.Current;
            httpContext.Session["shoppingcartid"] = cartId.ToString();
        }
        // GET: ShoppingCart
        public ActionResult Index(Guid? id)
        {
            var model = new FeShoppingCartPage();

            if (id != null && id.Value != Guid.Empty)
            {
                SetShoppingCartId(id.Value);
                model.Id = id.Value;
            }
            else
            {
                model.Id = GetShoppingCartId();
            }
            try
            {
                MemoryMessageBuss.PushCommand(new PreCalculateShoppingCart(model.Id));
            }
            catch (Exception ex)
            {
                model.ErrorMessage += ex.Message;
            }

            using (var db = new CoreEcommerceDbContext())
            {
                var cartItemsCount = db.ShoppingCartItems.Count(i => i.ShoppingCartId == model.Id);
                var cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == model.Id);

                model.CartItemCount = cartItemsCount;
                model.CartTotal = cart.CartTotal;
                model.CartDiscount = cart.CartDiscount;

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
                model.OrderPromotion.Description = db.ContentLanguages.GetValue(model.OrderPromotion.Id, "Description");
            }

            return View(model);
        }

        public ActionResult Checkout()
        {
            var model = new FeShoppingCartCheckoutPage();
            model.Id = GetShoppingCartId();
            List<ContentLanguage> contentLangs;
            using (var db = new CoreEcommerceDbContext())
            {
                var cartItemsCount = db.ShoppingCartItems.Count(i => i.ShoppingCartId == model.Id);
                var cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == model.Id);

                model.CartItemCount = cartItemsCount;
                model.CartTotal = cart.CartTotal;
                model.CartDiscount = cart.CartDiscount;

                model.PaymentMethods = db.PaymentMethods.Select(
                    i => new FeShoppingCartCheckoutPage.FeIdAndDescription()
                    {
                        Id = i.Id,
                        Name = i.Name
                    }).ToList();
                model.ShippingMethods = db.ShippingMethods.Select(
                    i => new FeShoppingCartCheckoutPage.FeIdAndDescription()
                    {
                        Id = i.Id,
                        Name = i.Name
                    }).ToList();

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

                var ids = model.PaymentMethods.Select(i => i.Id).ToList();
                ids.AddRange(model.ShippingMethods.Select(i => i.Id));
                ids.Add(model.OrderPromotion.Id);

                contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }

            model.OrderPromotion.Description = contentLangs.GetValue(model.OrderPromotion.Id, "Description");

            foreach (var p in model.PaymentMethods)
            {
                p.Description = contentLangs.GetValue(p.Id, "Description");
            }
            foreach (var s in model.ShippingMethods)
            {
                s.Description = contentLangs.GetValue(s.Id, "Description");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult ListResult(Guid id,
            int? skip, int? take, string sortField, string orderBy)
        {
            List<FeShoppingCartPage.Item> rows;
            List<ContentLanguage> contentLangs;
            using (var db = new CoreEcommerceDbContext())
            {
                rows = db.ShoppingCartItems.Join(db.Products, ci => ci.ProductId, p => p.Id, (ci, p) => new { Ci = ci, P = p })
                    .Where(i => i.Ci.ShoppingCartId == id)
                    .Select(i => new FeShoppingCartPage.Item()
                    {
                        ProductCode = i.P.ProductCode,
                        ProductId = i.P.Id,
                        Quantity = i.Ci.Quantity,
                        TotalPrice = i.Ci.TotalPrice,
                        UnitPrice = i.Ci.UnitPrice,
                        ProductPromotionId = i.Ci.ProductPromotionId
                    })
                    .ToList();
                var ids = rows.Select(i => i.ProductId).ToList();
                ids.AddRange(rows.Select(i => i.ProductPromotionId).ToList());
                contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }
            foreach (var item in rows)
            {
                item.Title = contentLangs.GetValue(item.ProductId, "Title");
                item.UrlImage = contentLangs.GetValue(item.ProductId, "UrlImage");
                item.SeoUrlFriendly = contentLangs.GetValue(item.ProductId, "SeoUrlFriendly");
                item.PromotionDescription = contentLangs.GetValue(item.ProductPromotionId, "Description");
            }
            var total = rows.Count;
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMiniCart()
        {
            var id = GetShoppingCartId();
            var totalQuantity = 0;
            using (var db = new CoreEcommerceDbContext())
            {
                totalQuantity = db.ShoppingCartItems.Where(i => i.ShoppingCartId == id)
                    .Select(i => i.Quantity)
                    .DefaultIfEmpty(0).Sum(i => i);
            }

            return Json(new { Ok = true, Data = new { Id = id, TotalQuantity = totalQuantity }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProductToShoppingCart(Guid productId, int quantity)
        {
            var id = GetShoppingCartId();

            MemoryMessageBuss.PushCommand(new AddProductToShoppingCart(id, productId, quantity));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProductFromShoppingCart(Guid productId, int quantity)
        {
            var id = GetShoppingCartId();

            MemoryMessageBuss.PushCommand(new RemoveProductFromShoppingCart(id, productId, quantity));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveAllProductFromShoppingCart()
        {
            var id = GetShoppingCartId();

            MemoryMessageBuss.PushCommand(new RemoveAllProductFromShoppingCart(id));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PreCalculate()
        {
            var id = GetShoppingCartId();

            MemoryMessageBuss.PushCommand(new PreCalculateShoppingCart(id));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EstimateShippingCost(Guid shippingMethodId, string address, double addressLatitude, double addressLongitude)
        {
            if (shippingMethodId == Guid.Empty || string.IsNullOrEmpty(address))
            {
                return Json(new { Ok = false, Data = new { }, Message = "shipping method, address required" }, JsonRequestBehavior.AllowGet);
            }
            var id = GetShoppingCartId();

            MemoryMessageBuss.PushCommand(new EstimateShippingCostForShoppingCart(id, shippingMethodId, address, addressLatitude, addressLongitude));
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == id);
            }

            double distance = ShippingMethodServices.CalculateDistance(id, address, addressLatitude, addressLongitude);
            
            return Json(new { Ok = true, Data =new {Cart=cart,Distance=distance}, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckVoucherCode(string voucherCode)
        {
            if (string.IsNullOrEmpty(voucherCode))
            {
                return Json(new
                {
                    Ok = false,
                    Data = new { VoucherValue = 0, Description = "voucher code required" },
                    Message = "voucher code required"
                }, JsonRequestBehavior.AllowGet);
            }
            var id = GetShoppingCartId();

            var currentUserId = UserSessionContext.CurrentUserId();
            MemoryMessageBuss.PushCommand(new CheckVoucherCodeForShoppingCart(id, voucherCode, currentUserId));
            ShoppingCart cart;
            string description = string.Empty;
            List<ContentLanguage> contentLanguages;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == id);
                var voucher =
                    db.VoucherCodes.SingleOrDefault(
                        i => i.Code.Equals(voucherCode, StringComparison.OrdinalIgnoreCase));
                if (voucher == null)
                {
                    return Json(new
                    {
                        Ok = false,
                        Data = new
                        {
                            VoucherValue = 0,
                            Description = "voucher code was not exist"
                        },
                        Message = "voucher code was not exist"
                    }, JsonRequestBehavior.AllowGet);
                }
                contentLanguages = db.ContentLanguages.Where(i => i.Id == voucher.Id || i.Id == voucher.VoucherMethodId)
                    .ToList();
            }
            description = string.Join("<br>", contentLanguages.Where(i => i.ColumnName.Equals("Description")).Select(i => i.ColumnValue).ToList());
            return Json(new
            {
                Ok = true,
                Data = new
                {
                    VoucherValue = cart.VoucherValue,
                    Description = description
                },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompleteAndPayOrder(Guid paymentMethodId,
            Guid shippingMethodId, DateTime receivingTime, string address, double addressLatitude, double addressLongitude
            , string addressName, string phone, string email
            , string voucherCode, string message)
        {
            if (receivingTime <= DateTime.Now)
            {
                return Json(new { Ok = false, Data = new { }, Message = "receiving time must bigger than now" }, JsonRequestBehavior.AllowGet);
            }
            if (shippingMethodId == Guid.Empty || string.IsNullOrEmpty(address))
            {
                return Json(new { Ok = false, Data = new { }, Message = "shipping method, address required" }, JsonRequestBehavior.AllowGet);
            }
            if (paymentMethodId == Guid.Empty || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                return Json(new { Ok = false, Data = new { }, Message = "payment method, phone, email required" }, JsonRequestBehavior.AllowGet);
            }
            if (email.IsValidEmail() == false)
            {
                return Json(new { Ok = false, Data = new { }, Message = "email not valid" }, JsonRequestBehavior.AllowGet);
            }

            var id = GetShoppingCartId();

            var userId = UserSessionContext.CurrentUserId();

            MemoryMessageBuss.PushCommand(new CheckoutShoppingCart(id, userId,
                paymentMethodId, shippingMethodId, addressName, receivingTime, address, addressLatitude, addressLongitude
                , voucherCode, phone, email, message, LanguageId));

            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == id);
            }

            return Json(new { Ok = true, Data = cart, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult OrderPayment(string orderCode)
        {
            ShoppingCart cart;
            List<ContentLanguage> contentLanguages;
            var model = new FeOrderPaymentPage();

            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));

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

            model.Shipping.Description = contentLanguages.GetValue(model.Shipping.Id, "Description");
            model.Payment.Description = contentLanguages.GetValue(model.Payment.Id, "Description");
            model.OrderPromotion.Description = contentLanguages.GetValue(model.OrderPromotion.Id, "Description");

            return View(model);
        }

        public JsonResult CustomerConfirm(string orderCode)
        {
            PaymentTransaction payTrans;
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                payTrans = db.PaymentTransactions.SingleOrDefault(
                    i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
                if (payTrans == null)
                {
                    throw new Exception("payment was not created");
                }

                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
            }

            MemoryMessageBuss.PushCommand(new ConfirmShoppingCartByCustomer(cart.Id, UserSessionContext.CurrentUserId()));

            return Json(new { Ok = true, Data = payTrans, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateNewShoppingCart()
        {
            var id = Guid.NewGuid();
            SetShoppingCartId(id);
            MemoryMessageBuss.PushCommand(new CreateShoppingCart(id, UserSessionContext.CurrentUserId(), LanguageId, CurrentIpAddress, SiteDomainUrl));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public void Handle(ShoppingCartCheckedout e)
        {
            var httpContext = System.Web.HttpContext.Current;
            httpContext.Session["shoppingcartid"] = string.Empty;
        }

        public void Handle(ShoppingCartAddedProduct e)
        {
            PreCalculateShoppingCart(e.Id, e.ProductId, e.Quantity);
        }

        public void Handle(ShoppingCartRemovedProduct e)
        {
            PreCalculateShoppingCart(e.Id, e.ProductId, e.Quantity);
        }
    
        public void Handle(ShoppingCartRemovedAllProduct e)
        {
            PreCalculateShoppingCart(e.Id, Guid.Empty, 0);
        }

        private static void PreCalculateShoppingCart(Guid shoppingCartId, Guid productId, int productQuantity)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();
            var msg = string.Empty;

            var pp = ProductPromotionServices.CalculateDiscount(productId, productQuantity);
            var ooDiscount = OrderPromotionServices.CalculateForDiscount(shoppingCartId);
            var ooShipping = OrderPromotionServices.CalculateForShipping(shoppingCartId);

            using (var db=new CoreEcommerceDbContext())
            {
                if (pp != null)
                {
                    msg += db.ContentLanguages.GetValue(pp.Id, "Description");
                }
                if (ooDiscount != null)
                {
                    msg +="<br>"+ db.ContentLanguages.GetValue(ooDiscount.Id, "Description");
                }
                if (ooShipping != null)
                {
                    msg += "<br>" + db.ContentLanguages.GetValue(ooShipping.Id, "Description");
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                hubContext.Clients.All.shoppingCartMessage(new NotificationMessage()
                {
                    DataType = "ShoppingCart",
                    DataJson = JsonConvert.SerializeObject(new
                    {
                        Id = shoppingCartId,
                        ProductId = productId,
                        ActionType = "PreCalculateShoppingCart",
                        Message = msg
                    })
                });
            }

            hubContext.Clients.All.shoppingCartMessage(new NotificationMessage()
            {
                DataType = "ShoppingCart",
                DataJson = JsonConvert.SerializeObject(new
                {
                    Id = shoppingCartId,
                    ActionType = "RefreshShoppingCart",
                    Message = msg
                })
            });
        }
    }
}