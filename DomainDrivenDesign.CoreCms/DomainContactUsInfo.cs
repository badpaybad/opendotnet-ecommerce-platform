using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.CoreCms
{
    public class DomainContactUsInfo
    {
        public DomainContactUsInfo(Guid id, string fromName, string fromEmail, string toEmail,
            string fromPhone, string toPhone,
            string title, string body, Guid languageId)
        {
            using (var db = new CoreDbContext())
            {
                db.ContactUsInfos.Add(new ContactUsInfo()
                {
                    Id = id,
                    LanguageId = languageId,
                    FromName = fromName,
                    Title = title,
                    CreatedDate = DateTime.Now,
                    Body = body,
                    ParentId = Guid.Empty,
                    Status = 0,
                    ToEmail = toEmail,
                    FromEmail = fromEmail,
                    ToPhone = toPhone,
                    FromPhone = fromPhone,
                    SentCounter = 0
                });
                db.SaveChanges();
            }
        }

        public DomainContactUsInfo()
        {
        }
    }
}
