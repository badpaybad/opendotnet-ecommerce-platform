using System;
using System.Linq;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class UrlFriendlyEventHandles : IEventHandle<UrlFriendlyCreated>
    {
        public void Handle(UrlFriendlyCreated e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = new UrlFriendly();
                temp.UfId = Guid.NewGuid();
                temp.Id = e.Id;
                temp.TableName = e.TableName;
                temp.ActionName = e.ActionName;
                temp.ControllerName = e.ControllerName;
                temp.UrlSegment = e.UrlSegment;
                db.UrlFriendlys.Add(temp);

                db.SaveChanges();
            }
        }
    }
}