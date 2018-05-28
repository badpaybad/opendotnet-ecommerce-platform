using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class CreateUser : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Password { get; }
        public string Phone { get; }
        public string Email { get; }
        public string WebsiteUrl { get; }
        public new DateTime CreatedDate { get; }


        public CreateUser(Guid id, string username, string password, string phone, string email, string websiteUrl, DateTime createdDate
            , Guid userId) : base(userId, createdDate)
        {
            Id = id;
            Username = username;
            Password = password;
            Phone = phone;
            Email = email;
            WebsiteUrl = websiteUrl;
            CreatedDate = createdDate;
        }
        
    }
}