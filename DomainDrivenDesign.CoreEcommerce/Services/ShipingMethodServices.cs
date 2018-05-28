using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Reflection;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public interface IShippingMethod
    {
        Guid Id { get; }
        long CalculateCost(Guid shoppingCartId, ShippingMethod config, string address, double latitude, double longitude);
    }

    public class ShippingMethodServices
    {
        public static long ShippingCost(Guid shoppingCartId, Guid shippingMethodId, string address, double latitude, double longitude)
        {
            ShippingMethod methodInfo;
            using (var db = new CoreEcommerceDbContext())
            {
                methodInfo = db.ShippingMethods.SingleOrDefault(i => i.Id == shippingMethodId);
            }
            if (methodInfo == null || string.IsNullOrEmpty(methodInfo.AssemblyType)) return 0;

            var type = AssemblyExtesions.FindType(methodInfo.AssemblyType);
            if (type == null) throw new Exception("Can not load assembly " + methodInfo.AssemblyType);

            var method = Activator.CreateInstance(type) as IShippingMethod;

            var result = method.CalculateCost(shoppingCartId, methodInfo, address, latitude, longitude);

            return result;
        }

        public static double CalculateDistance(Guid shoppingCartId, string address, double latitude, double longitude)
        {
            List<Distance> distances = new List<Distance>();
            ShoppingCartShippingAddress cartAddress = new ShoppingCartShippingAddress();
            cartAddress.Address = address;
            cartAddress.AddressLatitude = latitude;
            cartAddress.AddressLongitude = longitude;

            using (var db = new CoreEcommerceDbContext())
            {
                distances = db.ShoppingCartItems.Where(i => i.ShoppingCartId == shoppingCartId)
                    .Join(db.RelationShips, ci => ci.ProductId, r => r.FromId, (ci, r) => new { Ci = ci, R = r })
                    .Join(db.Suppliers, m => m.R.ToId, s => s.Id, (m, s) => new { S = s })
                    .Select(i => new Distance()
                    {
                        SupplierLat = i.S.AddressLatitude,
                        SupplierLng = i.S.AddressLongitude
                    }).ToList();
            }

            foreach (var ds in distances)
            {
                ds.CartLat = cartAddress.AddressLatitude;
                ds.CartLng = cartAddress.AddressLongitude;
                ds.DistanceInKm =
                    GeoExtensions.GetDistanceFromLatLonInKm(ds.CartLat, ds.CartLng, ds.SupplierLat, ds.SupplierLng);
            }
            if (distances.Count == 0) return 0;
            return Math.Round(distances.Max(i => i.DistanceInKm), 2);
        }
    }

    public class Distance
    {
        public double SupplierLat;
        public double SupplierLng;
        public double CartLat;
        public double CartLng;
        public double DistanceInKm;
    }
}
