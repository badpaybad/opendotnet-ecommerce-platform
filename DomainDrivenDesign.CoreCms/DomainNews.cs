using System;
using System.Collections.Generic;
using System.Linq;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Events;

namespace DomainDrivenDesign.CoreCms
{
    public class DomainNews : AggregateRoot
    {
        private string _title;
        private bool _published;
        private bool _allowComment;
        public override string Id { get; set; }

        public DomainNews()
        {

        }

        void Apply(NewsCreated e)
        {
            Id = e.Id.ToString();
            _title = e.Title;
            _allowComment = e.AllowComment;
        }

        void Apply(NewsUpdated e)
        {
            _title = e.Title;
            _allowComment = e.AllowComment;
        }

        void Apply(NewsPublished e)
        {
            Id = e.Id.ToString();
            _published = true;
        }
        void Apply(NewsUnpublished e)
        {
            Id = e.Id.ToString();
            _published = false;
        }

        public DomainNews(Guid id, string title, string shortDescription, string description, string urlImage, bool allowComment, Guid languageId, Guid parentId)
        {
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "ShortDescription", shortDescription, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "UrlImage", urlImage, "News"));

            var urlSegment = title.ToUrlSegment();
            var seoUrlFiendly = RefindSeoUrlFiendly(urlSegment, id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrlFiendly, "News"));
            ApplyChange(new UrlFriendlyCreated(seoUrlFiendly, "News", id, "News", "Detail"));
            ApplyChange(new NewsCreated(id, parentId, title,allowComment, DateTime.Now));

        }

        public void Update(bool allowComment, string title, string shortDescription, string description, string urlImage, Guid languageId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "ShortDescription", shortDescription, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "UrlImage", urlImage, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "News"));
            ApplyChange(new NewsUpdated(id,allowComment, title));
        }

        public void AddToCategory(Guid categoryId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new RelationShipUpdated(categoryId, id, categoryId, id, "Category", "News", 0));
            ApplyChange(new NewsAddedToCategory(id, categoryId));
        }

        public void RemoveFromCategory(Guid categoryId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new RelationShipUpdated(categoryId, id, Guid.Empty, id, "Category", "News", 0));
            ApplyChange(new NewsRemovedFromCategory(id, categoryId));
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new NewsDeleted(id));
        }

        public void UpdateSeo(string seokeywords, string seoDesscription, Guid languageId, string seoUrlFiendly = "")
        {
            var id = Guid.Parse(Id);

            if (string.IsNullOrEmpty(seoUrlFiendly))
            {
                seoUrlFiendly = _title.ToUrlSegment();
            }

            if (ExistedSeoUrlFiendly(seoUrlFiendly, id)) throw new Exception("Existed seo url friendly");

            seoUrlFiendly = RefindSeoUrlFiendly(seoUrlFiendly, id);

            ApplyChange(new UrlFriendlyCreated(seoUrlFiendly, "News", id, "News", "Detail"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrlFiendly, "News"));

            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoKeywords", seokeywords, "News"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoDescription", seoDesscription, "News"));
        }

        public void Publish()
        {
            var id = Guid.Parse(Id);

            ApplyChange(new NewsPublished(id));
        }

        public void Unpublish()
        {
            var id = Guid.Parse(Id);

            ApplyChange(new NewsUnpublished(id));
        }
        public void ChangeToCategories(List<Guid> categoryIds)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new RelationShipToRemoved(id, "News"));
            ApplyChange(new RelationShipAddedManyFromWithOneTo(categoryIds, id, "Category", "News"));

        }

        public void AddComment(string comment, string authorName, Guid userId, Guid parentCommentId)
        {
            if(!_allowComment) throw new Exception("Not allow comment");
            var id = Guid.Parse(Id);
            if (parentCommentId == Guid.Empty)
            {
                ApplyChange(new CommentAdded(id, Guid.NewGuid(), "News", authorName, comment, DateTime.Now, userId));
            }
            else
            {
               ApplyChange( new CommentReplied(id,Guid.NewGuid(), parentCommentId,"News",authorName,comment,DateTime.Now,userId));
            }
        }

        private string RefindSeoUrlFiendly(string seoUrlFiendly, Guid id)
        {
            using (var db = new CoreDbContext())
            {
                var existedSeoUrl =
                    db.ContentLanguages.Where(i => i.Id != id &&
                                                   i.ColumnValue.Equals(seoUrlFiendly, StringComparison.OrdinalIgnoreCase))
                        .Select(i => i.ColumnValue).FirstOrDefault();

                if (!string.IsNullOrEmpty(existedSeoUrl))
                {
                    seoUrlFiendly = existedSeoUrl + "-" + id.GetHashCode().ToString().Trim('-');
                }
            }

            if (string.IsNullOrEmpty(seoUrlFiendly)) seoUrlFiendly = id.ToString().ToLower();

            return seoUrlFiendly;
        }

        private bool ExistedSeoUrlFiendly(string seoUrlFiendly, Guid id)
        {
            using (var db = new CoreDbContext())
            {
                var existedSeoUrl =
                    db.ContentLanguages.Any(i => i.Id != id &&
                                                   i.ColumnValue.Equals(seoUrlFiendly, StringComparison.OrdinalIgnoreCase));

                return existedSeoUrl;
            }

        }

        

    }

}