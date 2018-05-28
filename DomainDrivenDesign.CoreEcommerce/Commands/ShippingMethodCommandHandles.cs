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
    public class ShippingMethodCommandHandles : ICommandHandle<UpdateShippingMethod>
    {
        IEventPublisher _eventPublisher = new EventPublisher();

        public void Handle(UpdateShippingMethod c)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShippingMethods.SingleOrDefault(i => i.Id == c.Id);
                if (temp != null)
                {
                    temp.UnitCost = c.UnitCost;
                    db.SaveChanges();
                }
            }

            _eventPublisher.Publish(new ContentLanguageUpdated(c.Id, c.LanguageId, "Description", c.Description, "ShippingMethod"));

        }
    }

    public class UpdateShippingMethod : AdminBaseCommand
    {
        public Guid Id { get; }
        public long UnitCost { get; }
        public string Description { get; }
        public Guid LanguageId { get; }

        public UpdateShippingMethod(Guid id, long unitCost, string description, Guid languageId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            UnitCost = unitCost;
            Description = description;
            LanguageId = languageId;
        }
    }
}
