using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class ChangeNewsToCategories : AdminBaseCommand
    {
        public Guid Id { get; }
        public List<Guid> CategoryIds { get; }

        public ChangeNewsToCategories(Guid id, List<Guid> categoryIds
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            CategoryIds = categoryIds;
        }
    }
    
    public class AddCommentToNews : ICommand
    {
        public AddCommentToNews(Guid newsId, string comment, string authorName, Guid userId, Guid commentParentId)
        {
            NewsId = newsId;
            Comment = comment;
            AuthorName = authorName;
            UserId = userId;
            CommentParentId = commentParentId;
        }

        public Guid NewsId { get; private set; }
        public string Comment { get; set; }
        public string AuthorName { get; set; }
        public Guid UserId { get; private set; }
        public Guid CommentParentId { get; private set; }
    }
}