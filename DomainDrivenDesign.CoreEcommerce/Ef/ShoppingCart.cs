using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }
        public short Status { get; set; }
        public Guid UserId { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
        public long CartTotal { get; set; }
        public long ShippingFee { get; set; }
        public long CartDiscount { get; set; }
        public long CartSubTotal { get; set; }
        public long CartTax { get; set; }
        public Guid ShippingMethodId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public long VoucherValue { get; set; }
        [StringLength(32)]
        public string VoucherCode { get; set; }
        [StringLength(128)]
        public string OrderCode { get; set; }

        public short PaymentStatus { get; set; }
        public short PackingStatus { get; set; }
        public short ShippingStatus { get; set; }
        public DateTime ReceivingTime { get; set; }
        public Guid OrderPromotionId { get; set; }
    }

    [Table("OrderPromotion")]
    public class OrderPromotion
    {
        [Key]
        public Guid Id { get; set; }
        public long AmountToDiscount { get; set; }
        public long DiscountAmount { get; set; }
        public bool FreeShip { get; set; }
        public bool Actived { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}