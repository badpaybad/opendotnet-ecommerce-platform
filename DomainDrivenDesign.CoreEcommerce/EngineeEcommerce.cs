using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace DomainDrivenDesign.CoreEcommerce
{
    public static class EngineeEcommerce
    {
        public static void Init()
        {
            var allAss = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allAss)
            {
                RegisterPaymentMethodPlugin(assembly);
                RegisterShippingMethodPlugin(assembly);
                RegisterVouchẻMethodPlugin(assembly);
            }
        }


        public static void RegisterShippingMethodPlugin(Assembly executingAssembly)
        {
            var allTypes = executingAssembly.GetTypes();
            var list = allTypes.Where(t => typeof(IShippingMethod).IsAssignableFrom(t)
                                           && t.IsClass && !t.IsAbstract).ToList();

            foreach (var pmType in list)
            {
                var tempPm = Activator.CreateInstance(pmType) as IShippingMethod;
                if (tempPm == null) continue;

                using (var db = new CoreEcommerce.Ef.CoreEcommerceDbContext())
                {
                    var existed = db.ShippingMethods.SingleOrDefault(i => i.Id == tempPm.Id);
                    if (existed != null) continue;

                    var type = tempPm.GetType();

                    db.ShippingMethods.Add(new ShippingMethod()
                    {
                        AssemblyType = type.FullName,
                        Id = tempPm.Id,
                        Name = type.Name
                    });
                    db.SaveChanges();
                }
            }
        }

        public static void RegisterPaymentMethodPlugin(Assembly executingAssembly)
        {
            var allTypes = executingAssembly.GetTypes();
            var list = allTypes.Where(t => typeof(IPaymentMethod).IsAssignableFrom(t)
                                                  && t.IsClass && !t.IsAbstract).ToList();

            foreach (var pmType in list)
            {
                var tempPm = Activator.CreateInstance(pmType) as IPaymentMethod;
                if (tempPm == null) continue;

                using (var db=new CoreEcommerce.Ef.CoreEcommerceDbContext())
                {
                    var existed = db.PaymentMethods.SingleOrDefault(i => i.Id == tempPm.Id);
                    if (existed != null) continue;

                    var type = tempPm.GetType();
                    db.PaymentMethods.Add(new PaymentMethod()
                    {
                        AssemblyType= type.FullName,
                        Id= tempPm.Id,
                        Name= type.Name
                    });
                    db.SaveChanges();
                }
            }
        }

        public static void RegisterVouchẻMethodPlugin(Assembly executingAssembly)
        {
            var allTypes = executingAssembly.GetTypes();
            var list = allTypes.Where(t => typeof(IVoucherCodeMethod).IsAssignableFrom(t)
                                                  && t.IsClass && !t.IsAbstract).ToList();

            foreach (var pmType in list)
            {
                var tempPm = Activator.CreateInstance(pmType) as IVoucherCodeMethod;
                if (tempPm == null) continue;

                using (var db=new CoreEcommerce.Ef.CoreEcommerceDbContext())
                {
                    var existed = db.VoucherMethods.SingleOrDefault(i => i.Id == tempPm.Id);
                    if (existed != null) continue;

                    var type = tempPm.GetType();
                    db.VoucherMethods.Add(new VoucherMethod()
                    {
                        AssemblyType= type.FullName,
                        Id= tempPm.Id,
                        Name= type.Name
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}
