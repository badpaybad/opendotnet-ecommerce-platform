using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class OrderPromotionCommandHandles : ICommandHandle<CreateOrderPromotion>, ICommandHandle<UpdateOrderPromotion>
        , ICommandHandle<ActiveOrderPromotion>, ICommandHandle<InactiveOrderPromotion>,
        ICommandHandle<DeleteOrderPromotion>
    {
        ICqrsEventSourcingRepository<DomainOrderPromotion> _repo = new CqrsEventSourcingRepository<DomainOrderPromotion>(new EventPublisher());

        public void Handle(CreateOrderPromotion c)
        {
            _repo.CreateNew(new DomainOrderPromotion(c.Id, c.LanguageId, c.Description, c.AmountToDiscount, c.DiscountAmount, c.FreeShip));
        }

        public void Handle(UpdateOrderPromotion c)
        {
            _repo.GetDoSave(c.Id, o => o.Update(c.LanguageId, c.Description, c.AmountToDiscount, c.DiscountAmount, c.FreeShip));
        }

        public void Handle(ActiveOrderPromotion c)
        {
            _repo.GetDoSave(c.Id, o => o.Acitve());
        }

        public void Handle(InactiveOrderPromotion c)
        {
            _repo.GetDoSave(c.Id, o => o.Inactive());
        }

        public void Handle(DeleteOrderPromotion c)
        {
            _repo.GetDoSave(c.Id, o => o.Delete());
        }
    }

    public class DeleteOrderPromotion : AdminBaseCommand
    {
        public DeleteOrderPromotion(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class InactiveOrderPromotion : AdminBaseCommand
    {
        public InactiveOrderPromotion(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class ActiveOrderPromotion : AdminBaseCommand
    {
        public ActiveOrderPromotion(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class UpdateOrderPromotion : AdminBaseCommand
    {
        public UpdateOrderPromotion(Guid id, Guid languageId, string description, long amountToDiscount, long discountAmount
            , bool freeShip, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            LanguageId = languageId;
            Description = description;
            AmountToDiscount = amountToDiscount;
            DiscountAmount = discountAmount;
            FreeShip = freeShip;
        }
        public Guid Id { get; private set; }
        public Guid LanguageId { get; private set; }
        public string Description { get; private set; }
        public long AmountToDiscount { get; private set; }
        public long DiscountAmount { get; private set; }
        public bool FreeShip { get; private set; }
    }

    public class CreateOrderPromotion : AdminBaseCommand
    {
        public CreateOrderPromotion(Guid id, Guid languageId, string description, long amountToDiscount, long discountAmount
            , bool freeShip, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            LanguageId = languageId;
            Description = description;
            AmountToDiscount = amountToDiscount;
            DiscountAmount = discountAmount;
            FreeShip = freeShip;
        }

        public Guid Id { get; private set; }
        public Guid LanguageId { get; private set; }
        public string Description { get; private set; }
        public long AmountToDiscount { get; private set; }
        public long DiscountAmount { get; private set; }
        public bool FreeShip { get; private set; }
    }
}
