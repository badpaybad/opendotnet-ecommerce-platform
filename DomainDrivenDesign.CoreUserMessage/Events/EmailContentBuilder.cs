using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Events;
using DomainDrivenDesign.CorePermission.Events;

namespace DomainDrivenDesign.CoreUserMessage.Events
{
    public class OrderItem
    {
        public Guid Id;
        public string Title;
        public string Promotion;
        public Guid PromotionId;
        public int Quanity;
        public long Price;
    }
    public class EmailContentBuilder : IEventHandle<UserRegistered>
         , IEventHandle<ShoppingCartUpdatedPayStatus>,IEventHandle<UserCreatedResetPasswordConfirmCode>
        ,IEventHandle<UserResetPasswordDone>, IEventHandle<UserActived>
    {
        public void Handle(UserRegistered e)
        {
            var subject = "Your active code";
            var content = "This email to guide to active account: " + e.Username;
            content += "Here is your active code: " + e.ActiveCode;
            var siteUrl = e.WebsiteUrl.Trim(new[] { ' ', '/' });

            content += $"<br><a href='{siteUrl}/User/ActiveUserAccount/?id={e.Id}&code={e.ActiveCode}'>Click this link to active account</a>";

            MemoryMessageBuss.PushCommand(new SendEmailToUser(Guid.NewGuid(), e.Email, e.Username, subject, content, DateTime.Now, e.Id, DateTime.Now));
        }

        public void Handle(ShoppingCartUpdatedPayStatus e)
        {
            ShoppingCartShippingAddress addr;
            ShoppingCart order;
            List<OrderItem> items;
            List<ContentLanguage> contentLanguages;
            Language lang;
            using (var db = new CoreEcommerceDbContext())
            {
                addr = db.ShoppingCartShippingAddresses.OrderByDescending(i => i.CreatedDate)
                    .FirstOrDefault(i => i.ShoppingCartId == e.Id);
                order = db.ShoppingCarts.SingleOrDefault(i => i.Id == e.Id);

                lang = db.Languages.SingleOrDefault(i => i.Id == e.LanguageId);

                items = db.ShoppingCartItems.Where(i => i.ShoppingCartId == e.Id)
                    .Select(i => new OrderItem()
                    {
                        Id = i.ProductId,
                        PromotionId = i.ProductPromotionId,
                        Price = i.TotalPrice,
                        Quanity = i.Quantity
                    }).ToList();
                var ids = items.Select(i => i.Id).ToList();
                ids.AddRange(items.Select(i=>i.PromotionId).ToList());
                contentLanguages = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }
            var orderUrl = e.WebsiteUrl.Trim(new[] {'/'}) + "/Order/Detail/?orderCode="+e.OrderCode;
            string orderInfo = "<br>Your order code: " + e.OrderCode +$" <a href='{orderUrl}'>Click here to check status</a>";
            orderInfo += "<table>";
            orderInfo += "<tr><td>Product</td><td>Quantity</td><td>Price</td><tr>";
            foreach (var itm in items)
            {
                itm.Title = contentLanguages.GetValue(itm.Id, e.LanguageId, "Title");
                itm.Promotion = contentLanguages.GetValue(itm.PromotionId, e.LanguageId, "Description");
                orderInfo += "<tr>";
                orderInfo += $"<td>{itm.Title}<br>{itm.Promotion}</td><td>{itm.Quanity}</td><td>{itm.Price}</td>";
                orderInfo += "</tr>";
            }
            orderInfo += "<tr>";
            var subtotal = (order.CartSubTotal * lang.CurrencyExchangeRate) + lang.CurrencyCode;
            orderInfo += $"<td>Subtotal:</td><td></td><td>{subtotal}</td>";
            orderInfo += "</tr><tr>";
            var cartDiscount = (order.CartDiscount * lang.CurrencyExchangeRate) + lang.CurrencyCode;
            orderInfo += $"<td>Discount:</td><td></td><td>-{cartDiscount}</td>";
            orderInfo += "</tr><tr>";
            var voucherValue = (order.VoucherValue * lang.CurrencyExchangeRate) + lang.CurrencyCode;
            orderInfo += $"<td>Voucher:</td><td>{order.VoucherCode}</td><td>-{voucherValue}</td>";
            orderInfo += "</tr><tr>";
            var shippingFee = (order.ShippingFee * lang.CurrencyExchangeRate) + lang.CurrencyCode;
            orderInfo += $"<td>Shipping:</td><td></td><td>-{shippingFee}</td>";
            orderInfo += "</tr><tr>";
            var cartTotal = (order.CartTotal * lang.CurrencyExchangeRate) + lang.CurrencyCode;
            orderInfo += $"<td>Subtotal:</td><td></td><td>{cartTotal}</td>";
            orderInfo += "</tr>";
            orderInfo += "</table>";

            if (e.PayStatus == (short)Enums.ShoppingCartPayStatus.PaymentSuccess)
            {
                var subject = "Your order completed, we doing for packing and shipping. Order code: " + e.OrderCode;
                var content = "Thank you for your order";
                content += orderInfo;
                MemoryMessageBuss.PushCommand(new SendEmailToUser(Guid.NewGuid(), addr.Email, addr.AddressName, subject, content, DateTime.Now, e.Id, DateTime.Now));
            }
        }

        public void Handle(UserCreatedResetPasswordConfirmCode e)
        {
            var subject = "Your reset password confirm code";
            var content = "This email to guide to reset password of your account: " + e.Username;
            content += "<br>Here is your confirm code: " + e.ConfirmCode;
            var siteUrl = e.WebsiteUrl.Trim(new[] { ' ', '/' });

            content += $"<br><a href='{siteUrl}/User/ResetPassword/?id={e.Id}&code={e.ConfirmCode}'>Click this link to reset password</a>";

            MemoryMessageBuss.PushCommand(new SendEmailToUser(Guid.NewGuid(), e.Email, e.Username, subject, content, DateTime.Now, e.Id, DateTime.Now));

        }

        public void Handle(UserResetPasswordDone e)
        {
            var subject = "Your new password was reset";
            var content = "Your account: " + e.Username;
            content += "<br>Here is your new password: " + e.NewPassword;
            content += "<br>Please change it as soon as posible";
            var siteUrl = e.WebsiteUrl.Trim(new[] { ' ', '/' });

            content += $"<br><a href='{siteUrl}/User/Login'>Click this link to login</a>";

            MemoryMessageBuss.PushCommand(new SendEmailToUser(Guid.NewGuid(), e.Email, e.Username, subject, content, DateTime.Now, e.Id, DateTime.Now));

        }

        public void Handle(UserActived e)
        {
            var subject = "Your account were actived";
            var content = $"This email address (as username: {e.Username}) were actived " ;
            var siteUrl = e.WebsiteUrl.Trim(new[] { ' ', '/' });

            content += $"<br><a href='{siteUrl}/User/Login/'>Click this link to login</a>";

            MemoryMessageBuss.PushCommand(new SendEmailToUser(Guid.NewGuid(), e.Email, e.Username, subject, content, DateTime.Now, e.Id, DateTime.Now));

        }

    }
}
