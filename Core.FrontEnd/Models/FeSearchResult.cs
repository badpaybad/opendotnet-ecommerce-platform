using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeSearchResult
    {
        public Guid Id;
        public string Title;
        public string TableName;
        public string SeoUrlFriendly;
        public string UrlImage;
        public DateTime CreatedDate;
    }
}