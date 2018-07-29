using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class OrderPromotionEventHandles : IEventHandle<OrderPromotionCreated>, IEventHandle<OrderPromotionUpdated>
         , IEventHandle<OrderPromotionActived>, IEventHandle<OrderPromotionInactived>, IEventHandle<OrderPromotionDeleted>
    {
        public void Handle(OrderPromotionCreated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                db.OrderPromotions.Add(new OrderPromotion()
                {
                    Id = e.Id,
                    FreeShip = e.FreeShip,
                    Actived = false,
                    AmountToDiscount = e.AmountToDiscount,
                    DiscountAmount = e.DiscountAmount,
                    CreatedDate=DateTime.Now
                });
                db.SaveChanges();
            }
        }

        public void Handle(OrderPromotionUpdated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.AmountToDiscount = e.AmountToDiscount;
                    temp.DiscountAmount = e.DiscountAmount;
                    temp.FreeShip = e.FreeShip;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(OrderPromotionActived e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Actived = true;

                    db.SaveChanges();
                }
            }
        }

        public void Handle(OrderPromotionInactived e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Actived = false;

                    db.SaveChanges();
                }
            }

        }

        public void Handle(OrderPromotionDeleted e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.OrderPromotions.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    db.OrderPromotions.Remove(temp);

                    db.SaveChanges();
                }
            }
        }

       
    }


    public class OrderPromotionDeleted : IEvent
    {
        public Guid Id { get; private set; }

        public OrderPromotionDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class OrderPromotionInactived : IEvent
    {
        public Guid Id { get; private set; }

        public OrderPromotionInactived(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class OrderPromotionActived : IEvent
    {
        public Guid Id { get; private set; }

        public OrderPromotionActived(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class OrderPromotionCreated : IEvent
    {
        public Guid Id { get; }
        public Guid LanguageId { get; }
        public string Description { get; }
        public long AmountToDiscount { get; }
        public long DiscountAmount { get; }
        public bool FreeShip { get; }

        public OrderPromotionCreated(Guid id, Guid languageId, string description, long amountToDiscount, long discountAmount, bool freeShip)
        {
            Id = id;
            LanguageId = languageId;
            Description = description;
            AmountToDiscount = amountToDiscount;
            DiscountAmount = discountAmount;
            FreeShip = freeShip;
        }

        public long Version { get; set; }
    }

    public class OrderPromotionUpdated : IEvent
    {
        public Guid Id { get; }
        public Guid LanguageId { get; }
        public string Description { get; }
        public long AmountToDiscount { get; }
        public long DiscountAmount { get; }
        public bool FreeShip { get; }

        public OrderPromotionUpdated(Guid id, Guid languageId, string description, long amountToDiscount, long discountAmount, bool freeShip)
        {
            Id = id;
            LanguageId = languageId;
            Description = description;
            AmountToDiscount = amountToDiscount;
            DiscountAmount = discountAmount;
            FreeShip = freeShip;
        }

        public long Version { get; set; }
    }

}
