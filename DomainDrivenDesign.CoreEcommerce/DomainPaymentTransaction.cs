using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Reflection;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Events;
using DomainDrivenDesign.CoreEcommerce.Services;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class 
        DomainPaymentTransaction : AggregateRoot
    {
        public override string Id { get; set; }

        public DomainPaymentTransaction()
        {
            
        }

        void Apply(PaymentTransactionCreated e)
        {
            Id = e.Id.ToString();
        }

        public DomainPaymentTransaction(Guid id, Guid paymentMethodId, string orderCode, long amount
            , string ipAddress, Guid languageId,string siteDomainUrl)
        {
            DateTime createdDate=DateTime.Now;

            PaymentMethod pm;
            Language lang = null;
            using (var db = new CoreEcommerceDbContext())
            {
                pm = db.PaymentMethods.SingleOrDefault(i => i.Id == paymentMethodId);
                lang = db.Languages.SingleOrDefault(i => i.Id == languageId);
            }

            var type = AssemblyExtesions.FindType(pm.AssemblyType);
         
            if (type == null) throw new Exception("Can not load assembly " + pm.AssemblyType);

            var amountByCurrencyCode =(long)(amount * lang.CurrencyExchangeRate);

            var p = Activator.CreateInstance(type) as IPaymentMethod;

            var urlRedirect = p.GetRedirectUrl(pm,id, orderCode, amountByCurrencyCode, ipAddress, languageId, siteDomainUrl);

            ApplyChange(new PaymentTransactionCreated(id, orderCode, amountByCurrencyCode, paymentMethodId,createdDate, urlRedirect, ipAddress));
        }

        public void Process()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new PaymentTransactionProcessed(id));
        }

        public void Success()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new PaymentTransactionSuccessed(id));
        }

        public void Fail()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new PaymentTransactionFailed(id));
        }
    }

}
