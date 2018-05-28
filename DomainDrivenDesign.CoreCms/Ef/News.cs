using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreCms.Ef
{
    [Table("News")]
    public class News
    {
        [Key]
         
        public Guid Id { get; set; }
         
        public Guid ParentId { get; set; }
         
        public bool Deleted { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
         
        public bool Published { get; set; }
        public bool AllowComment { get; set; }
    }
}