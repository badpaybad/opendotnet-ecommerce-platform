using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("VoucherCode")]
    public class VoucherCode
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(32)]
        public string Code { get; set; }
        public Guid VoucherMethodId { get; set; }
        public long Value { get; set; }
        public Guid AppliedForUserId { get; set; }
        public string AppliedForOrderCode { get; set; }
        public bool Applied { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}