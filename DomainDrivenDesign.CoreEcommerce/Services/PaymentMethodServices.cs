using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public interface IPaymentMethod
    {
        Guid Id { get; }

        string GetRedirectUrl(PaymentMethod config, Guid paymentTransactionId, string orderCode, long amount
            , string ipAddress, Guid languageId,string siteDomainUrl);

        Enums.ShoppingCartPayStatus QueryDr(Guid paymentTransactionId);

    }

    
}
