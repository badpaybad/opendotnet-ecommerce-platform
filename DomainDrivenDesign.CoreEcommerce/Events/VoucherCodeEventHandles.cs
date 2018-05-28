using System;
using System.Linq;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class VoucherCodeEventHandles:IEventHandle<VoucherCodeCreated>
        ,IEventHandle<VoucherCodeApplied>, IEventHandle<VoucherCodeDeleted>{

        public void Handle(VoucherCodeCreated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var vc = db.VoucherCodes.SingleOrDefault(i => i.Code.Equals(e.Code));
                if (vc == null)
                {
                    db.VoucherCodes.Add(new VoucherCode()
                    {
                        Id = e.Id,
                        Value = e.Value,
                        CreatedDate = e.CreatedDate,
                        Code = e.Code,
                        AppliedForUserId = Guid.Empty,
                        VoucherMethodId = e.MethodId
                    });
                    db.SaveChanges();
                }
            }
        }

        public void Handle(VoucherCodeApplied e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var vc = db.VoucherCodes.SingleOrDefault(i => i.Code.Equals(e.Code));
                if (vc != null)
                {
                    vc.AppliedForUserId = e.UserId;
                    vc.AppliedForOrderCode = e.OrderCode;
                    vc.Applied = true;
                    db.SaveChanges();
                }
                
            }
        }

        public void Handle(VoucherCodeDeleted e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var vc = db.VoucherCodes.SingleOrDefault(i => i.Id==e.Id);
                if (vc != null)
                {
                    db.VoucherCodes.Remove(vc);
                    db.SaveChanges();
                }

            }
        }
    }
}