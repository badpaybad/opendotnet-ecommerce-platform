using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Utils
{
    public static class EnumsExtensions
    {
        public static Dictionary<short, string> ListCategoryType() 
        {
            return CacheManager.GetOrSetIfNull("Enums.CategoryType", () =>
             {
                 return Enum.GetValues(typeof(Enums.CategoryType)).Cast<Enums.CategoryType>()
                     .ToDictionary(k => (short)k, v => v.ToString());
             });
        }
        public static Dictionary<short, string> ListOrderStatus()
        {
            return CacheManager.GetOrSetIfNull("Enums.ShoppingCartStatus", () =>
            {
                return Enum.GetValues(typeof(Enums.ShoppingCartStatus)).Cast<Enums.ShoppingCartStatus>()
                    .ToDictionary(k => (short)k, v => v.ToString());
            });
        }
        public static Dictionary<short, string> ListPayStatus()
        {
            return CacheManager.GetOrSetIfNull("Enums.ShoppingCartPayStatus", () =>
            {
                return Enum.GetValues(typeof(Enums.ShoppingCartPayStatus)).Cast<Enums.ShoppingCartPayStatus>()
                    .ToDictionary(k => (short)k, v => v.ToString());
            });
        }
        public static Dictionary<short, string> ListShipStatus()
        {
            return CacheManager.GetOrSetIfNull("Enums.ShoppingCartShipStatus", () =>
            {
                return Enum.GetValues(typeof(Enums.ShoppingCartShipStatus)).Cast<Enums.ShoppingCartShipStatus>()
                    .ToDictionary(k => (short)k, v => v.ToString());
            });
        }
    }
}
