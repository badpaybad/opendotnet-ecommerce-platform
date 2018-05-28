using System.Linq;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class PaymentTransactionEventHandles : IEventHandle<PaymentTransactionCreated>
        , IEventHandle<PaymentTransactionSuccessed>, IEventHandle<PaymentTransactionFailed>
        , IEventHandle<PaymentTransactionProcessed>
    {
        public void Handle(PaymentTransactionCreated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                db.PaymentTransactions.Add(new PaymentTransaction()
                {
                    Id = e.Id,
                    PaymentMethodId = e.PaymentMethodId,
                    Amount = e.Amount,
                    CreatedDate = e.CreatedDate,
                    OrderCode = e.OrderCode,
                    UrlRedirect = e.UrlRedirect,
                    Status = (short)Enums.ShoppingCartPayStatus.PaymentCreated,
                    IpAddress = e.IpAddress
                });
                db.SaveChanges();
            }
        }

        public void Handle(PaymentTransactionProcessed e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.PaymentTransactions.SingleOrDefault(i => i.Id == e.Id);
                temp.Status = (short)Enums.ShoppingCartPayStatus.PaymentProcess;
                db.SaveChanges();
            }
        }

        public void Handle(PaymentTransactionSuccessed e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.PaymentTransactions.SingleOrDefault(i => i.Id == e.Id);
                temp.Status = (short)Enums.ShoppingCartPayStatus.PaymentSuccess;
                db.SaveChanges();
            }
        }

        public void Handle(PaymentTransactionFailed e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.PaymentTransactions.SingleOrDefault(i => i.Id == e.Id);
                temp.Status = (short)Enums.ShoppingCartPayStatus.PaymentFail;
                db.SaveChanges();
            }
        }

    }
}