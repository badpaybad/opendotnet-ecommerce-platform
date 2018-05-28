using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ShoppingCartEventHandles : IEventHandle<ShoppingCartCreated>, IEventHandle<ShoppingCartAddedProduct>
         , IEventHandle<ShoppingCartRemovedProduct>, IEventHandle<ShoppingCartRemovedAllProduct>
         , IEventHandle<ShoppingCartPreCalculated>, IEventHandle<ShoppingCartCheckedout>,
         IEventHandle<ShoppingCartCalculatedShipping>
         , IEventHandle<ShoppingCartCalculatedVoucherCode>
         , IEventHandle<ShoppingCartCreatedOrderCode>
         , IEventHandle<ShoppingCartShippingAddressCreated>
         , IEventHandle<ShoppingCartUpdatedPayStatus>
         , IEventHandle<ShoppingCartUpdatedShipStatus>
        , IEventHandle<ShoppingCartConfirmedByCustomer>
        , IEventHandle<ShoppingCartCanceledOrder>
        , IEventHandle<ShoppingCartClosedOrder>
        , IEventHandle<ShoppingCartItemAppliedProductPromotion>
        , IEventHandle<ShoppingCartPromotionCalulatedForOrderDiscount>
        , IEventHandle<ShoppingCartPromotionCalulatedForOrderShipping>
        , IEventHandle<ShoppingCartPromotionAppliedToOrder>
        ,IEventHandle<ShoppingCartReceivingTimeUpdated>
        ,IEventHandle<ShoppingCartLabelPackedAndPrinted>
    {
        public void Handle(ShoppingCartCreated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.FirstOrDefault(i => i.Id == e.Id);
                if (temp != null) return;
                db.ShoppingCarts.Add(new ShoppingCart()
                {
                    Id = e.Id,
                    CreatedDate = e.CreatedDate,
                    ReceivingTime=e.ReceivingTime,
                    UserId = e.UserId,
                    Status = (short)Enums.ShoppingCartStatus.ShoppingCart,
                });
                db.SaveChanges();
            }
        }

        public void Handle(ShoppingCartAddedProduct e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCartItems.SingleOrDefault(i => i.ProductId == e.ProductId && i.ShoppingCartId == e.Id);
                if (temp != null)
                {
                    temp.Quantity = e.Quantity;
                    temp.UnitPrice = e.UnitPrice;
                    temp.TotalPrice = e.TotalPrice;
                }
                else
                {
                    db.ShoppingCartItems.Add(new ShoppingCartItem()
                    {
                        ShoppingCartId = e.Id,
                        CreatedDate = e.CreatedDate,
                        Quantity = e.Quantity,
                        ProductId = e.ProductId,
                        UnitPrice = e.UnitPrice,
                        TotalPrice = e.TotalPrice,
                    });
                }
                db.SaveChanges();
            }
        }

        public void Handle(ShoppingCartRemovedProduct e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCartItems.SingleOrDefault(i => i.ProductId == e.ProductId && i.ShoppingCartId == e.Id);
                if (temp != null)
                {
                    temp.Quantity = e.Quantity;
                    temp.TotalPrice = e.TotalPrice;
                    if (temp.Quantity <= 0)
                    {
                        db.ShoppingCartItems.Remove(temp);
                    }
                    db.SaveChanges();
                }

            }
        }

        public void Handle(ShoppingCartRemovedAllProduct e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCartItems.Where(i => i.ShoppingCartId == e.Id);
                db.ShoppingCartItems.RemoveRange(temp);
                db.SaveChanges();
            }
        }

        public void Handle(ShoppingCartPreCalculated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.CartTotal = e.CartTotal;
                    temp.CartDiscount = e.CartDiscount;
                    temp.CartSubTotal = e.CartSubTotal;
                    temp.CartTax = e.CartTax;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartCheckedout e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Status = (short)Enums.ShoppingCartStatus.Checkedout;
                    temp.VoucherValue = e.VoucherValue;
                    temp.VoucherCode = e.VoucherCode;
                    temp.CartTotal = e.CartTotal;
                    temp.CartDiscount = e.CartDiscount;
                    temp.CartSubTotal = e.CartSubTotal;
                    temp.CartTax = e.CartTax;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartCalculatedShipping e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.ShippingFee = e.ShippingFee;
                    temp.ShippingMethodId = e.ShippingMethodId;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartCalculatedVoucherCode e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.VoucherValue = e.VoucherValue;
                    temp.VoucherCode = e.VoucherCode;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartCreatedOrderCode e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.OrderCode = e.OrderCode;
                    temp.PaymentMethodId = e.PaymentMethodId;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartShippingAddressCreated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var tempA = db.ShoppingCartShippingAddresses.FirstOrDefault(i => i.ShoppingCartId == e.Id);
                if (tempA == null)
                {
                    var entity = new ShoppingCartShippingAddress();
                    entity.Id = Guid.NewGuid();
                    entity.ShoppingCartId = e.Id;
                    entity.AddressName = e.AddressName;
                    entity.Address = e.Address;
                    entity.AddressLatitude = e.AddressLatitude;
                    entity.AddressLongitude = e.AddressLongitude;
                    entity.Phone = e.Phone;
                    entity.Email = e.Email;
                    entity.Message = e.Message;
                    entity.CreatedDate = DateTime.Now;
                    db.ShoppingCartShippingAddresses.Add(entity);
                }
                else
                {
                    tempA.AddressName = e.AddressName;
                    tempA.Address = e.Address;
                    tempA.AddressLatitude = e.AddressLatitude;
                    tempA.AddressLongitude = e.AddressLongitude;
                    tempA.Phone = e.Phone;
                    tempA.Email = e.Email;
                    tempA.Message = e.Message;
                    tempA.CreatedDate = DateTime.Now;
                }

                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.ShippingMethodId = e.ShippingMethodId;

                }
                db.SaveChanges();
            }
        }

        public void Handle(ShoppingCartUpdatedPayStatus e)
        {
            using (var db = new CoreEcommerceDbContext())
            {

                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.PaymentStatus = e.PayStatus;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartUpdatedShipStatus e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.ShippingStatus = e.ShipStatus;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartConfirmedByCustomer e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Status = (short)Enums.ShoppingCartStatus.OrderConfirmed;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartCanceledOrder e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Status = (short)Enums.ShoppingCartStatus.OrderCanceled;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartClosedOrder e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.Status = (short)Enums.ShoppingCartStatus.OrderClosed;
                    db.SaveChanges();
                }

            }
        }

        public void Handle(ShoppingCartItemAppliedProductPromotion e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCartItems
                    .SingleOrDefault(i => i.ProductId == e.ProductId && i.ShoppingCartId == e.Id);
                if (temp != null)
                {
                    temp.ProductDiscount = e.ProductDiscount;
                    temp.ProductPromotionId = e.ProductPromotionId;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartPromotionCalulatedForOrderDiscount e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.ShoppingCartId);
                if (temp != null)
                {
                    temp.OrderPromotionId = e.OrderPromotionId;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartPromotionCalulatedForOrderShipping e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.ShoppingCartId);
                if (temp != null)
                {
                    temp.OrderPromotionId = e.OrderPromotionId;
                    db.SaveChanges();
                }

            }
        }

        public void Handle(ShoppingCartPromotionAppliedToOrder e)
        {
           
        }

        public void Handle(ShoppingCartReceivingTimeUpdated e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.ReceivingTime = e.ReceivingTime;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(ShoppingCartLabelPackedAndPrinted e)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);
                if (temp != null)
                {
                    temp.PackingStatus = (short)Enums.ShoppingCartPackingStatus.PackingDone;
                    db.SaveChanges();
                }
            }
        }
    }


    public class ShoppingCartLabelPackedAndPrinted : IEvent
    {
        public Guid Id { get; }
        public string OrderCode { get; }

        public ShoppingCartLabelPackedAndPrinted(Guid id, string orderCode)
        {
            Id = id;
            OrderCode = orderCode;
        }

        public long Version { get; set; }
    }
}
