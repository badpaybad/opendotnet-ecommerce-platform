using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public class OrderPromotionServices
    {
        public static OrderPromotion CalculateForDiscount(Guid shoppingCartId)
        {
            using (var db =new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.OrderByDescending(i=>i.CreatedDate).FirstOrDefault(i => i.Actived);
                var cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == shoppingCartId);
                if (cart != null && temp != null && cart.CartTotal >= temp.AmountToDiscount)
                {
                    return temp;
                }
                return temp;
            }
        }
        public static OrderPromotion CalculateForShipping(Guid shoppingCartId)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.OrderByDescending(i => i.CreatedDate).FirstOrDefault(i => i.Actived);
                var cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == shoppingCartId);
                if (cart != null && temp!=null && cart.CartTotal >= temp.AmountToDiscount)
                {
                    return temp;
                }
            }
            return null;
        }
    }
}
