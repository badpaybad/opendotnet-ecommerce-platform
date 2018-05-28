using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class ContentLanguageUpdated : IEvent
    {
        public ContentLanguageUpdated(Guid id, Guid languageId, string columnName, string columnValue, string tableName)
        {
            Id = id;
            LanguageId = languageId;
            ColumnName = columnName;
            ColumnValue = columnValue;
            TableName = tableName;
        }

        public long Version { get; set; }
        public Guid Id { get;  }
        public Guid LanguageId { get;  }
        public string ColumnName { get; }
        public string ColumnValue { get; }
        public string TableName { get; }
    }

    public class ContentLanguageDeleted : IEvent
    {
        public Guid Id { get; }
        public long Version { get; set; }
        public ContentLanguageDeleted(Guid id)
        {
            Id = id;
        }
    }
}
