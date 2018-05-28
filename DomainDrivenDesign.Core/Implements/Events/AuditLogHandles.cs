using System;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class AuditLogHandles : IEventHandle<AuditLogCreated>
    {
        public void Handle(AuditLogCreated e)
        {
            using (var db = new CoreDbContext())
            {
                db.AuditLogs.Add(new AuditLog()
                {
                    CreatedDate = e.CreatedDate,
                    AlId = Guid.NewGuid(),
                    CommandData = e.CommandData,
                    CommandType = e.CommandType,
                    UserId = e.UserId
                });
                db.SaveChanges();
            }
        }
    }
}