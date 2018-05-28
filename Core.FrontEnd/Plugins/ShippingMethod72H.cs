using System;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class ShippingMethod72H : IShippingMethod
    {
        public Guid Id { get { return Guid.Parse("ec15e832-f216-4506-9df0-2b5004fdade6"); } }

        public long CalculateCost(Guid shoppingCartId, ShippingMethod config, string address, double latitude, double longitude)
        {
            var distance = ShippingMethodServices.CalculateDistance(shoppingCartId, address, latitude, longitude);
            if (distance > 1)
            {
                return(long)(distance * config.UnitCost);
            }
            return config.UnitCost;
        }
    }
}