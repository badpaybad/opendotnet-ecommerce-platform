using System;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class EmailMessageEventHandles:IEventHandle<EmailMessageCreated>, IEventHandle<EmailMessageResent>
        ,IEventHandle<EmailMessageSent>
    {
        public void Handle(EmailMessageCreated e)
        {
            using (var db = new FidgaDbContext())
            {
                db.QueueEmailMessages.Add(new QueueEmailMessage()
                {
                    Id=e.Id,
                    Title = e.Title,
                    ParentId = Guid.Empty,
                    CreatedDate=e.CreatedDate,
                    Body=e.Body,
                    FromEmail=e.FromEmail,
                    ToEmail = e.ToEmail,
                    SentCounter = 0,
                    Status = e.Status
                });
            }
        }

        public void Handle(EmailMessageResent e)
        {
            
        }

        public void Handle(EmailMessageSent e)
        {
           
        }
    }
}