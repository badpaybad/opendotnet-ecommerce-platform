using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("ShoppingCartShippingAddress")]
    public class ShoppingCartShippingAddress
    {
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public string AddressName { get; set; }
        public string Address { get; set; }
        public double AddressLatitude { get; set; }
        public double AddressLongitude { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}