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
    public class DomainOrderPromotion : AggregateRoot
    {
        private bool _freeShip;
        private long _amountToDiscount;
        private long _discountAmount;
        private bool _actived;

        public DomainOrderPromotion()
        {
        }

        public override string Id { get; set; }

        void Apply(OrderPromotionCreated e)
        {
            Id = e.Id.ToString();
            _amountToDiscount = e.AmountToDiscount;
            _discountAmount = e.DiscountAmount;
            _freeShip = e.FreeShip;
        }
        void Apply(OrderPromotionUpdated e)
        {
            _amountToDiscount = e.AmountToDiscount;
            _discountAmount = e.DiscountAmount;
            _freeShip = e.FreeShip;
        }
        void Apply(OrderPromotionActived e)
        {
            _actived = true;
        }
        void Apply(OrderPromotionInactived e)
        {
            _actived = false;
        }

        public DomainOrderPromotion(Guid id, Guid languageId, string description, long amountToDiscount, long discountAmount, bool freeShip)
        {
            Id = id.ToString();

            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "OrderPromotion"));

            ApplyChange(new OrderPromotionCreated(id, languageId, description, amountToDiscount, discountAmount, freeShip));
        }

        public void Update(Guid languageId, string description, long amountToDiscount, long discountAmount, bool freeShip)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "OrderPromotion"));
            ApplyChange(new OrderPromotionUpdated(id, languageId, description, amountToDiscount, discountAmount, freeShip));
        }

        public void Acitve()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new OrderPromotionActived(id));
        }

        public void Inactive()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new OrderPromotionInactived(id));
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new OrderPromotionDeleted(id));
        }
    }

}