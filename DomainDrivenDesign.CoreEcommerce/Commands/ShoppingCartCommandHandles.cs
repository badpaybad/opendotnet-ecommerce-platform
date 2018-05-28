using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class ShoppingCartCommandHandles : ICommandHandle<CreateShoppingCart>,
        ICommandHandle<AddProductToShoppingCart>, ICommandHandle<RemoveProductFromShoppingCart>
        , ICommandHandle<RemoveAllProductFromShoppingCart>
        , ICommandHandle<PreCalculateShoppingCart>, ICommandHandle<CheckoutShoppingCart>
        , ICommandHandle<EstimateShippingCostForShoppingCart>
        , ICommandHandle<CheckVoucherCodeForShoppingCart>
        , ICommandHandle<UpdatePaymentStatusForShoppingCart>
        , ICommandHandle<UpdateShippingStatusForShoppingCart>
        , ICommandHandle<ConfirmShoppingCartByCustomer>
        , ICommandHandle<AdminCloseOrderForShoppingCart>
        , ICommandHandle<AdminCancelOrderForShoppingCart>
        , ICommandHandle<ConfirmShoppingCartByAdmin>
    {
        ICqrsEventSourcingRepository<DomainShoppingCart> _repo = new CqrsEventSourcingRepository<DomainShoppingCart>(new EventPublisher());


        public void Handle(CreateShoppingCart c)
        {
            _repo.CreateNew(new DomainShoppingCart(c.Id, c.UserId, c.LanguageId, c.IpAddress, c.SiteDomainUrl));
        }

        public void Handle(AddProductToShoppingCart c)
        {
            _repo.GetDoSave(c.Id, obj => obj.AddProduct(c.ProductId, c.Quantity));
        }

        public void Handle(RemoveProductFromShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.RemoveProduct(c.ProductId, c.Quantity));
        }

        public void Handle(RemoveAllProductFromShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.RemoveAllProduct());
        }

        public void Handle(PreCalculateShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.PreCalculate());
        }

        public void Handle(CheckoutShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.Checkout(c.UserId, c.VoucherCode, c.AddressName, c.Email, c.Phone
               , c.ShippingMethodId, c.ReceivingTime, c.Address, c.AddressLatitude, c.AddressLongitude, c.PaymentMethodId, c.Message, c.LanguageId));
        }

        public void Handle(EstimateShippingCostForShoppingCart c)
        {
            _repo.GetDoSave(c.ShoppingCartId, o => o.CalculateShipping(c.Address, c.AddressLatitude, c.AddressLongitude, c.ShippingMethodId));
        }

        public void Handle(CheckVoucherCodeForShoppingCart c)
        {
            _repo.GetDoSave(c.ShoppingCartId, o => o.CalculateVoucher(c.VoucherCode, c.UserId));
        }

        public void Handle(UpdatePaymentStatusForShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.UpdatePaymentStatus(c.PayStatus));
        }

        public void Handle(UpdateShippingStatusForShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.UpdateShippingStatus(c.ShipStatus));
        }

        public void Handle(ConfirmShoppingCartByCustomer c)
        {
            _repo.GetDoSave(c.Id, o => o.CustomerConfirm(c.UserId));
        }

        public void Handle(AdminCloseOrderForShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.CloseOrder());
        }

        public void Handle(AdminCancelOrderForShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.CancelOrder());
        }

        public void Handle(ConfirmShoppingCartByAdmin c)
        {
            _repo.GetDoSave(c.Id, o => o.CustomerConfirm(c.UserId));
        }

        public void Handle(PackAndPrintShoppingCart c)
        {
            _repo.GetDoSave(c.Id, o => o.PrintLabel());
        }
    }

    public class PackAndPrintShoppingCart:AdminBaseCommand
    {
        public Guid Id { get; }
        public string OrderCode { get; }

        public PackAndPrintShoppingCart(Guid id,string orderCode, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            OrderCode = orderCode;
        }
    }
}
