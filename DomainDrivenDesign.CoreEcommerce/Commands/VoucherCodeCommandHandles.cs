using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
   public class VoucherCodeCommandHandles: ICommandHandle<CreateVoucherCode>,
        ICommandHandle<ApplyVoucherCode>, ICommandHandle<DeleteVoucherCode>
   {
        ICqrsEventSourcingRepository<DomainVoucherCode> _repo=new CqrsEventSourcingRepository<DomainVoucherCode>(new EventPublisher());
       public void Handle(CreateVoucherCode c)
       {
           _repo.CreateNew(new DomainVoucherCode(c.Id,c.Code,c.Value,c.VoucherMethodId));
       }

       public void Handle(ApplyVoucherCode c)
       {
           _repo.GetDoSave(c.Id,o=>o.Apply(c.Code,c.OrderCode,c.UserId,c.ValueApply));
       }

       public void Handle(DeleteVoucherCode c)
       {
           _repo.GetDoSave(c.Id,o=>o.Delete());
       }
   }

}
