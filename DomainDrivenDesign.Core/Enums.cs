using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core
{
    public class Enums
    {
        public enum UserMessageType
        {
            Email,
            Sms
        }

        public enum UserMessageTransactionStatus
        {
            Created,
            ReSend,
            Sent,
            Fail
        }

        public enum CategoryType
        {
            News = 0,
            Product = 1,
            NewsAndProduct = 2
        }

        public enum ShoppingCartStatus
        {
            ShoppingCart = 0,
            Checkedout = 1,
            OrderConfirmed = 2,
            OrderClosed = 3,
            OrderCanceled = 4,
        }

        public enum ShoppingCartPayStatus
        {
            PaymentCreated=0,
            PaymentProcess,
            PaymentSuccess,
            PaymentFail
        }

        public enum ShoppingCartPackingStatus
        {
            PackingCreated=0,
            PackingDone
        }

        public enum ShoppingCartShipStatus
        {
            ShippingCreated=0,
            ShippingSuccess,
            ShippingFail
        }
    }


}
