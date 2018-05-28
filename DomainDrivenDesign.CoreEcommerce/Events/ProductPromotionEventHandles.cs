using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
   public class ProductPromotionEventHandles:IEventHandle<ProductPromotionCreated>, IEventHandle<ProductPromotionUpdated>
        ,IEventHandle<ProductPromotionAddedToProducts>
        , IEventHandle<ProductPromotionDeleted>
    {
        public void Handle(ProductPromotionCreated e)
        {
            using (var db =new CoreEcommerceDbContext())
            {
                db.ProductPromotions.Add(new ProductPromotion()
                {
                    Id=e.Id,
                    CreatedDate=DateTime.Now,
                    ProductQuantity=e.Quantity,
                    DiscountValue=e.DiscountValue,
                    ToDate=e.ToDate,
                    FromDate=e.FromDate
                });
                db.SaveChanges();
            }
        }

        public void Handle(ProductPromotionUpdated e)
        {
            using (var db=new CoreEcommerceDbContext())
            {
                var temp = db.ProductPromotions.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.ProductQuantity = e.ProductQuantity;
                    temp.DiscountValue = e.DiscountValue;
                    temp.FromDate = e.FromDate;
                    temp.ToDate = e.ToDate;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ProductPromotionAddedToProducts e)
        {
            
        }

       

        public void Handle(ProductPromotionDeleted e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ProductPromotions.SingleOrDefault(i => i.Id == e.Id);
                
                if (temp != null)
                {
                    db.ProductPromotions.Remove(temp);
                    db.SaveChanges();
                }
            }
        }
    }


    public class ProductPromotionDeleted : IEvent
    {
        public Guid Id { get; private set; }

        public ProductPromotionDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class ProductPromotionAddedToProducts : IEvent
    {
        public Guid Id { get; }
        public List<Guid> ProductIds { get; }
        public long Quantity { get; }
        public long DiscountValue { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public ProductPromotionAddedToProducts(Guid id, List<Guid> productIds, long quantity, long discountValue
            , DateTime fromDate, DateTime toDate)
        {
            Id = id;
            ProductIds = productIds;
            Quantity = quantity;
            DiscountValue = discountValue;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public long Version { get; set; }
    }

    public class ProductPromotionUpdated : IEvent
    {
        public Guid Id { get; }
        public long ProductQuantity { get; }
        public long DiscountValue { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public ProductPromotionUpdated(Guid id, long productQuantity, long discountValue, DateTime fromDate, DateTime toDate)
        {
            Id = id;
            ProductQuantity = productQuantity;
            DiscountValue = discountValue;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public long Version { get; set; }
    }

    public class ProductPromotionCreated : IEvent
    {
        public Guid Id { get; private set; }
        public long Quantity { get; private set; }
        public long DiscountValue { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        public ProductPromotionCreated(Guid id, long quantity, long discountValue, DateTime fromDate, DateTime toDate)
        {
            Id = id;
            Quantity = quantity;
            DiscountValue = discountValue;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public long Version { get; set; }
    }
}
