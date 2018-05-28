using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("PaymentTransaction")]
    public class PaymentTransaction
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PaymentMethodId { get; set; }
        public long Amount { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
        [StringLength(128)]
        public string OrderCode { get; set; }
        [StringLength(2048)]
        public string UrlRedirect { get; set; }

        public short Status { get; set; }
        [StringLength(1024)]
        public string IpAddress { get; set; }
    }
}