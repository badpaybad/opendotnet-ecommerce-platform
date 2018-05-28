using System;
using System.Collections.Generic;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Models
{
    public class FeShoppingCartCheckoutPage
    {
        public Guid Id;
        public List<FeIdAndDescription> PaymentMethods;
        public List<FeIdAndDescription> ShippingMethods;
        public int CartItemCount;
        public FeOrderPromotion OrderPromotion;
        public long CartTotal;

        public class FeIdAndDescription
        {
            public Guid Id;
            public string Name;
            public string Description;
        }

        public long CartDiscount { get; set; }
    }
}