using System;
using System.Linq;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductEventHandles : IEventHandle<ProductCreated>, IEventHandle<ProductUpdated>,
            IEventHandle<ProductDeleted>, IEventHandle<ProductPublished>, IEventHandle<ProductUnpublished>,
        IEventHandle<ProductPriceChanged>
        , IEventHandle<ProductAddedImage>, IEventHandle<ProductRemovedImages>,
        IEventHandle<ProductAsComboAdded>,IEventHandle<ProductBoughtByCustomer>
    {
        public void Handle(ProductCreated e)
        {
            using (var db = new CoreEcommerce.Ef.CoreEcommerceDbContext())
            {
                db.Products.Add(new Product()
                {
                    Id = e.Id,
                    ParentId = e.ParentId,
                    CreatedDate = e.CreatedDate,
                    Price = e.Price,
                    Quantity=e.Quantity,
                    ProductCode=e.ProductCode,
                    AllowComment=e.AllowComment,
                    Gram=e.Gram,
                    Calorie=e.Calorie
                });
                db.SaveChanges();
            }
        }

        public void Handle(ProductUpdated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Quantity = e.Quantity;
                    temp.ProductCode = e.ProductCode;
                    temp.AllowComment = e.AllowComment;
                    temp.Gram = e.Gram;
                    temp.Calorie = e.Calorie;
                }
                db.SaveChanges();
            }
        }


        public void Handle(ProductDeleted e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Deleted = true;
                }
                db.SaveChanges();
            }
        }

        public void Handle(ProductPublished e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Published = true;
                }
                db.SaveChanges();
            }
        }

        public void Handle(ProductUnpublished e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Published = false;
                }
                db.SaveChanges();
            }
        }

        public void Handle(ProductPriceChanged e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Price = e.Price;
                }
                db.SaveChanges();
            }
        }

        public void Handle(ProductAddedImage e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                foreach (var imgurl in e.UrlImages)
                {
                    var temp = db.PhotoGalleries.FirstOrDefault(i => i.Id== e.Id &&
                    i.UrlImage.Equals(imgurl, StringComparison.OrdinalIgnoreCase));
                    if (temp == null)
                    {
                        db.PhotoGalleries.Add(new PhotoGallery()
                        {
                            Id = e.Id,
                            UrlImage = imgurl,
                            PgId = Guid.NewGuid(),
                            TableName = "Product"
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        public void Handle(ProductRemovedImages e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                foreach (var imgurl in e.UrlImages)
                {
                    var temp = db.PhotoGalleries.FirstOrDefault(i => i.Id== e.Id &&
                    i.UrlImage.Equals(imgurl, StringComparison.OrdinalIgnoreCase));
                    if (temp != null)
                    {
                        db.PhotoGalleries.Remove(temp);
                    }
                }

                db.SaveChanges();
            }
        }

        public void Handle(ProductAsComboAdded e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var combo = db.Products.SingleOrDefault(i => i.Id == e.Id);
                if (combo != null)
                {
                    combo.IsCombo = e.ProductIds.Count>0;
                }
                var temp = db.ProductInCombos.Where(i => i.ProductId == e.Id).ToList();

                db.ProductInCombos.RemoveRange(temp);

                foreach (var pcid in e.ProductIds)
                {
                    db.ProductInCombos.Add(new ProductInCombo()
                    {
                        ProductId = e.Id,
                        ProductComboId = pcid
                    });
                }
                db.SaveChanges();
            }
        }

        public void Handle(ProductBoughtByCustomer e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Products.SingleOrDefault(i => i.Id == e.ProductId);
                if (temp != null)
                {
                    temp.Quantity = e.Quantity;
                }
                db.SaveChanges();
            }
        }
    }
}
