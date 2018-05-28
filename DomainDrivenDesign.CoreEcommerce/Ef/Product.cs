using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("Product")]
    public class Product
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public long Price { get; set; }
        public long Quantity { get; set; }
        [StringLength(128)]
        public string ProductCode { get; set; }

        public bool AllowComment { get; set; }
        public bool IsCombo { get; set; }

        public int Gram { get; set; }
        public int Calorie { get; set; }
    }

    [Table("ProductInCombo")]
    public class ProductInCombo
    {
        [Key]
        [Column(Order=0)]
        public Guid ProductId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid ProductComboId { get; set; }
    }

    [Table("ProductPromotion")]
    public class ProductPromotion
    {
        public Guid Id { get; set; }
        public long ProductQuantity { get;  set; }
        public long DiscountValue { get;  set; }
        public DateTime FromDate { get;  set; }
        public DateTime ToDate { get;  set; }
        public DateTime CreatedDate { get; set; }
    }

}