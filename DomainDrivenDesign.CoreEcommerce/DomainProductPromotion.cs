using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.CoreEcommerce.Events;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class DomainProductPromotion : AggregateRoot
    {
        private long _productQuantity;
        private long _discountValue;
        private DateTime _fromDate;
        private DateTime _toDate;
      
        public override string Id { get; set; }

        public DomainProductPromotion()
        {
        }

        void Apply(ProductPromotionCreated e)
        {
            Id = e.Id.ToString();
            _productQuantity = e.Quantity;
            _discountValue = e.DiscountValue;
            _fromDate = e.FromDate;
            _toDate = e.ToDate;
        }

        void Apply(ProductPromotionUpdated e)
        {
            _productQuantity = e.ProductQuantity;
            _discountValue = e.DiscountValue;
            _fromDate = e.FromDate;
            _toDate = e.ToDate;
        }

        public DomainProductPromotion(Guid id, long productQuantity, long discountValue
            , string description, DateTime fromDate, DateTime toDate, Guid languageId)
        {

            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "ProductPromotion"));

            ApplyChange(new ProductPromotionCreated(id, productQuantity, discountValue, fromDate, toDate));
        }
        
        public void Update(long productQuantity, long discountValue
            , string description, DateTime fromDate, DateTime toDate, Guid languageId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "ProductPromotion"));
            ApplyChange(new ProductPromotionUpdated(id, productQuantity, discountValue, fromDate, toDate));
        }


        public void AddToProducts(List<Guid> productIds)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new RelationShipAddedOneFromWithManyTo(id,productIds,"ProductPromotion","Product"));

            ApplyChange(new ProductPromotionAddedToProducts(id, productIds, _productQuantity, _discountValue, _fromDate, _toDate));
        }

        public void RemoveFromProducts(List<Guid> productIds)
        {
            var id = Guid.Parse(Id);
            foreach (var productId in productIds)
            {
                ApplyChange(new RelationShipRemoved(id, productId, "ProductPromotion", "Product"));
            }
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ProductPromotionDeleted(id));
        }
    }

}
