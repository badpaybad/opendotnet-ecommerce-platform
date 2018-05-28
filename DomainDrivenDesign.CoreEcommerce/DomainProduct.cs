using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Events;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class DomainProduct : AggregateRoot
    {
        public DomainProduct()
        {
        }
        private bool _published;
        private long _price;
        private long _quantity;
        private bool _allowComment;
        private string _productCode;
        public override string Id { get; set; }

        void Apply(ProductCreated e)
        {
            Id = e.Id.ToString();
            _price = e.Price;
            _quantity = e.Quantity;
            _productCode = e.ProductCode;
        }

        void Apply(ProductUpdated e)
        {
            _quantity = e.Quantity;
            _productCode = e.ProductCode;
            _allowComment = e.AllowComment;
        }
        void Apply(ProductPriceChanged e)
        {
            _price = e.Price;
        }

        void Apply(ProductBoughtByCustomer e)
        {
            _quantity = e.Quantity;

        }
        void Apply(ProductPublished e)
        {
            Id = e.Id.ToString();
            _published = true;
        }
        void Apply(ProductUnpublished e)
        {
            Id = e.Id.ToString();
            _published = false;
        }

        public DomainProduct(Guid id, string productCode, bool allowComment, long price, int gram, int calorie, long quantity, string title, string urlImage, string shortDescription, string description,
            Guid languageId, Guid parentId)
        {
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "ShortDescription", shortDescription, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "UrlImage", urlImage, "Product"));

            var urlSegment = title.ToUrlSegment();
            var seoUrlFiendly = RefindSeoUrlFiendly(urlSegment, id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrlFiendly, "Product"));
            ApplyChange(new UrlFriendlyCreated(seoUrlFiendly, "Product", id, "Product", "Detail"));
            ApplyChange(new ProductCreated(id, productCode, allowComment, price, gram, calorie, quantity, parentId, title, DateTime.Now));
        }

        public void ChangeToCategories(List<Guid> categoryIds)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new RelationShipToRemoved(id, "Product"));
            ApplyChange(new RelationShipAddedManyFromWithOneTo(categoryIds, id, "Category", "Product"));

        }

        public void AddAttribute(string attributeKeyName, bool attributeIsMultiChoice
            , string skuCode, string valueText, double valueAddjustPrice, Guid languageId)
        {

        }

        public void RemoveAttributeValue(string valueKeyName)
        {

        }

        public void AddImages(List<string> urlImages)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ProductAddedImage(id, urlImages));
        }

        public void RemoveImages(List<string> urlImages)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ProductRemovedImages(id, urlImages));
        }

        public void Update(string productCode, bool allowComment, long price, int gram, int calorie, long quantity, string urlImage, string title, string shortDescription, string description, Guid languageId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "ShortDescription", shortDescription, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "UrlImage", urlImage, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "Product"));
            if (_price != price)
            {
                ApplyChange(new ProductPriceChanged(id, price));
            }

            ApplyChange(new ProductUpdated(id, quantity, gram, calorie, productCode, allowComment));
        }

        public void AddToCombo(List<Guid> productIds)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new ProductAsComboAdded(id, productIds));
        }

        public void UpdateSeo(string seokeywords, string seoDesscription, Guid languageId, string seoUrlFiendly = "")
        {
            var id = Guid.Parse(Id);

            if (string.IsNullOrEmpty(seoUrlFiendly))
            {
                List<ContentLanguage> contentLanguages;
                using (var db = new CoreEcommerceDbContext())
                {
                    contentLanguages = db.ContentLanguages.Where(i => i.Id == id).ToList();
                }
                var title = contentLanguages.GetValue(id, languageId, "Title");
                seoUrlFiendly = title.ToUrlSegment();
            }

            if (ExistedSeoUrlFiendly(seoUrlFiendly, id)) throw new Exception("Existed seo url friendly");

            seoUrlFiendly = RefindSeoUrlFiendly(seoUrlFiendly, id);

            ApplyChange(new UrlFriendlyCreated(seoUrlFiendly, "Product", id, "Product", "Detail"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrlFiendly, "Product"));

            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoKeywords", seokeywords, "Product"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoDescription", seoDesscription, "Product"));
        }

        public void BuyByCustomer(Guid customerId, int quantity, string customerEmail, string websiteUrl)
        {
            if (_quantity == 0) throw new Exception("Out of stock");
            _quantity = _quantity - quantity;
            var id = Guid.Parse(Id);
            ApplyChange(new ProductBoughtByCustomer(id, _quantity, customerId, customerEmail, websiteUrl, DateTime.Now));
        }

        public void Publish()
        {
            var id = Guid.Parse(Id);

            ApplyChange(new ProductPublished(id));
        }

        public void Unpublish()
        {
            var id = Guid.Parse(Id);

            ApplyChange(new ProductUnpublished(id));
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ProductDeleted(id));
        }


        public void AddComment(string comment, string authorName, Guid userId, Guid parentCommentId)
        {
            if (!_allowComment) throw new Exception("Not allow comment");
            var id = Guid.Parse(Id);
            if (parentCommentId == Guid.Empty)
            {
                ApplyChange(new CommentAdded(id, Guid.NewGuid(), "Product", authorName, comment, DateTime.Now, userId));
            }
            else
            {
                ApplyChange(new CommentReplied(id, Guid.NewGuid(), parentCommentId, "Product", authorName, comment, DateTime.Now, userId));
            }
        }


        private string RefindSeoUrlFiendly(string seoUrlFiendly, Guid id)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var existedSeoUrl =
                    db.ContentLanguages.Where(i => i.Id != id &&
                                                   i.ColumnValue.Equals(seoUrlFiendly, StringComparison.OrdinalIgnoreCase))
                        .Select(i => i.ColumnValue).FirstOrDefault();

                if (!string.IsNullOrEmpty(existedSeoUrl))
                {
                    seoUrlFiendly = existedSeoUrl + "-" + id.GetHashCode().ToString().Trim('-');
                }
            }
            if (string.IsNullOrEmpty(seoUrlFiendly)) seoUrlFiendly = id.ToString().ToLower();
            return seoUrlFiendly;
        }

        private bool ExistedSeoUrlFiendly(string seoUrlFiendly, Guid id)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var existedSeoUrl =
                    db.ContentLanguages.Any(i => i.Id != id &&
                                                 i.ColumnValue.Equals(seoUrlFiendly, StringComparison.OrdinalIgnoreCase));

                return existedSeoUrl;
            }

        }
    }
}
