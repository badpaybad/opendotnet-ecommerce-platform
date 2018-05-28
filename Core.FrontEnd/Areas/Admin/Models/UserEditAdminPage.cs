using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.Core.Implements.Models;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class UserEditAdminPage
    {
        public Guid Id;
        public string Username;
        public string Phone;
        public string Email;
        public bool Actived;
        public bool Deleted;

        public List<Role> AllRoles=new List<Role>();

        public List<Role> Roles=new List<Role>();

        public PageMode PageMode
        {
            get
            {
                return Id == Guid.Empty ? Models.PageMode.Create : PageMode.Edit;
            }
        }
    }
}