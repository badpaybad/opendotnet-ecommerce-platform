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
    public class ProductCommandHandles : ICommandHandle<CreateProduct>, ICommandHandle<UpdateProduct>,
         ICommandHandle<UpdateProductForSeo>, ICommandHandle<PublishProduct>, ICommandHandle<DeleteProduct>
        , ICommandHandle<ChangeProductToCategories>
        , ICommandHandle<AddImagesToProduct>, ICommandHandle<RemoveImagesFromProduct>
        , ICommandHandle<AddCommentToProduct>, ICommandHandle<AddProductsToCombo>
    {
        ICqrsEventSourcingRepository<DomainProduct> _repo = new CqrsEventSourcingRepository<DomainProduct>(new EventPublisher());
        public void Handle(CreateProduct c)
        {
            _repo.CreateNew(new DomainProduct(c.Id, c.ProductCode, c.AllowComment, c.Price, c.Gram, c.Calorie, c.Quantity, c.Title, c.UrlImage, c.ShortDesciption, c.Description, c.LanguageId, c.ParentId));
        }

        public void Handle(UpdateProduct c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.Update(c.ProductCode, c.AllowComment, c.Price, c.Gram, c.Calorie, c.Quantity, c.UrlImage, c.Title, c.ShortDesciption, c.Description, c.LanguageId);
            });
        }

        public void Handle(UpdateProductForSeo c)
        {
            _repo.GetDoSave(c.Id, obj => obj.UpdateSeo(c.SeoKeywords, c.SeoDescription, c.LanguageId, c.SeoUrlFriendly));
        }

        public void Handle(PublishProduct c)
        {
            if (c.IsPublish)
                _repo.GetDoSave(c.Id, obj => obj.Publish());
            else
                _repo.GetDoSave(c.Id, obj => obj.Unpublish());
        }

        public void Handle(DeleteProduct c)
        {
            _repo.GetDoSave(c.Id, obj => obj.Delete());
        }

        public void Handle(ChangeProductToCategories c)
        {
            _repo.GetDoSave(c.Id, obj => obj.ChangeToCategories(c.CategoryIds));
        }

        public void Handle(AddImagesToProduct c)
        {
            _repo.GetDoSave(c.Id, obj => obj.AddImages(c.UrlImages));
        }

        public void Handle(RemoveImagesFromProduct c)
        {
            _repo.GetDoSave(c.Id, obj => obj.RemoveImages(c.UrlImages));
        }

        public void Handle(AddCommentToProduct c)
        {
            _repo.GetDoSave(c.ProductId, o => o.AddComment(c.Comment, c.AuthorName, c.UserId, c.CommentParentId));
        }

        public void Handle(AddProductsToCombo c)
        {
            _repo.GetDoSave(c.Id, o => o.AddToCombo(c.ProductIds));
        }

        public void Handle(BuyProdcutByCustomer c)
        {
            _repo.GetDoSave(c.Id, o=>o.BuyByCustomer(c.CustomerId,c.Quantity,c.CustomerEmail,c.WebsiteUrl));
        }
    }

    public class BuyProdcutByCustomer:ICommand
    {
        public BuyProdcutByCustomer(Guid id, Guid customerId, int quantity, string customerEmail, string websiteUrl)
        {
            Id = id;
            CustomerId = customerId;
            Quantity = quantity;
            CustomerEmail = customerEmail;
            WebsiteUrl = websiteUrl;
        }

        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public int Quantity { get; private set; }
        public string CustomerEmail { get; private set; }
        public string WebsiteUrl { get; private set; }
    }

    public class AddProductsToCombo : AdminBaseCommand
    {
        public Guid Id { get; }
        public List<Guid> ProductIds { get; }

        public AddProductsToCombo(Guid id, List<Guid> productIds, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            ProductIds = productIds;
        }
    }
}
