using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryEventHandles : IEventHandle<CategoryCreated>,
         IEventHandle<CategoryRootChanged>, IEventHandle<CategoryDeleted>
         , IEventHandle<CategoryUpdated>, IEventHandle<CategoryChangedDisplayOrder>
    {
        public void Handle(CategoryCreated e)
        {
            using (var db = new CoreCmsDbContext())
            {
                db.Categories.Add(new Category()
                {
                    Deleted = false,
                    Id = e.Id,
                    ParentId = e.ParentId,
                    IsSinglePage = e.IsSinglePage,
                    ShowInFrontEnd = e.ShowInFrontEnd,
                    CategoryViewName = e.CategoryViewName,
                    Type = (short)e.Type
                });
                db.SaveChanges();
            }
        }

        public void Handle(CategoryRootChanged e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.Categories.SingleOrDefault(i => i.Id.Equals(e.Id));
                if (temp != null) temp.ParentId = e.ParentId;
                db.SaveChanges();
            }
        }

        public void Handle(CategoryDeleted e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.Categories.SingleOrDefault(i => i.Id.Equals(e.Id));
                if (temp != null) temp.Deleted = true;
                db.SaveChanges();
            }
        }

        public void Handle(CategoryUpdated e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.Categories.SingleOrDefault(i => i.Id.Equals(e.Id));
                if (temp != null)
                {
                    temp.IsSinglePage = e.IsSinglePage;
                    temp.ShowInFrontEnd = e.ShowInFrontEnd;
                    temp.CategoryViewName = e.CategoryViewName;
                    temp.Type = (short)e.Type;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(CategoryChangedDisplayOrder e)
        {
            using (var db = new CoreCmsDbContext())
            {
                var temp = db.Categories.SingleOrDefault(i => i.Id.Equals(e.Id));
                if (temp != null) temp.DisplayOrder = e.DisplayOrder;
                db.SaveChanges();
            }
        }
    }
}
