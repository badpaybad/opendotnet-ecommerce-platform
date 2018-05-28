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

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class VocherMethodCommandHandles : ICommandHandle<UpdateVocherMethod>
    {
        IEventPublisher _eventPublisher = new EventPublisher();

        public void Handle(UpdateVocherMethod c)
        {

            _eventPublisher.Publish(new ContentLanguageUpdated(c.Id, c.LanguageId, "Description", c.Description, "VoucherMethod"));
        }
    }

    public class UpdateVocherMethod : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Description { get; }
        public Guid LanguageId { get; }

        public UpdateVocherMethod(Guid id, string description, Guid languageId,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Description = description;
            LanguageId = languageId;
        }
    }
}
