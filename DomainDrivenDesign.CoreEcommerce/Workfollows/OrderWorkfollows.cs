using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Events;

namespace DomainDrivenDesign.CoreEcommerce.Workfollows
{
    public class OrderWorkfollows : IEventHandle<ShoppingCartCreatedOrderCode>
         , IEventHandle<PaymentTransactionSuccessed>
         , IEventHandle<PaymentTransactionFailed>
        ,IEventHandle<ShoppingCartCheckedout>
    {
        public void Handle(ShoppingCartCreatedOrderCode e)
        {
            var payTransId = Guid.NewGuid();
            MemoryMessageBuss.PushCommand(new CreatePaymentTransaction(payTransId, e.PaymentMethodId, e.OrderCode, e.CartTotal
                , e.IpAddress, e.LanguageId, e.SiteDomainUrl));

            if (!string.IsNullOrEmpty(e.VoucherCode))
            {
                Guid voucherCodeId;
                using (var db = new CoreEcommerceDbContext())
                {
                    voucherCodeId =
                        db.VoucherCodes.Where(
                                i => i.Code.Equals(e.VoucherCode, StringComparison.OrdinalIgnoreCase))
                            .Select(i => i.Id).FirstOrDefault();
                }
                if (voucherCodeId != Guid.Empty)
                {
                    MemoryMessageBuss.PushCommand(new ApplyVoucherCode(voucherCodeId, e.VoucherCode, e.OrderCode, e.UserId, e.VoucherValue));
                }
            }
        }

        public void Handle(ShoppingCartConfirmedByCustomer e)
        {
            ShoppingCart cart;
            PaymentTransaction trans;
            using (var db = new CoreEcommerceDbContext())
            {
                cart = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                 trans = db.PaymentTransactions.SingleOrDefault(i => i.OrderCode.Equals(cart.OrderCode, StringComparison.OrdinalIgnoreCase));
            }

            MemoryMessageBuss.PushCommand(new ProcessPaymentTransaction(trans.Id));

        }

        public void Handle(PaymentTransactionSuccessed e)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                var trans = db.PaymentTransactions.SingleOrDefault(i => i.Id == e.Id);
                cart = db.ShoppingCarts.SingleOrDefault(
                   i => i.OrderCode.Equals(trans.OrderCode, StringComparison.OrdinalIgnoreCase));
            }
            MemoryMessageBuss.PushCommand(new UpdatePaymentStatusForShoppingCart(cart.Id, Enums.ShoppingCartPayStatus.PaymentSuccess));
        }

        public void Handle(PaymentTransactionFailed e)
        {
            ShoppingCart cart;
            using (var db = new CoreEcommerceDbContext())
            {
                var trans = db.PaymentTransactions.SingleOrDefault(i => i.Id == e.Id);
                cart = db.ShoppingCarts.SingleOrDefault(
                    i => i.OrderCode.Equals(trans.OrderCode, StringComparison.OrdinalIgnoreCase));
            }
            MemoryMessageBuss.PushCommand(new UpdatePaymentStatusForShoppingCart(cart.Id, Enums.ShoppingCartPayStatus.PaymentFail));
        }

        public void Handle(ShoppingCartCheckedout e)
        {
            foreach (var cartItem in e.Items)
            {
                MemoryMessageBuss.PushCommand(new BuyProdcutByCustomer(cartItem.ProductId,e.UserId,cartItem.Quantity,e.Email,e.WebsiteUrl));
            }
        }
    }
}
