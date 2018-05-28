using System;
using System.Linq;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class ContentLanguageEventHandles : IEventHandle<ContentLanguageUpdated>,IEventHandle<ContentLanguageDeleted>
    {
        public void Handle(ContentLanguageUpdated e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.ContentLanguages.FirstOrDefault(i => i.Id == e.Id
                                                                   && i.LanguageId == e.LanguageId && i.ColumnName.Equals(e.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (temp == null)
                {
                    temp = new ContentLanguage();
                    temp.Id = e.Id;
                    temp.LanguageId = e.LanguageId;
                    temp.ColumnName = e.ColumnName;
                    temp.ColumnValue = e.ColumnValue;
                    temp.TableName = e.TableName;
                    temp.CreatedDate = DateTime.Now;
                    db.ContentLanguages.Add(temp);
                }
                else
                {
                    temp.Id = e.Id;
                    temp.LanguageId = e.LanguageId;
                    temp.ColumnName = e.ColumnName;
                    temp.ColumnValue = e.ColumnValue;
                    temp.TableName = e.TableName;
                    temp.CreatedDate = DateTime.Now;
                }
                db.SaveChanges();
            }
        }


        public void Handle(ContentLanguageDeleted e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.ContentLanguages.Where(i => i.Id == e.Id);
                db.ContentLanguages.RemoveRange(temp);
                db.SaveChanges();
            }
        }
    }
}