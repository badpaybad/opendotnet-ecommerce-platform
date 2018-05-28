using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class CategoryComandHandles : ICommandHandle<CreateCategory>,
        ICommandHandle<UpdateCategory>, ICommandHandle<ChangeRootCategory>,
        ICommandHandle<DeleteCategory>, ICommandHandle<ChangeCategoryDisplayOrder>
    {
        ICqrsEventSourcingRepository<DomainCategory> _repo
            =new CqrsEventSourcingRepository<DomainCategory>(new EventPublisher());

        public void Handle(CreateCategory c)
        {
            _repo.CreateNew(new DomainCategory(c.Id,c.IsSinglePage,c.ShowInFrontEnd,c.Title
                ,c.SeoKeywords,c.SeoDescription
                , c.CategoryViewName,c.IconUrl,c.Description,c.LanguageId,c.ParentId,c.Type));
        }

        public void Handle(UpdateCategory c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.Update(c.IsSinglePage,c.ShowInFrontEnd, c.Title
                    , c.SeoKeywords, c.SeoDescription
                    , c.CategoryViewName,c.IconUrl,c.Description,c.LanguageId,c.Type);
            });
        }

        public void Handle(ChangeRootCategory c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.ChangeRoot(c.ParentId);
            });
        }

        public void Handle(DeleteCategory c)
        {
            _repo.GetDoSave(c.Id,obj=>obj.Delete());
        }

        public void Handle(ChangeCategoryDisplayOrder c)
        {
            _repo.GetDoSave(c.Id,obj=>obj.ChangeDisplayOrder(c.DisplayOrder));
        }
    }
}
