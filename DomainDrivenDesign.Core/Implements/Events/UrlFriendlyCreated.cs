using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class UrlFriendlyCreated:IEvent
    {
        public string UrlSegment { get; }
        public string TableName { get; }
        public Guid Id { get; }
        public string ControllerName { get; }
        public string ActionName { get; }

        public UrlFriendlyCreated(string urlSegment, string tableName, Guid id, string controllerName,
            string actionName)
        {
            UrlSegment = urlSegment;
            TableName = tableName;
            Id = id;
            ControllerName = controllerName;
            ActionName = actionName;
        }

        public long Version { get; set; }
    }
}
