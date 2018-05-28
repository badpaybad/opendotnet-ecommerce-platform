using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Reflection;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public interface IVoucherCodeMethod
    {
        Guid Id { get; }
        long CalculateValue(Guid shoppingCartId, long amount, VoucherCode code, Guid? userId);
    }


    public class VoucherCodeServices
    {
        public static long CalculateValue(Guid shoppingCartId,string voucherCode, long amount, Guid? userId)
        {
            VoucherCode code;
            VoucherMethod method=null;
            using (var db=new CoreEcommerceDbContext())
            {
                code = db.VoucherCodes.SingleOrDefault(
                    i => i.Code.Equals(voucherCode, StringComparison.OrdinalIgnoreCase));
                if (code != null)
                {
                    method = db.VoucherMethods.SingleOrDefault(i => i.Id == code.VoucherMethodId);
                }
            }
            if (method == null || string.IsNullOrEmpty(method.AssemblyType)) return 0;

            var type = AssemblyExtesions.FindType(method.AssemblyType);

            if (type == null) throw new Exception("Can not load assembly " + method.AssemblyType);

            var vm = Activator.CreateInstance(type) as IVoucherCodeMethod;

            return vm.CalculateValue(shoppingCartId, amount, code, userId);
        }

        public static bool IsValidCode(string voucherCode, out VoucherCode code)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                code = db.VoucherCodes.SingleOrDefault(
                    i => i.Code.Equals(voucherCode, StringComparison.OrdinalIgnoreCase));
                if (code == null) return true;

                return !code.Applied;
            }
        }

        static Random _rnd=new Random();

        public static string GenerateCode()
        {
            var counter = 0;
            var now = DateTime.Now.ToString("yyMM");
            while (true)
            {
                if(counter>1000) throw new Exception("Limited can not generate more");

                var code = (now + StringExtensions.RandomString(5)).ToUpper();

                using (var db=new CoreEcommerceDbContext())
                {
                    var existed =
                        db.VoucherCodes.FirstOrDefault(i => i.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
                    if (existed == null)
                    {
                        return code;
                    }
                    else
                    {
                        counter++;
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}
