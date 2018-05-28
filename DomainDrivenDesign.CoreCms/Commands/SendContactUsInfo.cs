using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class SendContactUsInfo:ICommand
    {
        public Guid Id { get; }
        public string FromName { get; }
        public string FromEmail { get; }
        public string Title { get; }
        public string Body { get; }
        public string FromPhone { get;  }
        public Guid LanguageId { get; }

        public SendContactUsInfo(Guid id, string fromName, string fromEmail, string title, string body, string fromPhone,Guid languageId)
        {
            Id = id;
            FromName = fromName;
            FromEmail = fromEmail;
            Title = title;
            Body = body;
            FromPhone = fromPhone;
            LanguageId = languageId;
        }
    }
}
