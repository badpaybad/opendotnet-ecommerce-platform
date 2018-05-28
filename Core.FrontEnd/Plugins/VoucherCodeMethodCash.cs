using System;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class VoucherCodeMethodCash : IVoucherCodeMethod
    {
        public Guid Id { get { return Guid.Parse("ebf10913-c92f-4d0e-bc44-6ed711ee33a1"); } }

        public long CalculateValue(Guid shoppingCartId, long amount, VoucherCode code, Guid? userId)
        {
            return code.Value;
        }
    }
}