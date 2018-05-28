using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("ContentLanguage")]
    public class ContentLanguage
    {
        [Key]
        [Column(Order = 0)]
        
        public Guid Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid LanguageId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(128)]
       
        public string ColumnName { get; set; }
       
        public string ColumnValue { get; set; }
       
        [Key]
        [Column(Order = 3)]
        [StringLength(128)]
        public string TableName { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}