using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class NewsCommandHandles : ICommandHandle<CreateNews>, ICommandHandle<UpdateNews>
         , ICommandHandle<AddNewsToCategory>, ICommandHandle<RemoveNewsFromCategory>,
         ICommandHandle<DeleteNews>,ICommandHandle<UpdateNewsForSeo>,
        ICommandHandle<PublishNews>, ICommandHandle<ChangeNewsToCategories>
        ,ICommandHandle<AddCommentToNews>
    {
        ICqrsEventSourcingRepository<DomainNews> _repo = new CqrsEventSourcingRepository<DomainNews>(new EventPublisher());

        public void Handle(CreateNews c)
        {
            _repo.CreateNew(new DomainNews(c.Id, c.Title, c.ShortDesciption, c.Description,c.UrlImage,c.AllowComment, c.LanguageId, c.ParentId));

        }

        public void Handle(UpdateNews c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.Update(c.AllowComment, c.Title, c.ShortDesciption, c.Description,c.UrlImage, c.LanguageId);
            });
        }

        public void Handle(RemoveNewsFromCategory c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.RemoveFromCategory(c.CategoryId);
            });
        }

        public void Handle(AddNewsToCategory c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.AddToCategory(c.CategoryId);
            });
        }

        public void Handle(DeleteNews c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.Delete();
            });
        }

        public void Handle(UpdateNewsForSeo c)
        {
            _repo.GetDoSave(c.Id, obj =>
            {
                obj.UpdateSeo(c.SeoKeywords,c.SeoDescription,c.LanguageId,c.SeoUrlFriendly);
            });
        }


        public void Handle(PublishNews c)
        {
            if(c.IsPublish)
            { _repo.GetDoSave(c.Id,obj=>obj.Publish());}
            else
            {
                _repo.GetDoSave(c.Id, obj => obj.Unpublish());
            }
        }

        public void Handle(ChangeNewsToCategories c)
        {
            _repo.GetDoSave(c.Id,obj=>obj.ChangeToCategories(c.CategoryIds));
        }


        public void Handle(AddCommentToNews c)
        {
            _repo.GetDoSave(c.NewsId,o=>o.AddComment(c.Comment,c.AuthorName,c.UserId, c.CommentParentId));
        }

       
    }
    
}
