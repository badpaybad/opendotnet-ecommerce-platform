using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    public class CoreEcommerceDbContext : CoreCmsDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInCombo> ProductInCombos { get; set; }
        public DbSet<ProductPromotion> ProductPromotions { get; set; }
        public DbSet<PhotoGallery> PhotoGalleries { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<OrderPromotion> OrderPromotions { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<VoucherCode> VoucherCodes { get; set; }
        public DbSet<VoucherMethod> VoucherMethods { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<ShoppingCartShippingAddress> ShoppingCartShippingAddresses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
    }

    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(2048)]
        public string AddressName { get; set; }
        [StringLength(2048)]
        public string Address { get; set; }
        public double AddressLatitude { get; set; }
        public double AddressLongitude { get; set; }
        [StringLength(2048)]
        public string Phone { get; set; }
        [StringLength(2048)]
        public string Email { get; set; }
        [StringLength(2048)]
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
