using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.Core;

namespace Core.FrontEnd.Models
{
    public class FeCategory
    {
        public Guid Id;
        public string Title;
        public bool IsSinglePage;
        public bool ShowInFrontEnd;
        public Enums.CategoryType Type;
        public string CategoryViewName;

        public string SeoUrlFriendly;
        public string SeoDescription;
        public string SeoKeywords;

        public List<NewsItem> News = new List<NewsItem>();
        public List<ProductItem> Products = new List<ProductItem>();
        public long TotalNews;
        public long TotalProduct;

        private NewsItem _firstNews = null;
        public NewsItem FirstNews
        {
            get
            {
                if (_firstNews != null) return _firstNews;
                _firstNews = News.FirstOrDefault() ?? new NewsItem();
                return _firstNews;
            }
        }
        private ProductItem _firstProduct = null;
        public ProductItem FirstProduct
        {
            get
            {
                if (_firstProduct != null) return _firstProduct;
                _firstProduct = Products.FirstOrDefault() ?? new ProductItem();
                return _firstProduct;
            }
        }

        public class NewsItem
        {
            public Guid Id;
            public string Title;
            public string ShortDescription;
            public string Description;
            public string UrlImage;
            public string SeoUrlFriendly;
            public string SeoDescription;
            public string SeoKeywords;
            public DateTime CreatedDate;
        }

        public class ProductItem
        {
            public Guid Id;
            public string ProductCode;
            public string Title;
            public string ShortDescription;
            public string Description;
            public string UrlImage;
            public string SeoUrlFriendly;
            public string SeoDescription;
            public string SeoKeywords;
            public DateTime CreatedDate;
            public long Price;
            public long Quantity;
            public int Gram;
            public int Calorie;
        }
    }
}