using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{

    public class VoucherCodeDeleted : IEvent
    {
        public Guid Id { get; }

        public VoucherCodeDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class VoucherCodeApplied : IEvent
    {
        public Guid Id { get; }
        public string Code { get; }
        public string OrderCode { get; }
        public Guid UserId { get; }
        public long ValueApply { get; }

        public VoucherCodeApplied(Guid id, string code, string orderCode, Guid userId, long valueApply)
        {
            Id = id;
            Code = code;
            OrderCode = orderCode;
            UserId = userId;
            ValueApply = valueApply;
        }

        public long Version { get; set; }
    }

    public class VoucherCodeCreated : IEvent
    {
        public Guid Id { get; }
        public string Code { get; }
        public long Value { get; }
        public Guid MethodId { get; }
        public DateTime CreatedDate { get; }

        public VoucherCodeCreated(Guid id, string code, long value, Guid methodId, DateTime createdDate)
        {
            Id = id;
            Code = code;
            Value = value;
            MethodId = methodId;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }
}
