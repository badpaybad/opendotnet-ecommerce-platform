using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class ChangePasswordByUser : AdminBaseCommand
    {
        public Guid Id { get; }
        public string OldPassword { get; private set; }
        public string NewPassword { get; }

        public ChangePasswordByUser(Guid id, string oldPassword, string newPassword
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public long Version { get; set; }
    }

    public class ChangePasswordByAdmin : AdminBaseCommand
    {
        public Guid Id { get; }
        public string NewPassword { get; private set; }

        public ChangePasswordByAdmin(Guid id,  string newPassword
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            NewPassword = newPassword;
        }

        public long Version { get; set; }
    }
}
