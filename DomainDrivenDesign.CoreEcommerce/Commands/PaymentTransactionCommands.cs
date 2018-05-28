using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{

    public class FailPaymentTransaction : ICommand
    {
        public Guid Id { get; }

        public FailPaymentTransaction(Guid id)
        {
            Id = id;
        }
    }

    public class SuccessPaymentTransaction : ICommand
    {
        public Guid Id { get; }

        public SuccessPaymentTransaction(Guid id)
        {
            Id = id;
        }
    }


    public class AdminFailPaymentTransaction : AdminBaseCommand
    {
        public Guid Id { get; }

        public AdminFailPaymentTransaction(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }
    public class AdminSuccessPaymentTransaction : AdminBaseCommand
    {
        public Guid Id { get; }

        public AdminSuccessPaymentTransaction(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }


    public class ProcessPaymentTransaction : ICommand
    {
        public Guid Id { get; }

        public ProcessPaymentTransaction(Guid id)
        {
            Id = id;
        }
    }

    public class CreatePaymentTransaction : ICommand
    {
        public Guid Id { get; }
        public Guid PaymentMethodId { get; }
        public string OrderCode { get; }
        public long Amount { get; }
        public string IpAddress { get; private set; }
        public Guid LanguageId { get; }
        public string SiteDomainUrl { get; }

        public CreatePaymentTransaction(Guid id, Guid paymentMethodId, string orderCode, long amount,
            string ipAddress, Guid languageId,string siteDomainUrl)
        {
            Id = id;
            PaymentMethodId = paymentMethodId;
            OrderCode = orderCode;
            Amount = amount;
            IpAddress = ipAddress;
            LanguageId = languageId;
            SiteDomainUrl = siteDomainUrl;
        }
    }
}
