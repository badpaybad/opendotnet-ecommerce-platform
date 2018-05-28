using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class VoucherCodeMethodPercent : IVoucherCodeMethod
    {
        public Guid Id { get { return Guid.Parse("1b60efad-b15d-4eeb-b87e-3c6fe0330f8d"); } }

        public long CalculateValue(Guid shoppingCartId, long amount, VoucherCode code, Guid? userId)
        {
            long value = 0;
            value = (amount / 100) * code.Value;
            return value;
        }
    }

}