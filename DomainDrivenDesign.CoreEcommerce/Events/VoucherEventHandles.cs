using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
  //public   class VoucherEventHandles:IEventHandle<VoucherApplied>
  //  {
  //      public void Handle(VoucherApplied e)
  //      {
  //          using (var db =new CoreEcommerceDbContext())
  //          {
  //              var vc = db.VoucherCodes.SingleOrDefault(i => i.Code.Equals(e.VoucherCode));
  //              if (vc != null)
  //              {
  //                  vc.AppliedForUserId = e.UserId;
  //                  vc.AppliedForOrderCode = e.OrderCode;
  //                  vc.Applied = true;
  //              }
  //          }
  //      }
  //  }


  //  public class VoucherApplied : IEvent
  //  {
  //      public string VoucherCode { get; }
  //      public string OrderCode { get; }
  //      public Guid UserId { get; }

  //      public VoucherApplied(string voucherCode, string orderCode, Guid userId)
  //      {
  //          VoucherCode = voucherCode;
  //          OrderCode = orderCode;
  //          UserId = userId;
  //      }

  //      public long Version { get; set; }
  //  }
}
