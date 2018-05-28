using System;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class RegisterUser : ICommand
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Password { get; }
        public string Phone { get; }
        public string Address { get; private set; }
        public string WebsiteUrl { get; }

        public RegisterUser(Guid id,  string email, string password, string phone,string address,string websiteUrl)
        {
            Id = id;
            Email = email;
            Password = password;
            Phone = phone;
            Address = address;
            WebsiteUrl = websiteUrl;
        }
        
    }
}