using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserRegistered : IEvent
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }
        public string Phone { get; }
        public string ActiveCode { get; }
        public string WebsiteUrl { get; }
        public DateTime RegisteredDate { get; }

        public UserRegistered(Guid id, string username, string email, string password, string phone, string activeCode, string websiteUrl, DateTime registeredDate)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            Phone = phone;
            ActiveCode = activeCode;
            WebsiteUrl = websiteUrl;
            RegisteredDate = registeredDate;
        }

        public long Version { get; set; }
    }


    public class UserRegisteredFromGoogle : IEvent
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Username { get; }
        public string WebsiteUrl { get; }
        public string GoogleId { get; }
        public string GoogleAvatarUrl { get; }
        public string GoogleIdToken { get; }

        public UserRegisteredFromGoogle(Guid id, string email, string username, string websiteUrl, string googleId, string googleAvatarUrl, string googleIdToken)
        {
            Id = id;
            Email = email;
            Username = username;
            WebsiteUrl = websiteUrl;
            GoogleId = googleId;
            GoogleAvatarUrl = googleAvatarUrl;
            GoogleIdToken = googleIdToken;
        }

        public long Version { get; set; }
    }

    public class UserRegisteredFromFacebook : IEvent
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Username { get; }
        public string WebsiteUrl { get; }
        public string FacebookUserId { get; }
        public string AvatarUrl { get; }
        public string AccessToken { get; }

        public UserRegisteredFromFacebook(Guid id, string email, string username, string websiteUrl, string facebookUserId, string avatarUrl, string accessToken)
        {
            Id = id;
            Email = email;
            Username = username;
            WebsiteUrl = websiteUrl;
            FacebookUserId = facebookUserId;
            AvatarUrl = avatarUrl;
            AccessToken = accessToken;
        }

        public long Version { get; set; }
    }
}