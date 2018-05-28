using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class PaymentMethodCommandHandles:ICommandHandle<UpdatePaymentMethod>
    {
        IEventPublisher _eventPublisher=new EventPublisher();

        public void Handle(UpdatePaymentMethod c)
        {
           
            _eventPublisher.Publish(new ContentLanguageUpdated(c.Id, c.LanguageId, "Description", c.Description, "PaymentMethod"));

        }
    }

    public class UpdatePaymentMethod : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Description { get; }
        public Guid LanguageId { get; }

        public UpdatePaymentMethod(Guid id, string description, Guid languageId,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Description = description;
            LanguageId = languageId;
        }
    }
}
