using System;
using System.Collections.Generic;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class NewsEditAdminPage
    {
        public Guid Id;
        public Guid CategoryId;
        public string CategoryTitle;
        public string Title;
        public bool AllowComment;
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

        public string UrlImage;

        public bool Published;

        public DateTime CreatedDate;

        public List<Guid> NewsCategoies;
    }
}