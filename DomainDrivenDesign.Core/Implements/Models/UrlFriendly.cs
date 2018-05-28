using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("UrlFriendly")]
   public class UrlFriendly
    {
        [Key]
        public Guid UfId { get; set; }

         
        public string TableName { get; set; }
         
        [StringLength(512)]
        public string UrlSegment { get; set; }
         
        public Guid Id { get; set; }

        [StringLength(512)]
        public string ControllerName { get; set; }
        [StringLength(512)]
        public string ActionName { get; set; }
    }
}
