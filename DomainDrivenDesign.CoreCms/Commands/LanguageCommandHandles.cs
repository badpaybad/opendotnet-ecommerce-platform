using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class LanguageCommandHandles : ICommandHandle<CreateLanguage>, ICommandHandle<UpdateLanguage>
        , ICommandHandle<DeleteLaguage>
    {
        public void Handle(CreateLanguage c)
        {
            new DomainLanguage(c.Id, c.Title, c.Code, c.CurrencyCode, c.CurrencyExchangeRate);
        }

        public void Handle(UpdateLanguage c)
        {
            new DomainLanguage().Update(c.Id, c.Title, c.Code, c.CurrencyCode,c.CurrencyExchangeRate);
        }

        public void Handle(DeleteLaguage c)
        {
            new DomainLanguage().Delete(c.Id);
        }
    }

    public class CreateLanguage : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Code { get; }
        public string CurrencyCode { get; }
        public double CurrencyExchangeRate { get; }

        public CreateLanguage(Guid id, string title, string code, string currencyCode, double currencyExchangeRate,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            Code = code;
            CurrencyCode = currencyCode;
            CurrencyExchangeRate = currencyExchangeRate;
        }
    }

    public class UpdateLanguage : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Code { get; }
        public string CurrencyCode { get; }
        public double CurrencyExchangeRate { get; }

        public UpdateLanguage(Guid id, string title, string code, string currencyCode, double currencyExchangeRate,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            Code = code;
            CurrencyCode = currencyCode;
            CurrencyExchangeRate = currencyExchangeRate;
        }
    }

    public class DeleteLaguage : AdminBaseCommand
    {
        public Guid Id { get; private set; }

        public DeleteLaguage(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }
}
