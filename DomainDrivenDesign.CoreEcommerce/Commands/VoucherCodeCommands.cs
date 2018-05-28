using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{

    public class DeleteVoucherCode : AdminBaseCommand
    {
        public DeleteVoucherCode(Guid id, Guid userId, DateTime createdDate):base(userId,createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class ApplyVoucherCode : ICommand
    {
        public ApplyVoucherCode(Guid id, string code, string orderCode, Guid userId, long valueApply)
        {
            Id = id;
            Code = code;
            OrderCode = orderCode;
            UserId = userId;
            ValueApply = valueApply;
        }

        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string OrderCode { get; private set; }
        public Guid UserId { get; private set; }
        public long ValueApply { get; private set; }
    }

    public class CreateVoucherCode : AdminBaseCommand
    {
        public CreateVoucherCode(Guid id, string code, long value, Guid voucherMethodId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Code = code;
            Value = value;
            VoucherMethodId = voucherMethodId;
        }

        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public long Value { get; private set; }
        public Guid VoucherMethodId { get; private set; }
    }
}
