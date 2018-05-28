using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreCms.Ef
{
    [Table("HomePageSection")]
    public class HomePageSection
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public short DisplayOrder { get; set; }
        [StringLength(2048)]
        public string HomePageSectionViewName { get; set; }
        public bool Published { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}