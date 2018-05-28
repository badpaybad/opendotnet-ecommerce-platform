using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;

namespace Core.FrontEnd.Models
{
    public class MenuBuilder
    {
        public List<MenuItem> MainMenu()
        {
            List<Category> cat = new List<Category>();
            List<ContentLanguage> contentLanguages = null;
            using (var db = new CoreCmsDbContext())
            {
                contentLanguages = db.ContentLanguages.Join(db.Categories, cl => cl.Id, c => c.Id,
                        (cl, c) => new { Cl = cl, C = c })
                    .Select(i => i.Cl).ToList();
                cat = db.Categories.Where(i => i.ShowInFrontEnd).ToList();
            }

            return cat.Where(i => i.ParentId == null || i.ParentId == Guid.Empty).Select(c => new MenuItem()
            {
                Id = c.Id,
                Title = contentLanguages.GetValue(c.Id, "Title"),
                SeoUrlFriendly = contentLanguages.GetValue(c.Id, "SeoUrlFriendly")
                       ,
                DisplayOrder = c.DisplayOrder,
                SubItems = cat.Where(i => i.ParentId == c.Id)
                       .Select(s => new MenuItem()
                       {
                           Id = s.Id,
                           Title = contentLanguages.GetValue(s.Id, "Title"),
                           DisplayOrder = s.DisplayOrder,
                           SeoUrlFriendly = contentLanguages.GetValue(s.Id, "SeoUrlFriendly")
                       }).OrderBy(i => i.DisplayOrder).ToList()
            }).OrderBy(i => i.DisplayOrder).ToList();
        }
    }

    public class MenuItem
    {
        public Guid Id;
        public string Title;

        public List<MenuItem> SubItems = new List<MenuItem>();
        public string SeoUrlFriendly;
        public int DisplayOrder;
    }
}