using System;
using System.Collections.Generic;
using System.Linq;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Events;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class DomainShoppingCart : AggregateRoot
    {
        public DomainShoppingCart()
        {
        }

        public override string Id { get; set; }

        private Guid _userId;
        private Enums.ShoppingCartStatus _status;
        private List<ShoppingCartItem> _items;
        private long _cartTax;
        private long _cartDiscount;
        private long _cartSubTotal;
        private long _cartTotal;
        private long _shippingFee;
        private string _voucherCode;
        private long _voucherValue;
        private Guid _shippingMethodId;
        private bool _isPreCalculated;
        private Guid _languageId;
        private Guid _orderPromotionIdForDiscount;
        private Guid _orderPromotionIdForShipping;
        private string _ipAddress;
        private string _siteDomainUrl;
        private DateTime _receivingTime;
        private string _orderCode;


        void Apply(ShoppingCartCreated e)
        {
            Id = e.Id.ToString();
            _items = new List<ShoppingCartItem>();
            _languageId = e.LanguageId;
            _userId = e.UserId;
            _status = Enums.ShoppingCartStatus.ShoppingCart;
            _ipAddress = e.IpAddress;
            _siteDomainUrl = e.SiteDomainUrl;
        }

        void Apply(ShoppingCartAddedProduct e)
        {
            var temp = _items.SingleOrDefault(i => i.ProductId == e.ProductId);
            if (temp == null)
            {
                _items.Add(new ShoppingCartItem() { ProductId = e.ProductId, UnitPrice = e.UnitPrice, Quantity = e.Quantity, TotalPrice = e.TotalPrice });
            }
            else
            {
                temp.Quantity = e.Quantity;
                temp.TotalPrice = e.TotalPrice;
                temp.UnitPrice = e.UnitPrice;
            }
        }

        void Apply(ShoppingCartRemovedProduct e)
        {
            var temp = _items.SingleOrDefault(i => i.ProductId == e.ProductId);
            if (temp != null)
            {
                temp.Quantity = e.Quantity;
                temp.TotalPrice = e.TotalPrice;
                if (temp.Quantity <= 0)
                {
                    _items.RemoveAll(i => i.ProductId == e.ProductId);
                }
            }
        }

        void Apply(ShoppingCartRemovedAllProduct e)
        {
            _items.Clear();
        }

        void Apply(ShoppingCartPreCalculated e)
        {
            _cartTax = e.CartTax;
            _cartDiscount = e.CartDiscount;
            _cartSubTotal = e.CartSubTotal;
            _cartTotal = e.CartTotal;
            _isPreCalculated = true;
        }

        void Apply(ShoppingCartCalculatedShipping e)
        {
            _shippingFee = e.ShippingFee;
            _shippingMethodId = e.ShippingMethodId;
        }
        void Apply(ShoppingCartCalculatedVoucherCode e)
        {
            _voucherValue = e.VoucherValue;
            _voucherCode = e.VoucherCode;
        }
        void Apply(ShoppingCartCheckedout e)
        {
            _status = Enums.ShoppingCartStatus.Checkedout;
            _languageId = e.LanguageId;
            _cartTotal = e.CartTotal;
        }
        void Apply(ShoppingCartCreatedOrderCode e)
        {
            _orderCode = e.OrderCode;
        }

        void Apply(ShoppingCartClosedOrder e)
        {
            _status = Enums.ShoppingCartStatus.OrderClosed;
        }

        void Apply(ShoppingCartCanceledOrder e)
        {
            _status = Enums.ShoppingCartStatus.OrderCanceled;
        }

        void Apply(ShoppingCartPromotionCalulatedForOrderDiscount e)
        {
            _orderPromotionIdForDiscount = e.OrderPromotionId;
        }
        void Apply(ShoppingCartPromotionCalulatedForOrderShipping e)
        {
            _orderPromotionIdForShipping = e.OrderPromotionId;
        }
        void Apply(ShoppingCartPromotionAppliedToOrder e)
        {

        }
        void Apply(ShoppingCartReceivingTimeUpdated e)
        {
            _receivingTime = e.ReceivingTime;
        }

        public DomainShoppingCart(Guid id, Guid? userId, Guid languageId, string ipAddress, string siteDomainUrl)
        {
            userId = userId ?? Guid.Empty;

            ApplyChange(new ShoppingCartCreated(id, userId.Value, languageId, DateTime.Now, EngineeCurrentContext.SystemMinDate, ipAddress, siteDomainUrl));
        }

        public void AddProduct(Guid productId, int quantity)
        {
            if (_status != Enums.ShoppingCartStatus.ShoppingCart) throw new Exception("Shopping cart checkedout. Can not add product");
            if (quantity <= 0) throw new Exception("You must input Quantity of product");
            var id = Guid.Parse(Id);
            Product product;
            using (var db = new CoreEcommerceDbContext())
            {
                product = db.Products.SingleOrDefault(i => i.Id == productId);
            }
            if (product == null)
            {
                throw new Exception("Your selected product was not exist");
            }
            long total = 0;
            int tempQuantity = 0;
            var shoppingCartItems = _items.Where(i => i.ProductId == productId).ToList();
            if (shoppingCartItems.Count > 0)
            {
                total = shoppingCartItems.Sum(i => i.Quantity * i.UnitPrice);
                tempQuantity = shoppingCartItems.Sum(i => i.Quantity);
            }

            tempQuantity = tempQuantity + quantity;

            var pp = ProductPromotionServices.CalculateDiscount(productId, tempQuantity);
            long productDiscount = 0;
            Guid productPromotionId = Guid.Empty;

            if (pp != null)
            {
                productDiscount = pp.DiscountValue;
                productPromotionId = pp.Id;
            }
            else
            {
                productDiscount = 0;
            }

            total = total + (quantity * product.Price) - productDiscount;

            ApplyChange(new ShoppingCartAddedProduct(id, productId, tempQuantity, product.Price, total, DateTime.Now));
            ApplyChange(new ShoppingCartItemAppliedProductPromotion(id, productId, productPromotionId, quantity, productDiscount));

        }

        public void RemoveProduct(Guid productId, int quantity)
        {
            if (_status != Enums.ShoppingCartStatus.ShoppingCart) throw new Exception("Shopping cart checkedout. Can not remove product");
            var id = Guid.Parse(Id);
            Product product;
            using (var db = new CoreEcommerceDbContext())
            {
                product = db.Products.SingleOrDefault(i => i.Id == productId);
            }
            if (product == null)
            {
                throw new Exception("Your selected product was not exist");
            }
            long total = 0;
            int tempQuantity = 0;
            var shoppingCartItems = _items.Where(i => i.ProductId == productId).ToList();

            if (shoppingCartItems.Count > 0)
            {
                total = shoppingCartItems.Sum(i => i.Quantity * i.UnitPrice);
                tempQuantity = shoppingCartItems.Sum(i => i.Quantity);
            }

            tempQuantity = tempQuantity - quantity;

            var pp = ProductPromotionServices.CalculateDiscount(productId, tempQuantity);
            long productDiscount = 0;
            Guid productPromotionId = Guid.Empty;
            if (pp != null)
            {
                productDiscount = pp.DiscountValue;
                productPromotionId = pp.Id;
            }
            else
            {
                productDiscount = 0;
            }

            total = total - quantity * product.Price - productDiscount;

            ApplyChange(new ShoppingCartRemovedProduct(id, productId, tempQuantity, total, DateTime.Now));
            ApplyChange(new ShoppingCartItemAppliedProductPromotion(id, productId, productPromotionId, quantity, productDiscount));

        }

        public void RemoveAllProduct()
        {
            if (_status != Enums.ShoppingCartStatus.ShoppingCart) throw new Exception("Shopping cart checkedout. Can not remove product");
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartRemovedAllProduct(id, DateTime.Now));
        }

        public void PreCalculate()
        {
            if (_items == null || _items.Count == 0) throw new Exception("No item to pre-calculate");
            var id = Guid.Parse(Id);

            long cartTax = 0;
            long cartSubTotal = 0;
            if (_items != null && _items.Count >= 0)
            {
                cartSubTotal = _items.Sum(i => i.TotalPrice);
            }
            long cartDiscount = 0;
            var orderPromo = OrderPromotionServices.CalculateForDiscount(id);
            var orderPromotionId = Guid.Empty;
            if (orderPromo != null)
            {
                cartDiscount = orderPromo.DiscountAmount;
                orderPromotionId = orderPromo.Id;
            }

            ApplyChange(new ShoppingCartPromotionCalulatedForOrderDiscount(id, orderPromotionId));

            var cartTotal = cartSubTotal - cartTax - cartDiscount - _voucherValue - _shippingFee;

            ApplyChange(new ShoppingCartPreCalculated(id, DateTime.Now, cartTax, cartDiscount, cartSubTotal, cartTotal));
        }

        public void CalculateShipping(string address, double addressLatitude, double addressLongitude, Guid shippingMethodId)
        {
            if (_items == null || _items.Count == 0) throw new Exception("No item to calculate");

            if (string.IsNullOrEmpty(address) || shippingMethodId == Guid.Empty) throw new Exception("address and shipping method required");

            if (_isPreCalculated == false) PreCalculate();

            var id = Guid.Parse(Id);
            long shippingFee = ShippingMethodServices.ShippingCost(id, shippingMethodId, address, addressLatitude, addressLongitude);
            var orderPromotionId = Guid.Empty;
            var orderPromo = OrderPromotionServices.CalculateForShipping(id);
            if (orderPromo != null)
            {
                if (orderPromo.FreeShip)
                {
                    shippingFee = 0;
                }
                orderPromotionId = orderPromo.Id;
            }

            ApplyChange(new ShoppingCartPromotionCalulatedForOrderShipping(id, orderPromotionId));

            ApplyChange(new ShoppingCartCalculatedShipping(id, shippingMethodId, shippingFee));
        }

        public void CalculateVoucher(string voucherCode, Guid? userId)
        {
            if (_items == null || _items.Count == 0) throw new Exception("No item to calculate");

            if (string.IsNullOrEmpty(voucherCode)) throw new Exception("voucher code required");

            if (_isPreCalculated == false) PreCalculate();

            VoucherCode code;
            if (VoucherCodeServices.IsValidCode(voucherCode, out code) == false) return;
            if (code == null) return;

            var id = Guid.Parse(Id);
            userId = userId ?? Guid.Empty;

            var voucherValue = VoucherCodeServices.CalculateValue(id, voucherCode, _cartSubTotal, userId);
            ApplyChange(new ShoppingCartCalculatedVoucherCode(id, voucherCode, voucherValue, userId.Value));
        }

        public void Checkout(Guid? userId, string voucherCode, string addressName, string email, string phone
            , Guid shippingMethodId, DateTime receivingTime, string address, double addressLatitude, double addressLongitude
            , Guid paymentMethodId, string message, Guid languageId)
        {
            if (_items == null || _items.Count == 0) throw new Exception("No item to checkout");

            if (_status == Enums.ShoppingCartStatus.Checkedout) throw new Exception("Shopping cart already checkedout");
            if (_status == Enums.ShoppingCartStatus.OrderConfirmed) throw new Exception("Shopping cart confirmed");
            if (_status == Enums.ShoppingCartStatus.OrderClosed) throw new Exception("Shopping cart closed");
            if (_status == Enums.ShoppingCartStatus.OrderCanceled) throw new Exception("Shopping cart canceled");

            if (string.IsNullOrEmpty(address) || shippingMethodId == Guid.Empty) throw new Exception("address and shipping method required");
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone)) throw new Exception("email and phone required");
            if (paymentMethodId == Guid.Empty) throw new Exception("payment method required");

            if (_isPreCalculated == false) PreCalculate();

            var id = Guid.Parse(Id);
            userId = userId ?? Guid.Empty;

            CalculateShipping(address, addressLatitude, addressLongitude, shippingMethodId);

            if (!string.IsNullOrEmpty(voucherCode))
            {
                CalculateVoucher(voucherCode, userId);
            }

            var cartTotal = _cartSubTotal + _cartTax + _shippingFee - _voucherValue - _cartDiscount;

            var shoppingCartCheckedoutDate = DateTime.Now;

            ApplyChange(new ShoppingCartReceivingTimeUpdated(id, receivingTime));

            ApplyChange(new ShoppingCartShippingAddressCreated(id, userId.Value, phone, email
                , addressName, address, addressLatitude, addressLongitude, message, shippingMethodId));

            ApplyChange(new ShoppingCartCheckedout(id, userId.Value, _items
                , addressName, address, addressLatitude, addressLongitude
                , email, phone, message, _cartSubTotal, _shippingFee, _voucherValue, _cartDiscount, cartTotal, _cartTax
                , languageId, voucherCode, shoppingCartCheckedoutDate, _siteDomainUrl));

            var orderCode = OrderCodeServices.GenerateOrderCode(id, shoppingCartCheckedoutDate);
            _orderCode = orderCode;

            ApplyChange(new ShoppingCartPromotionAppliedToOrder(id, orderCode, _orderPromotionIdForDiscount, _orderPromotionIdForShipping, _cartDiscount, _shippingFee));

            ApplyChange(new ShoppingCartCreatedOrderCode(id, paymentMethodId, orderCode, cartTotal, _voucherCode
                , _voucherValue, userId.Value, _ipAddress, _languageId, _siteDomainUrl));
        }

        public void CustomerConfirm(Guid userId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartConfirmedByCustomer(id, _orderCode, userId, DateTime.Now));
        }

        public void AdminConfirm(Guid userId)
        {
            if (_isPreCalculated == false) PreCalculate();
        }

        public void UpdatePaymentStatus(Enums.ShoppingCartPayStatus payStatus)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartUpdatedPayStatus(id, _orderCode, (short)payStatus, _languageId, _siteDomainUrl));
        }

        public void UpdateShippingStatus(Enums.ShoppingCartShipStatus shipStatus)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartUpdatedShipStatus(id, (short)shipStatus));
        }

        public void CloseOrder()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartClosedOrder(id));
        }

        public void CancelOrder()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartCanceledOrder(id));
        }

        public void PrintLabel()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new ShoppingCartLabelPackedAndPrinted(id, _orderCode));
        }
    }

}