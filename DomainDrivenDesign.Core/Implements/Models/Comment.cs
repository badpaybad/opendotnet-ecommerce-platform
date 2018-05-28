using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; }
        public Guid CommentParentId { get; set; }
        public Guid Id { get; set; }
        [StringLength(128)]
        public string TableName { get; set; }
        [StringLength(256)]
        public string AuthorName { get; set; }
        public string Content { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
        public Guid UserId { get; set; }
    }
}
