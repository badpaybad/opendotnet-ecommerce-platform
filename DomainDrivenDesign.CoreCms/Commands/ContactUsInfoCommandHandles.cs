using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class ContactUsInfoCommandHandles : ICommandHandle<SendContactUsInfo> {
        public void Handle(SendContactUsInfo c)
        {
            var dqem = new DomainContactUsInfo(c.Id,c.FromName, c.FromEmail, EngineeCurrentContext.GetEmailForContactUs()
                , c.FromPhone, EngineeCurrentContext.GetPhoneForContactUs(), c.Title, c.Body, c.LanguageId);
        }
    }
}