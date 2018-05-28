using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class HomePageSettingsCommandHandles : ICommandHandle<CreateHomePageSection>, ICommandHandle<UpdateHomePageSection>
         , ICommandHandle<PublishHomePageSection>, ICommandHandle<DeleteHomPageSection>
    {
        EventPublisher _eventPublisher=new EventPublisher();

        public void Handle(CreateHomePageSection c)
        {
            var h = new HomePageSection();

            using (var db = new CoreCmsDbContext())
            {
                h.CategoryId = c.CategoryId;
                h.Id = c.Id;
                h.CreatedDate = DateTime.Now;
                h.DisplayOrder = c.DisplayOrder;
                h.HomePageSectionViewName = c.ViewName;

                db.HomePageSections.Add(h);
                db.SaveChanges();
            }
            _eventPublisher.Publish(new ContentLanguageUpdated(c.Id,c.LanguageId,"Title",c.Title, "HomePageSection"));
        }

        public void Handle(UpdateHomePageSection c)
        {
            HomePageSection h = null;
            using (var db = new CoreCmsDbContext())
            {
                h = db.HomePageSections.FirstOrDefault(i => i.Id == c.Id);
                if (h == null) return;
                h.CategoryId = c.CategoryId;

                h.DisplayOrder = c.DisplayOrder;
                h.HomePageSectionViewName = c.ViewName;
                db.SaveChanges();
            }
            _eventPublisher.Publish(new ContentLanguageUpdated(c.Id, c.LanguageId, "Title", c.Title, "HomePageSection"));
        }

        public void Handle(PublishHomePageSection c)
        {
            HomePageSection h = null;
            using (var db = new CoreCmsDbContext())
            {
                h = db.HomePageSections.FirstOrDefault(i => i.Id == c.Id);
                if (h == null) return;
                h.Published = c.IsPublish;
                db.SaveChanges();
            }
        }

        public void Handle(DeleteHomPageSection c)
        {
            HomePageSection h = null;
            using (var db = new CoreCmsDbContext())
            {
                h = db.HomePageSections.FirstOrDefault(i => i.Id == c.Id);
                if (h == null) return;
                db.HomePageSections.Remove(h);
                db.SaveChanges();
            }

            _eventPublisher.Publish(new ContentLanguageDeleted(c.Id));
        }
    }
}
