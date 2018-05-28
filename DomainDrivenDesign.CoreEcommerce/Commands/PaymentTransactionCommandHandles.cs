using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
  public  class PaymentTransactionCommandHandles:ICommandHandle<CreatePaymentTransaction>
        ,ICommandHandle<SuccessPaymentTransaction>, ICommandHandle<FailPaymentTransaction>
        ,ICommandHandle<ProcessPaymentTransaction>
        ,ICommandHandle<AdminFailPaymentTransaction>
        ,ICommandHandle<AdminSuccessPaymentTransaction>
    {
        ICqrsEventSourcingRepository<DomainPaymentTransaction> 
            _repo=new CqrsEventSourcingRepository<DomainPaymentTransaction>(new EventPublisher());


        public void Handle(CreatePaymentTransaction c)
        {
            _repo.CreateNew(new DomainPaymentTransaction(c.Id,c.PaymentMethodId,c.OrderCode,c.Amount, c.IpAddress, c.LanguageId, c.SiteDomainUrl));
        }

        public void Handle(SuccessPaymentTransaction c)
        {
            _repo.GetDoSave(c.Id,o=>o.Success());
        }

        public void Handle(FailPaymentTransaction c)
        {
            _repo.GetDoSave(c.Id,o=>o.Fail());
        }

        public void Handle(ProcessPaymentTransaction c)
        {
            _repo.GetDoSave(c.Id, o => o.Process());
        }

        public void Handle(AdminFailPaymentTransaction c)
        {
            _repo.GetDoSave(c.Id, o => o.Fail());
        }

        public void Handle(AdminSuccessPaymentTransaction c)
        {
            _repo.GetDoSave(c.Id, o => o.Success());
        }
    }

}
