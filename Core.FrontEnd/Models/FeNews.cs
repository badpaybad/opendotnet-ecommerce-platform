using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeNews
    {
        public Guid Id;
        public string Title;
        public string ShortDescription;
        public string Description;

        public string SeoUrlFriendly;
        public string SeoKeywords;
        public string SeoDescription;
        public string UrlImage;
        public DateTime CreatedDate;

        public bool Published;
        public bool AllowComment;
    }
    
}