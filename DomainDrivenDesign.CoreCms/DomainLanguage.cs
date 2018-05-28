using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreCms
{
    public class DomainLanguage
    {
        public DomainLanguage(Guid id, string title, string code, string currencyCode,double currencyExchageRate)
        {
            using (var db = new CoreCmsDbContext())
            {
                var lexisted = db.Languages.SingleOrDefault(i => i.Code.Equals(code, StringComparison.OrdinalIgnoreCase)
                || i.Id == id
                || i.CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
                if (lexisted == null)
                {
                    db.Languages.Add(new Language()
                    {
                        Id = id,
                        Title = title,
                        Code = code,
                        CurrencyCode = currencyCode,
                        CurrencyExchangeRate=currencyExchageRate
                    });
                    db.SaveChanges();
                }
            }
        }

        public DomainLanguage()
        {
        }

        public void Update(Guid id, string title, string code, string currencyCode, double currencyExchageRate)
        {
            using (var db = new CoreCmsDbContext())
            {
                var lexisted = db.Languages.SingleOrDefault(i => i.Id == id);
                if (lexisted != null)
                {
                    lexisted.Code = code;
                    lexisted.Title = title;
                    lexisted.CurrencyCode = currencyCode;
                    lexisted.CurrencyExchangeRate = currencyExchageRate;

                    db.SaveChanges();
                }
            }
        }

        public void Delete(Guid id)
        {
            if (id == EngineeCurrentContext.DefaultLanguageId) throw new Exception("Can not delete Default language");

            using (var db = new CoreCmsDbContext())
            {
                var lexisted = db.Languages.SingleOrDefault(i => i.Id == id);
                if (lexisted != null)
                {
                    db.Languages.Remove(lexisted);

                    db.SaveChanges();
                }
            }
        }
    }
}
