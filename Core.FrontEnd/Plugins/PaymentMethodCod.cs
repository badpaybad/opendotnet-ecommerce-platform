using System;
using System.Linq;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class PaymentMethodCod : IPaymentMethod
    {
        public Guid Id { get { return Guid.Parse("5bd56988-36a6-4522-843d-64cc7ebececf"); } }
        public string GetRedirectUrl(PaymentMethod config, Guid paymentTransactionId, string orderCode, long amount 
            ,string ipAddres, Guid languageId, string siteDomainUrl)
        {
            return "/Payment/Cod?orderCode=" + orderCode;
        }

        public Enums.ShoppingCartPayStatus QueryDr(Guid paymentTransactionId)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.PaymentTransactions.SingleOrDefault(i => i.Id== paymentTransactionId);
                if(temp==null ) return Enums.ShoppingCartPayStatus.PaymentCreated;

                return (Enums.ShoppingCartPayStatus) temp.Status;
            }
        }
    }
}