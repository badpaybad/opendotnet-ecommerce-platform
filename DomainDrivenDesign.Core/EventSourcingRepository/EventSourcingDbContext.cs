using System.Data.Entity;
using DomainDrivenDesign.Core.Ef;

namespace DomainDrivenDesign.Core.EventSourcingRepository
{
    internal class EventSourcingDbContext : BaseDbContext
    {
        public DbSet<EventSourcingDescription> EventSoucings { get; set; }
    }
}