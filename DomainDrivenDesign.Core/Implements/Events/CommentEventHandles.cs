using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class CommentEventHandles : IEventHandle<CommentAdded>, IEventHandle<CommentReplied>
    {
        public void Handle(CommentAdded e)
        {
            using (var db = new CoreDbContext())
            {
                db.Comments.Add(new Comment()
                {
                    Id = e.Id,
                    CommentId = e.CommentId,
                    CreatedDate = e.CreatedDate,
                    TableName = e.TableName,
                    Content = e.Content,
                    UserId = e.UserId,
                    AuthorName = e.AuthorName
                });
                db.SaveChanges();
            }
        }

        public void Handle(CommentReplied e)
        {
            using (var db = new CoreDbContext())
            {
                db.Comments.Add(new Comment()
                {
                    Id = e.Id,
                    CommentId = e.CommentId,
                    CreatedDate = e.CreatedDate,
                    TableName = e.TableName,
                    Content = e.Content,
                    UserId = e.UserId,
                    AuthorName = e.AuthorName,
                    CommentParentId = e.ToCommentId
                });
                db.SaveChanges();
            }
        }
    }

    public class CommentAdded : IEvent
    {
        public CommentAdded(Guid id, Guid commentId, string tableName, string authorName, string content, DateTime createdDate, Guid userId)
        {
            Id = id;
            CommentId = commentId;
            TableName = tableName;
            AuthorName = authorName;
            Content = content;
            CreatedDate = createdDate;
            UserId = userId;
        }

        public long Version { get; set; }
        public Guid Id { get; }
        public Guid CommentId { get; }
        public string TableName { get; }
        public string AuthorName { get; }
        public string Content { get; }
        public DateTime CreatedDate { get; }
        public Guid UserId { get; }
    }

    public class CommentReplied : IEvent
    {
        public CommentReplied(Guid id, Guid commentId, Guid toCommentId, string tableName, string authorName, string content, DateTime createdDate, Guid userId)
        {
            Id = id;
            CommentId = commentId;
            ToCommentId = toCommentId;
            TableName = tableName;
            AuthorName = authorName;
            Content = content;
            CreatedDate = createdDate;
            UserId = userId;
        }

        public Guid Id { get; }
        public Guid CommentId { get; }
        public Guid ToCommentId { get; }
        public string TableName { get; }
        public string AuthorName { get; }
        public string Content { get; }
        public DateTime CreatedDate { get; }
        public Guid UserId { get; }
        public long Version { get; set; }
    }
}
