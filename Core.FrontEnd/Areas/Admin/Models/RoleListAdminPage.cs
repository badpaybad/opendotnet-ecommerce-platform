using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainDrivenDesign.Core.Implements.Models;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class RoleListAdminPage
    {
        public List<Role> Roles;
        public List<Right> Rights;
    }
}