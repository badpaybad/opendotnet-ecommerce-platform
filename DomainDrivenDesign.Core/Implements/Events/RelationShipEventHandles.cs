using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipEventHandles : IEventHandle<RelationShipUpdated>
         , IEventHandle<RelationShipFromRemoved>, IEventHandle<RelationShipToRemoved>,
         IEventHandle<RelationShipAdded>, IEventHandle<RelationShipAddedOneFromWithManyTo>
        ,IEventHandle<RelationShipAddedManyFromWithOneTo>
        ,IEventHandle<RelationShipRemoved>
    {
        public void Handle(RelationShipUpdated e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.FirstOrDefault(i => i.FromId == e.OldFromId
                && i.ToId == e.OldToId);
                if (temp == null)
                {
                    temp = new RelationShip();
                    temp.FromId = e.FromId;
                    temp.ToId = e.ToId;
                    temp.FromTableName = e.FromTableName;
                    temp.ToTableName = e.ToTableName;
                    temp.DisplayOrder = e.DisplayOrder;
                    db.RelationShips.Add(temp);
                    db.SaveChanges();
                }
                else
                {
                    temp.FromId = e.FromId;
                    temp.ToId = e.ToId;
                    temp.FromTableName = e.FromTableName;
                    temp.ToTableName = e.ToTableName;
                    temp.DisplayOrder = e.DisplayOrder;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(RelationShipFromRemoved e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.Where(i => i.FromId == e.FromId).ToList();
                db.RelationShips.RemoveRange(temp);
                db.SaveChanges();
            }
        }

        public void Handle(RelationShipToRemoved e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.Where(i => i.ToId == e.ToId).ToList();
                db.RelationShips.RemoveRange(temp);
                db.SaveChanges();
            }
        }


        public void Handle(RelationShipAdded e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.FirstOrDefault(i => i.FromId == e.FromId
                                                                 && i.ToId == e.ToId);
                if (temp == null)
                {
                    temp = new RelationShip();
                    temp.FromId = e.FromId;
                    temp.ToId = e.ToId;
                    temp.FromTableName = e.FromTableName;
                    temp.ToTableName = e.ToTableName;
                    temp.DisplayOrder = e.DisplayOrder;
                    db.RelationShips.Add(temp);
                    db.SaveChanges();
                }

            }
        }

        public void Handle(RelationShipAddedOneFromWithManyTo e)
        {
            using (var db = new CoreDbContext())
            {
                foreach (var toId in e.ToIds)
                {
                    var temp = db.RelationShips.FirstOrDefault(i => i.FromId == e.FromId
                                                                          && i.ToId == toId);
                    if (temp == null)
                    {
                        temp = new RelationShip();
                        temp.FromId = e.FromId;
                        temp.ToId = toId;
                        temp.FromTableName = e.FromTableName;
                        temp.ToTableName = e.ToTableName;
                        temp.DisplayOrder = 0;
                        db.RelationShips.Add(temp);
                    }
                }
                db.SaveChanges();
            }
        }

        public void Handle(RelationShipAddedManyFromWithOneTo e)
        {
            using (var db = new CoreDbContext())
            {
                foreach (var fromId in e.FromIds)
                {
                    var temp = db.RelationShips.FirstOrDefault(i => i.FromId == fromId
                                                                    && i.ToId == e.ToId);
                    if (temp == null)
                    {
                        temp = new RelationShip();
                        temp.FromId = fromId;
                        temp.ToId = e.ToId;
                        temp.FromTableName = e.FromTableName;
                        temp.ToTableName = e.ToTableName;
                        temp.DisplayOrder = 0;
                        db.RelationShips.Add(temp);
                    }
                }
                db.SaveChanges();
            }
        }

        public void Handle(RelationShipRemoved e)
        {
            using (var db = new CoreDbContext())
            {
                var temp = db.RelationShips.FirstOrDefault(i => i.FromId == e.FromId
                                                                && i.ToId == e.ToId);
                if (temp != null)
                {
                    db.RelationShips.Remove(temp);
                    db.SaveChanges();
                }
               
            }
        }
    }

    public class RelationShipRemoved : IEvent {
        public Guid FromId { get; }
        public Guid ToId { get; }
        public string FromTableName { get; }
        public string ToTableName { get; }

        public RelationShipRemoved(Guid fromId, Guid toId, string fromTableName, string toTableName)
        {
            FromId = fromId;
            ToId = toId;
            FromTableName = fromTableName;
            ToTableName = toTableName;
        }

        public long Version { get; set; }
    }
}
