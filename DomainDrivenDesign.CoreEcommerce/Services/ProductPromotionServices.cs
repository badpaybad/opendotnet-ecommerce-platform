using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public class ProductPromotionServices
    {
        public static ProductPromotion CalculateDiscount(Guid productId, long quantity)
        {
            List<ProductPromotion> pp = null;
            using (var db = new CoreEcommerceDbContext())
            {
                pp = db.ProductPromotions.Join(db.RelationShips, p => p.Id, r => r.FromId,
                        (p, r) => new { Pp = p, R = r })
                    .Where(m => m.R.ToId == productId).Select(i => i.Pp)
                    //.Where(i => i.FromDate >= DateTime.Now && i.ToDate <= DateTime.Now)
                    .ToList();
               
            }
            if (pp.Count == 0) return null;

            return pp.Where(i => i.ProductQuantity <= quantity).OrderByDescending(i=>i.DiscountValue).FirstOrDefault();
        }
    }
}
