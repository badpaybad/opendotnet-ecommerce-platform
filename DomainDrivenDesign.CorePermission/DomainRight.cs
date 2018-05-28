using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Reflection;

namespace DomainDrivenDesign.CorePermission
{
    public class DomainRight
    {
        public void UpdateDescription(Guid id, string description)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.Rights.FirstOrDefault(i => i.Id == id);
                if (temp != null)
                {
                    temp.Description = description;
                    db.SaveChanges();
                }
            }
        }
    }
}