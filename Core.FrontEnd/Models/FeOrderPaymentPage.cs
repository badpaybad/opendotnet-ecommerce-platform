using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Models
{
    public class FeOrderPaymentPage
    {
        public ShoppingCart Order;
        public FeShoppingCartCheckoutPage.FeIdAndDescription Payment;
        public FeShoppingCartCheckoutPage.FeIdAndDescription Shipping;
        public PaymentTransaction PaymentResult;
        public ShoppingCartShippingAddress Address;

        public FeOrderPromotion OrderPromotion;
    }
}