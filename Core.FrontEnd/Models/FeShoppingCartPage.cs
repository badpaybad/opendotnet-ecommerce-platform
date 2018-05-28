using System;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeShoppingCartPage
    {
        public Guid Id;
        public int CartItemCount;
        public long CartTotal;
        public long CartDiscount;

        public FeOrderPromotion OrderPromotion;

        public string ErrorMessage;

        public class Item
        {
            public Guid ProductId;
            public string Title;
            public string ProductCode;
            public string UrlImage;
            public string SeoUrlFriendly;
            public long UnitPrice;
            public long Quantity;
            public long TotalPrice;
            public string PromotionDescription;
            public Guid ProductPromotionId;
        }
    }
}