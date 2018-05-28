using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserCreated : IEvent
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Password { get; }
        public string Phone { get; }
        public string Email { get; }
        public DateTime CreatedDate { get; }
        public string WebsiteUrl { get; }


        public UserCreated(Guid id, string username, string password, string phone, string email, DateTime createdDate, string websiteUrl)
        {
            Id = id;
            Username = username;
            Password = password;
            Phone = phone;
            Email = email;
            CreatedDate = createdDate;
            WebsiteUrl = websiteUrl;
        }

        public long Version { get; set; }
    }
}