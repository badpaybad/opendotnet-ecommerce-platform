using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class ProductPromotionCommandHandles:ICommandHandle<CreateProductPromotion>,ICommandHandle<UpdateProductPromotion>
        ,ICommandHandle<AddPromotionsToProduct>,ICommandHandle<AddProductsToPromotion>
        ,ICommandHandle<DeleteProductPromotion>,ICommandHandle<RemovePromotionsFromProduct>
    {
        ICqrsEventSourcingRepository<DomainProductPromotion> _repo
            =new CqrsEventSourcingRepository<DomainProductPromotion>(new EventPublisher());


        public void Handle(CreateProductPromotion c)
        {
            _repo.CreateNew(new DomainProductPromotion(c.Id,c.ProductQuantity,c.DiscountValue,c.Description
                ,EngineeCurrentContext.SystemMinDate,EngineeCurrentContext.SystemMinDate, c.LanguageId));
        }

        public void Handle(UpdateProductPromotion c)
        {
            _repo.GetDoSave(c.Id,o=>o.Update(c.ProductQuantity,c.DiscountValue,c.Description
                , EngineeCurrentContext.SystemMinDate, EngineeCurrentContext.SystemMinDate,c.LanguageId));
        }

        public void Handle(AddPromotionsToProduct c)
        {
            foreach (var ppId in c.PromotionIds)
            {
                _repo.GetDoSave(ppId,o=>o.AddToProducts(new List<Guid>(){c.ProductId}));
            }
        }

        public void Handle(RemovePromotionsFromProduct c)
        {
            foreach (var ppid in c.PromotionIds)
            {
                _repo.GetDoSave(ppid,o=>o.RemoveFromProducts(new List<Guid>(){c.ProductId}));
            }
        }

        public void Handle(AddProductsToPromotion c)
        {
            _repo.GetDoSave(c.PromotionId, o => o.AddToProducts(c.ProductIds));
        }

        public void Handle(DeleteProductPromotion c)
        {
            _repo.GetDoSave(c.Id,o=>o.Delete());
        }
    }

    public class RemovePromotionsFromProduct : AdminBaseCommand
    {
        public List<Guid> PromotionIds { get; }
        public Guid ProductId { get; }

        public RemovePromotionsFromProduct(List<Guid> promotionIds, Guid productId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            PromotionIds = promotionIds;
            ProductId = productId;
        }
    }

    public class DeleteProductPromotion : AdminBaseCommand
    {
        public DeleteProductPromotion(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class AddProductsToPromotion : AdminBaseCommand
    {
        public AddProductsToPromotion(Guid promotionId, List<Guid> productIds
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            PromotionId = promotionId;
            ProductIds = productIds;
        }

        public Guid PromotionId { get; private set; }
        public List<Guid> ProductIds { get; private set; }
    }

    public class AddPromotionsToProduct : AdminBaseCommand
    {
        public AddPromotionsToProduct(List<Guid> promotionIds, Guid productId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            ProductId = productId;
            PromotionIds = promotionIds;
        }

        public Guid ProductId { get; private set; }
        public List<Guid> PromotionIds { get; private set; }
    }

    public class UpdateProductPromotion : AdminBaseCommand
    {
        public UpdateProductPromotion( Guid id, string description,  long discountValue
            , long productQuantity, Guid languageId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            ProductQuantity = productQuantity;
            DiscountValue = discountValue;
            Description = description;
            LanguageId = languageId;
        }

        public Guid Id { get;  }
        public long ProductQuantity { get; private set; }
        public long DiscountValue { get; private set; }
        public string Description { get; private set; }
        public Guid LanguageId { get; private set; }
    }

    public class CreateProductPromotion : AdminBaseCommand
    {
      
        public CreateProductPromotion(Guid id, string description, long discountValue, long productQuantity
            , Guid languageId, Guid userId, DateTime createdDate) 
            : base(userId, createdDate)
        {
            Description = description;
            DiscountValue = discountValue;
            ProductQuantity = productQuantity;
            Id = id;
            LanguageId = languageId;
        }

        public string Description { get; private set; }
        public long DiscountValue { get; private set; }
        public long ProductQuantity { get; private set; }
        public Guid Id { get; set; }
        public Guid LanguageId { get; private set; }
    }
}
