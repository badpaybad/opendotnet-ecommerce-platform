using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        public Guid TagId { get; set; }
        public Guid Id { get; set; }
        [StringLength(128)]
        public string TableName { get; set; }
        [StringLength(256)]
        public string TagString { get; set; }
        public long Counter { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}
