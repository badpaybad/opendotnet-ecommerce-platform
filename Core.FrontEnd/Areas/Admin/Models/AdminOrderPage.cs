using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.Core;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class AdminOrderPage
    {
        public short? Status;

        public class OrderItem
        {
            public Guid Id;
            public string OrderCode;
            public DateTime CreatedDate;

            public short OrderStatusId;
            public Enums.ShoppingCartStatus OrderStatus
            {
                get { return (Enums.ShoppingCartStatus)OrderStatusId; }
            }
            public string OrderStatusText { get { return OrderStatus.ToString(); } }

            public short PayStatusId;
            public Enums.ShoppingCartPayStatus PayStatus
            {
                get { return (Enums.ShoppingCartPayStatus)PayStatusId; }
            }
            public string PayStatusText { get { return PayStatus.ToString(); } }


            public short PackingStatusId;
            public Enums.ShoppingCartPackingStatus PackingStatus
            {
                get { return (Enums.ShoppingCartPackingStatus)PackingStatusId; }
            }
            public string PackingStatusText { get { return PackingStatus.ToString(); } }

            public short ShipStatusId;
            public Enums.ShoppingCartShipStatus ShipStatus
            {
                get { return (Enums.ShoppingCartShipStatus)ShipStatusId; }
            }
            public string ShipStatusText { get { return ShipStatus.ToString(); } }

            public long CartTotal { get; set; }
            public long ShippingFee { get; set; }
            public long CartDiscount { get; set; }
            public long CartSubTotal { get; set; }
            public Guid ShippingMethodId { get; set; }
            public string ShippingMethodName { get; set; }
            public Guid PaymentMethodId { get; set; }
            public string PaymentMethodName { get; set; }
            public long VoucherValue { get; set; }
            public string VoucherCode { get; set; }
            public DateTime ReceivingTime { get; set; }

            public string CustomerAddress;
            public string CustomerName;
            public string CustomerEmail;
            public string CustomerPhone;
            public double AddressLatitude;
            public double AddressLongitude;
            public string Note;

        }
    }
}