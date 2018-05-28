using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public class OrderCodeServices
    {
        public  static string GenerateOrderCode(Guid shoppingCartId, DateTime shoppingCartCheckedoutDate)
        {
            while (true)
            {
                var random = StringExtensions.RandomString(5);
                var code = (shoppingCartCheckedoutDate.ToString("yyMM") + random).ToUpper();
                using (var db=new CoreEcommerceDbContext())
                {
                    var temp = db.ShoppingCarts.FirstOrDefault(
                        i => i.OrderCode.Equals(code, StringComparison.OrdinalIgnoreCase));
                    if (temp == null) return code;
                }
                Thread.Sleep(1);
            }
        }
    }
}
