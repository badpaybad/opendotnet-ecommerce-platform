using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.Core.Implements.Commands
{
    public class AdminBaseCommand : ICommand
    {
        public Guid UserId { get; }
        public DateTime CreatedDate { get; }

        public AdminBaseCommand(Guid userId, DateTime createdDate)
        {
            UserId = userId;
            CreatedDate = createdDate;
        }
    }
}
