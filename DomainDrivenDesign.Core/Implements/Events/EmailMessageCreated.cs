using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class EmailMessageCreated:IEvent
    {
        public Guid Id { get; }
        public string FromEmail { get; }
        public string ToEmail { get; }
        public string Title { get; }
        public string Body { get; }
        public DateTime CreatedDate { get; }

        public EmailMessageCreated(Guid id, string fromEmail, string toEmail, string title, string body, DateTime createdDate, int status)
        {
            Id = id;
            FromEmail = fromEmail;
            ToEmail = toEmail;
            Title = title;
            Body = body;
            CreatedDate = createdDate;
            Status = status;
        }

        public long Version { get; set; }
        public int Status { get; set; }
    }

    public class EmailMessageSent:IEvent
    {
        public Guid Id { get; }
        public DateTime SentDate { get; }
        public long Version { get; set; }

        public EmailMessageSent(Guid id, DateTime sentDate)
        {
            Id = id;
            SentDate = sentDate;
        }
    }

    public class EmailMessageResent:IEvent
    {
        public DateTime ResendDate { get; }

        public EmailMessageResent(Guid id, int ttimes, DateTime resendDate)
        {
            ResendDate = resendDate;
        }

        public long Version { get; set; }
    }
}
