using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using Core.FrontEnd.Plugins;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Cod(string orderCode)
        {
            ShoppingCart cart;
            PaymentTransaction payTrans;
            List<ContentLanguage> contentLanguages;
            var model = new FeOrderPaymentPage();

            using (var db = new CoreEcommerceDbContext())
            {
                payTrans = db.PaymentTransactions.SingleOrDefault(
                   i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
                if (payTrans == null)
                {
                    throw new Exception("payment was not created");
                }

                cart = db.ShoppingCarts.SingleOrDefault(i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));

                contentLanguages = db.ContentLanguages
                    .Where(i => i.Id == cart.PaymentMethodId || i.Id == cart.ShippingMethodId).ToList();

                model.Payment = db.PaymentMethods.Select(i => new FeShoppingCartCheckoutPage.FeIdAndDescription() { Id = i.Id, Name = i.Name })
                    .SingleOrDefault(i => i.Id == cart.PaymentMethodId);
                model.Shipping = db.ShippingMethods.Select(i => new FeShoppingCartCheckoutPage.FeIdAndDescription() { Id = i.Id, Name = i.Name })
                    .SingleOrDefault(i => i.Id == cart.ShippingMethodId);

                model.Address = db.ShoppingCartShippingAddresses
                                    .Where(i => i.ShoppingCartId == cart.Id).OrderByDescending(i => i.CreatedDate).FirstOrDefault()
                                ?? new ShoppingCartShippingAddress();
            }
            model.Order = cart;

            model.Shipping.Description = contentLanguages.GetValue(model.Shipping.Id, "Description");
            model.Payment.Description = contentLanguages.GetValue(model.Payment.Id, "Description");

            return View(model);
        }

        public ActionResult OnepayInternationalDr(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            var model = PaymentMethodInternational.ProcessDr(vpc_MerchTxnRef, vpc_OrderInfo, vpc_Amount);

            return View(model);
        }

        public ActionResult OnepayInternationalIpn(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            var model = PaymentMethodInternational.ProcessDr(vpc_MerchTxnRef, vpc_OrderInfo, vpc_Amount);
            if (model.Success && model.Status == Enums.ShoppingCartPayStatus.PaymentSuccess)
            {
                return Content("responsecode=1&desc=confirm-success");
            }

            return Content("responsecode=0&desc=" + model.Message);
        }

        public ActionResult OnepayDomesticDr(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            var model = PaymentMethodDomestic.ProcessDr(vpc_MerchTxnRef, vpc_OrderInfo, vpc_Amount);
            return View(model);
        }

        public ActionResult OnepayDomesticIpn(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            var model = PaymentMethodInternational.ProcessDr(vpc_MerchTxnRef, vpc_OrderInfo, vpc_Amount);
            if (model.Success && model.Status == Enums.ShoppingCartPayStatus.PaymentSuccess)
            {
                return Content("responsecode=1&desc=confirm-success");
            }

            return Content("responsecode=0&desc=" + model.Message);
        }
    }
}