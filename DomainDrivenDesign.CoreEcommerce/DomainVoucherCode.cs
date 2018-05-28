using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.CoreEcommerce.Events;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class DomainVoucherCode: AggregateRoot
    {
        private string _code;

        private bool _deleted;
        private bool _applied;

        public DomainVoucherCode()
        {
        }

        public override string Id { get; set; }

        void Apply(VoucherCodeCreated e)
        {
            Id = e.Id.ToString();
            _code = e.Code;
        }
        void Apply(VoucherCodeApplied e)
        {
            _applied = true;
        }
        void Apply(VoucherCodeDeleted e)
        {
            _deleted = true;
        }

        public DomainVoucherCode(Guid id,string code, long value, Guid methodId)
        {
            ApplyChange(new VoucherCodeCreated(id,code,value,methodId, DateTime.Now));
        }

        public void Apply(string code, string orderCode, Guid userId, long valueApply)
        {
            if(_deleted) throw new Exception("Already deleted");
            if(_applied) throw new Exception("Already applied");
            var id = Guid.Parse(Id);
            ApplyChange(new VoucherCodeApplied(id, code, orderCode, userId,valueApply));
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new VoucherCodeDeleted(id));
        }
    }

}
