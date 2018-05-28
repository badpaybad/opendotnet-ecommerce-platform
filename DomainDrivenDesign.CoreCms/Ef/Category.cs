using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreCms.Ef
{
    [Table("Category")]
    public class Category
    {
        [Key]
         
        public Guid Id { get; set; }
         
        public Guid ParentId { get; set; }
         
        public bool Deleted { get; set; }
         
        public int DisplayOrder { get; set; }
         
        public bool ShowInFrontEnd { get; set; }
         
        public bool IsSinglePage { get; set; }
        [StringLength(2048)]
        public string CategoryViewName { get; set; }

        public short Type { get; set; }
    }
}
