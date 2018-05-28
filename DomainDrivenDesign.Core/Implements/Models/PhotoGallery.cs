using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("PhotoGallery")]
    public class PhotoGallery
    {
        [Key]
        public Guid PgId { get; set; }
        public Guid Id { get; set; }
        public string TableName { get; set; }
        [StringLength(2048)]
        public string UrlImage { get; set; }
        public short DisplayOrder { get; set; }
    }
}