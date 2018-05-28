using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class PaymentTransactionFailed : IEvent
    {
        public Guid Id { get; }

        public PaymentTransactionFailed(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
    public class PaymentTransactionProcessed : IEvent
    {
        public Guid Id { get; }

        public PaymentTransactionProcessed(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class PaymentTransactionSuccessed : IEvent
    {
        public Guid Id { get; }

        public PaymentTransactionSuccessed(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class PaymentTransactionCreated : IEvent
    {
        public Guid Id { get; }
        public string OrderCode { get; }
        public long Amount { get; }
        public Guid PaymentMethodId { get; }
        public DateTime CreatedDate { get; }
        public string UrlRedirect { get; }
        public string IpAddress { get; }

        public PaymentTransactionCreated(Guid id, string orderCode, long amount, Guid paymentMethodId, DateTime createdDate, string urlRedirect,string ipAddress)
        {
            Id = id;
            OrderCode = orderCode;
            Amount = amount;
            PaymentMethodId = paymentMethodId;
            CreatedDate = createdDate;
            UrlRedirect = urlRedirect;
            IpAddress = ipAddress;
        }

        public long Version { get; set; }
    }
}
