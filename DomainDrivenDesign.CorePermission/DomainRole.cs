using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission.Comands;

namespace DomainDrivenDesign.CorePermission
{
    public class DomainRole
    {
        public void CreateRole(Guid id, string keyName, string title)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.Roles.FirstOrDefault(i => i.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (temp == null)
                {
                    db.Roles.Add(new Role()
                    {
                        KeyName = keyName,
                        Title = title,
                        Id = id
                    });
                    db.SaveChanges();
                }
            }
        }

        public void UpdateRole(Guid id, string keyName, string title)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.Roles.FirstOrDefault(i => i.Id == id);
                if (temp != null)
                {
                    temp.Title = title;
                    temp.KeyName = keyName;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateRightsForRole(Guid roleId, List<Guid> rightIds)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.Where(i => i.FromId == roleId).ToList();
                db.RelationShips.RemoveRange(temp);

                foreach (var rt in rightIds)
                {
                    db.RelationShips.Add(new RelationShip()
                    {
                        FromId = roleId,
                        ToId = rt,
                        FromTableName = "Role",
                        ToTableName = "Right"
                    });
                }

                db.SaveChanges();
            }
        }

        public void DeleteRole(Guid roleId)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.Roles.FirstOrDefault(i => i.Id == roleId);
                if (temp != null)
                {
                    db.Roles.Remove(temp);
                    var tempx = db.RelationShips.Where(i => i.FromId == roleId).ToList();
                    db.RelationShips.RemoveRange(tempx);
                    db.SaveChanges();
                }
            }
        }


    }
}
