using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{

    public class CreateShoppingCart : ICommand
    {
        public Guid Id { get; }
        public Guid? UserId { get; }
        public Guid LanguageId { get; }
        public string IpAddress { get; }
        public string SiteDomainUrl { get; }

        public CreateShoppingCart(Guid id, Guid? userId, Guid languageId, string ipAddress,string siteDomainUrl)
        {
            Id = id;
            UserId = userId;
            LanguageId = languageId;
            IpAddress = ipAddress;
            SiteDomainUrl = siteDomainUrl;
        }
    }

    public class AddProductToShoppingCart : ICommand
    {
        public Guid Id { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public AddProductToShoppingCart(Guid id, Guid productId, int quantity)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveProductFromShoppingCart : ICommand
    {
        public Guid Id { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public RemoveProductFromShoppingCart(Guid id, Guid productId, int quantity)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveAllProductFromShoppingCart : ICommand
    {
        public Guid Id { get; }

        public RemoveAllProductFromShoppingCart(Guid id)
        {
            Id = id;
        }
    }

    public class PreCalculateShoppingCart : ICommand
    {
        public Guid Id { get; }

        public PreCalculateShoppingCart(Guid id)
        {
            Id = id;
        }
    }

    public class CheckoutShoppingCart : ICommand
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid PaymentMethodId { get; }
        public Guid ShippingMethodId { get; }
        public string AddressName { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }
        public string VoucherCode { get; }
        public string Phone { get; }
        public string Email { get; }
        public string Message { get; }
        public Guid LanguageId { get; }
        public DateTime ReceivingTime { get; private set; }


        public CheckoutShoppingCart(Guid id, Guid userId, Guid paymentMethodId, Guid shippingMethodId
            , string addressName, DateTime receivingTime, string address, double addressLatitude, double addressLongitude
            , string voucherCode, string phone, string email, string message
            , Guid languageId)
        {
            Id = id;
            UserId = userId;
            PaymentMethodId = paymentMethodId;
            ShippingMethodId = shippingMethodId;
            AddressName = addressName;
            ReceivingTime = receivingTime;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
            VoucherCode = voucherCode;
            Phone = phone;
            Email = email;
            Message = message;
            LanguageId = languageId;
        }
    }


    public class EstimateShippingCostForShoppingCart : ICommand
    {
        public Guid ShoppingCartId { get; }
        public Guid ShippingMethodId { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }

        public EstimateShippingCostForShoppingCart(Guid shoppingCartId, Guid shippingMethodId
            , string address, double addressLatitude, double addressLongitude)
        {
            ShoppingCartId = shoppingCartId;
            ShippingMethodId = shippingMethodId;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
        }
    }


    public class CheckVoucherCodeForShoppingCart : ICommand
    {
        public Guid ShoppingCartId { get; }
        public string VoucherCode { get; }
        public Guid UserId { get; }

        public CheckVoucherCodeForShoppingCart(Guid shoppingCartId, string voucherCode, Guid userId)
        {
            ShoppingCartId = shoppingCartId;
            VoucherCode = voucherCode;
            UserId = userId;
        }
    }


    public class UpdateShippingStatusForShoppingCart : AdminBaseCommand
    {
        public Guid Id { get; }
        public Enums.ShoppingCartShipStatus ShipStatus { get; }
        public string Note { get; }

        public UpdateShippingStatusForShoppingCart(Guid id, Enums.ShoppingCartShipStatus shipStatus, string note,Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            ShipStatus = shipStatus;
            Note = note;
        }
        
    }

    public class UpdatePaymentStatusForShoppingCart : ICommand
    {
        public UpdatePaymentStatusForShoppingCart(Guid id, Enums.ShoppingCartPayStatus payStatus)
        {
            Id = id;
            PayStatus = payStatus;
        }

        public Guid Id { get; private set; }
        public Enums.ShoppingCartPayStatus PayStatus { get; private set; }
    }

    public class ConfirmShoppingCartByCustomer : ICommand
    {
        public Guid Id { get; }
        public Guid UserId { get; }

        public ConfirmShoppingCartByCustomer(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }


    public class ConfirmShoppingCartByAdmin : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Note { get; }

        public ConfirmShoppingCartByAdmin(Guid id,string note, Guid userId, DateTime createdDate):base(userId,  createdDate)
        {
            Id = id;
            Note = note;
        }
    }


    public class AdminCloseOrderForShoppingCart : AdminBaseCommand
    {
        public Guid Id { get; }

        public AdminCloseOrderForShoppingCart(Guid id,Guid userId,DateTime createdDate):base(userId,createdDate)
        {
            Id = id;
        }
    }

    public class AdminCancelOrderForShoppingCart : AdminBaseCommand
    {
        public Guid Id { get; }

        public AdminCancelOrderForShoppingCart(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }
}
