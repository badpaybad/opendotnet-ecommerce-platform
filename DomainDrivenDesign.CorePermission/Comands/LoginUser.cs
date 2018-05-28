using System;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class LoginUser : ICommand
    {
        public LoginUser(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }

       
        
    }
}