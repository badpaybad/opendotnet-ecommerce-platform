using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("ShoppingCartItem")]
    public class ShoppingCartItem
    {
        [Key]
        [Column(Order = 0)]
        public Guid ShoppingCartId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid ProductId { get; set; }
        public Guid ParentProductId { get; set; }
        public long UnitPrice { get; set; }
        public int Quantity { get; set; }
        public long TotalPrice { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }

        public long ProductDiscount { get; set; }
        public Guid ProductPromotionId { get; set; }
    }
}