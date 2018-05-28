using System;
using System.Collections.Generic;
using Core.FrontEnd.Models;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Models
{
   
    public class ProductEditAdminPage
    {
        public Guid Id;
        public bool AllowComment;
        public long Price;
        public long Quantity;
        public string Title;
        public string ProductCode;
        public string ShortDescription;
        public string Description;

        public string SeoKeywords;
        public string SeoDescription;
        public string SeoUrlFriendly;

        public PageMode PageMode
        {
            get
            {
                return Id == Guid.Empty ? Models.PageMode.Create : PageMode.Edit;
            }
        }

        public int Gram { get; set; }
        public int Calorie { get; set; }

        public string UrlImage;

        public bool Published;

        public DateTime CreatedDate;

        public List<Guid> ProductCategoies;

        public List<Image> Galleries;

        public List<ProductInCombo> ProductsInCombo;

        public List<FeProductPromotion> Promotions;
        public List<Supplier> Suppliers;

        public class Image
        {
            public Guid Id { get; set; }
            public string UrlImage { get; set; }
        }

        public class ProductInCombo
        {
            public Guid Id;
            public string ProductCode;
            public string Title;
            public long Price;
            public bool Published;
        }
    }
}