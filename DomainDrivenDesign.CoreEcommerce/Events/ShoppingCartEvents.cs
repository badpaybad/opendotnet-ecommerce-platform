using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{

    public class ShoppingCartShippingAddressCreated : IEvent
    {
        public Guid Id { get; }
        public Guid UserIdValue { get; }
        public string Phone { get; }
        public string Email { get; }
        public string AddressName { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }
        public string Message { get; }
        public Guid ShippingMethodId { get; }

        public ShoppingCartShippingAddressCreated(Guid id, Guid userIdValue,string phone, string email
            , string addressName, string address, double addressLatitude, double addressLongitude, string message
            ,Guid shippingMethodId)
        {
            Id = id;
            UserIdValue = userIdValue;
            Phone = phone;
            Email = email;
            AddressName = addressName;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
            Message = message;
            ShippingMethodId = shippingMethodId;
        }

        public long Version { get; set; }
    }
    public class ShoppingCartCreatedOrderCode : IEvent
    {
        public Guid Id { get; }
        public Guid PaymentMethodId { get; }
        public string OrderCode { get; }
        public long CartTotal { get; }
        public string VoucherCode { get; }
        public long VoucherValue { get; }
        public Guid UserId { get; }
        public string IpAddress { get; }
        public Guid LanguageId { get; private set; }
        public string SiteDomainUrl { get; }

        public ShoppingCartCreatedOrderCode(Guid id, Guid paymentMethodId, string orderCode, long cartTotal
            , string voucherCode,long voucherValue,Guid userId, string ipAddress, Guid languageId,string siteDomainUrl)
        {
            Id = id;
            PaymentMethodId = paymentMethodId;
            OrderCode = orderCode;
            CartTotal = cartTotal;
            VoucherCode = voucherCode;
            VoucherValue = voucherValue;
            UserId = userId;
            IpAddress = ipAddress;
            LanguageId = languageId;
            SiteDomainUrl = siteDomainUrl;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartCheckedout : IEvent
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public List<ShoppingCartItem> Items { get; }
        public string AddressName { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Message { get; }
        public long CartSubTotal { get; }
        public long ShippingFee { get; }
        public long VoucherValue { get; }
        public long CartDiscount { get; }
        public long CartTotal { get; }
        public long CartTax { get; }
        public Guid LanguageId { get; }
        public string VoucherCode { get; }
        public DateTime CheckedoutDate { get; }

        public ShoppingCartCheckedout(Guid id, Guid userId, List<ShoppingCartItem> items
            , string addressName, string address, double addressLatitude, double addressLongitude
            , string email, string phone, string message
            , long cartSubTotal, long shippingFee, long voucherValue, long cartDiscount, long  cartTotal, long cartTax
            , Guid languageId, string voucherCode, DateTime checkedoutDate, string websiteUrl)
        {
            Id = id;
            UserId = userId;
            Items = items;
            AddressName = addressName;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
            Email = email;
            Phone = phone;
            Message = message;
            CartSubTotal = cartSubTotal;
            ShippingFee = shippingFee;
            VoucherValue = voucherValue;
            CartDiscount = cartDiscount;
            CartTotal = cartTotal;
            CartTax = cartTax;
            LanguageId = languageId;
            VoucherCode = voucherCode;
            CheckedoutDate = checkedoutDate;
            WebsiteUrl = websiteUrl;
        }

        public long Version { get; set; }
        public string WebsiteUrl { get; private set; }
    }

    public class ShoppingCartPreCalculated : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public long CartTax { get; }
        public long CartDiscount { get; }
        public long CartSubTotal { get; }
        public long CartTotal { get; }

        public ShoppingCartPreCalculated(Guid id, DateTime createdDate, long cartTax, long cartDiscount, long cartSubTotal, long cartTotal)
        {
            Id = id;
            CreatedDate = createdDate;
            CartTax = cartTax;
            CartDiscount = cartDiscount;
            CartSubTotal = cartSubTotal;
            CartTotal = cartTotal;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartRemovedAllProduct : IEvent
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }

        public ShoppingCartRemovedAllProduct(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartRemovedProduct : IEvent
    {
        public Guid Id { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public long TotalPrice { get; }
        public DateTime CreatedDate { get; }

        public ShoppingCartRemovedProduct(Guid id, Guid productId, int quantity,long totalPrice, DateTime createdDate)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            TotalPrice = totalPrice;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartAddedProduct : IEvent
    {
        public Guid Id { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public long UnitPrice { get; }
        public long TotalPrice { get; }
        public DateTime CreatedDate { get; }

        public ShoppingCartAddedProduct(Guid id, Guid productId, int quantity, long unitPrice, long totalPrice, DateTime createdDate)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }
    
    public class ShoppingCartCreated : IEvent
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid LanguageId { get; }
        public DateTime CreatedDate { get; }
        public DateTime ReceivingTime { get; }
        public string IpAddress { get; }
        public string SiteDomainUrl { get; }

        public ShoppingCartCreated(Guid id, Guid userId, Guid languageId, DateTime createdDate, DateTime receivingTime,string ipAddress, string siteDomainUrl)
        {
            Id = id;
            UserId = userId;
            LanguageId = languageId;
            CreatedDate = createdDate;
            ReceivingTime = receivingTime;
            IpAddress = ipAddress;
            SiteDomainUrl = siteDomainUrl;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartCalculatedShipping : IEvent
    {
        public Guid Id { get; }
        public Guid ShippingMethodId { get; }
        public long ShippingFee { get; }

        public ShoppingCartCalculatedShipping(Guid id, Guid shippingMethodId, long shippingFee)
        {
            Id = id;
            ShippingMethodId = shippingMethodId;
            ShippingFee = shippingFee;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartCalculatedVoucherCode : IEvent
    {
        public Guid Id { get; }
        public string VoucherCode { get; }
        public long VoucherValue { get; }
        public Guid UserId { get; }

        public ShoppingCartCalculatedVoucherCode(Guid id, string voucherCode, long voucherValue,Guid userId)
        {
            Id = id;
            VoucherCode = voucherCode;
            VoucherValue = voucherValue;
            UserId = userId;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartUpdatedShipStatus : IEvent
    {
        public Guid Id { get; }
        public short ShipStatus { get; }

        public ShoppingCartUpdatedShipStatus(Guid id, short shipStatus)
        {
            Id = id;
            ShipStatus = shipStatus;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartUpdatedPayStatus : IEvent
    {
        public Guid Id { get; }
        public string OrderCode { get; }
        public short PayStatus { get; }
        public Guid LanguageId { get; }
        public string WebsiteUrl { get; }

        public ShoppingCartUpdatedPayStatus(Guid id, string orderCode, short payStatus, Guid languageId,string websiteUrl)
        {
            Id = id;
            OrderCode = orderCode;
            PayStatus = payStatus;
            LanguageId = languageId;
            WebsiteUrl = websiteUrl;
        }

        public long Version { get; set; }
    }

    public class ShoppingCartConfirmedByCustomer:IEvent
    {
        public Guid Id { get; }
        public string OrderCode { get; }
        public Guid UserId { get; }
        public DateTime DateConfirmed { get; }

        public ShoppingCartConfirmedByCustomer(Guid id,string orderCode, Guid userId, DateTime dateConfirmed)
        {
            Id = id;
            OrderCode = orderCode;
            UserId = userId;
            DateConfirmed = dateConfirmed;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartCanceledOrder : IEvent
    {
        public Guid Id { get; }

        public ShoppingCartCanceledOrder(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartClosedOrder : IEvent
    {
        public Guid Id { get; }

        public ShoppingCartClosedOrder(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartItemAppliedProductPromotion : IEvent
    {
        public Guid Id { get; }
        public Guid ProductId { get; }
        public Guid ProductPromotionId { get; }
        public int Quantity { get; }
        public long ProductDiscount { get; }

        public ShoppingCartItemAppliedProductPromotion(Guid id, Guid productId, Guid productPromotionId
            , int quantity, long productDiscount)
        {
            Id = id;
            ProductId = productId;
            ProductPromotionId = productPromotionId;
            Quantity = quantity;
            ProductDiscount = productDiscount;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartPromotionCalulatedForOrderShipping : IEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid OrderPromotionId { get; }

        public ShoppingCartPromotionCalulatedForOrderShipping(Guid shoppingCartId, Guid orderPromotionId)
        {
            ShoppingCartId = shoppingCartId;
            OrderPromotionId = orderPromotionId;
        }

        public long Version { get; set; }
    }



    public class ShoppingCartPromotionCalulatedForOrderDiscount : IEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid OrderPromotionId { get; }

        public ShoppingCartPromotionCalulatedForOrderDiscount(Guid shoppingCartId, Guid orderPromotionId)
        {
            ShoppingCartId = shoppingCartId;
            OrderPromotionId = orderPromotionId;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartPromotionAppliedToOrder : IEvent
    {
        public Guid Id { get; }
        public string OrderCode { get; }
        public Guid OrderPromotionIdForDiscount { get; }
        public Guid OrderPromotionIdForShipping { get; }
        public long CartDiscount { get; }
        public long ShippingFee { get; }

        public ShoppingCartPromotionAppliedToOrder(Guid id, string orderCode, Guid orderPromotionIdForDiscount, Guid orderPromotionIdForShipping, long cartDiscount, long shippingFee)
        {
            Id = id;
            OrderCode = orderCode;
            OrderPromotionIdForDiscount = orderPromotionIdForDiscount;
            OrderPromotionIdForShipping = orderPromotionIdForShipping;
            CartDiscount = cartDiscount;
            ShippingFee = shippingFee;
        }

        public long Version { get; set; }
    }


    public class ShoppingCartReceivingTimeUpdated : IEvent
    {
        public Guid Id { get; }
        public DateTime ReceivingTime { get; }

        public ShoppingCartReceivingTimeUpdated(Guid id, DateTime receivingTime)
        {
            Id = id;
            ReceivingTime = receivingTime;
        }

        public long Version { get; set; }
    }
}
