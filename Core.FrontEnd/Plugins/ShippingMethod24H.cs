using System;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class ShippingMethod24H : IShippingMethod
    {
        public Guid Id { get { return Guid.Parse("d8d20f78-6095-4269-b6f9-aa5c05c4be74"); } }

        public long CalculateCost(Guid shoppingCartId, ShippingMethod config, string address, double latitude, double longitude)
        {
            var distance = ShippingMethodServices.CalculateDistance(shoppingCartId, address,latitude,longitude);
            if (distance > 1)
            {
                return (long)(distance * config.UnitCost);
            }
            return config.UnitCost;
        }
    }
}