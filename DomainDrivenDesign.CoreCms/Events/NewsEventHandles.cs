using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsEventHandles : IEventHandle<NewsCreated>, IEventHandle<NewsUpdated>
            , IEventHandle<NewsAddedToCategory>, IEventHandle<NewsRemovedFromCategory>,
            IEventHandle<NewsDeleted>, IEventHandle<NewsPublished>, IEventHandle<NewsUnpublished>
    {
        public void Handle(NewsCreated e)
        {
            using (var db = new CoreCmsDbContext())
            {
                db.News.Add(new News()
                {
                    Id = e.Id,
                    ParentId = e.ParentId,
                    CreatedDate = e.CreatedDate,
                    AllowComment=e.AllowComment
                });
                db.SaveChanges();
            }
        }

        public void Handle(NewsUpdated e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.News.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.AllowComment = e.AllowComment;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(NewsAddedToCategory e)
        {

        }

        public void Handle(NewsRemovedFromCategory e)
        {

        }

        public void Handle(NewsDeleted e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.News.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Deleted = true;
                }
                db.SaveChanges();
            }
        }

        public void Handle(NewsPublished e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.News.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Published = true;
                }
                db.SaveChanges();
            }
        }

        public void Handle(NewsUnpublished e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.News.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Published = false;
                }
                db.SaveChanges();
            }
        }
    }
}
