using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class LogoutUser : ICommand
    {
       
        public LogoutUser( string tokenSession)
        {
            TokenSession = tokenSession;
          
        }

        public string TokenSession { get; set; }
    }
}