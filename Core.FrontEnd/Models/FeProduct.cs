using System;
using System.Collections.Generic;

namespace Core.FrontEnd.Models
{
    public class FeProduct
    {
        public Guid Id;
        public bool AllowComment;
        public string Title;
        public string ProductCode;
        public string ShortDescription;
        public string Description;

        public string SeoUrlFriendly;
        public string SeoKeywords;
        public string SeoDescription;
        public string UrlImage;
        public DateTime CreatedDate;

        public long Price;
        public long Quantity;

        public bool Published;
        public bool IsCombo;

        public List<string> Galleries=new List<string>();

        public List<ProductInCombo> ProductsInCombo;

        public List<FeProductPromotion> Promotions;

        public class ProductInCombo
        {
            public Guid Id;
            public string ProductCode;
            public string Title;
            public long Price;
            public bool Published;
        }

        public int Gram { get; set; }
        public int Calorie { get; set; }
    }
}