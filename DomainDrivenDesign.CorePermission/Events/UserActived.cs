using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserActived : IEvent
    {
        public Guid Id { get; }
        public string WebsiteUrl { get; }
        public string Email { get; }
        public string Username { get; }

        public UserActived(Guid id, string websiteUrl, string email, string username)
        {
            Id = id;
            WebsiteUrl = websiteUrl;
            Email = email;
            Username = username;
        }

        public long Version { get; set; }
    }


    public class UserResetPasswordDone : IEvent
    {
        public Guid Id { get; }
        public string Username { get; }
        public string NewPassword { get; }
        public string WebsiteUrl { get; }
        public string Email { get; }

        public UserResetPasswordDone(Guid id,string username, string newPassword,string websiteUrl, string email)
        {
            Id = id;
            Username = username;
            NewPassword = newPassword;
            WebsiteUrl = websiteUrl;
            Email = email;
        }

        public long Version { get; set; }
    }
    public class UserCreatedResetPasswordConfirmCode : IEvent
    {
        public Guid Id { get; }
        public string ConfirmCode { get; }
        public string Email { get; }
        public string Username { get; }
        public string WebsiteUrl { get; private set; }

        public UserCreatedResetPasswordConfirmCode(Guid id, string confirmCode, string email, string username, string websiteUrl)
        {
            Id = id;
            ConfirmCode = confirmCode;
            Email = email;
            Username = username;
            WebsiteUrl = websiteUrl;
        }

        public long Version { get; set; }
    }
}