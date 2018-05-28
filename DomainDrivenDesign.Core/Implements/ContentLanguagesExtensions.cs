using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements
{
   public static  class ContentLanguageExtensions
    {
        public static string GetValue(this IEnumerable<ContentLanguage> src, Guid srcId, Guid languageId,
            string columnName, string tableName = "")
        {
            var temp = src.Where(i => i.Id == srcId
                                      && i.LanguageId == languageId
                                      && i.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.ColumnValue).FirstOrDefault();
            if (string.IsNullOrEmpty(temp) == false) return temp;

            return src.Where(i => i.Id == srcId
                                  && i.LanguageId == EngineeCurrentContext.DefaultLanguageId
                                  && i.ColumnValue.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.ColumnValue).FirstOrDefault();
        }

        public static string GetValue(this IEnumerable<ContentLanguage> src, Guid srcId,
            string columnName, string tableName = "")
        {
            var temp = src.Where(i => i.Id == srcId
                                      && i.LanguageId == EngineeCurrentContext.LanguageId
                                      && i.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.ColumnValue).FirstOrDefault();
            if (string.IsNullOrEmpty(temp) == false) return temp;

            return src.Where(i => i.Id == srcId
                                  && i.LanguageId == EngineeCurrentContext.DefaultLanguageId
                                  && i.ColumnValue.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.ColumnValue).FirstOrDefault();
        }
    }
}
