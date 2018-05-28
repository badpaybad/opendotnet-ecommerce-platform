using System.Data.Entity;
using DomainDrivenDesign.Core.Ef;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements.Models;
using MySql.Data.Entity;

namespace DomainDrivenDesign.Core.Implements
{
    public  class CoreDbContext : BaseDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<ContentLanguage> ContentLanguages { get; set; }
        public DbSet<RelationShip> RelationShips { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<EventSourcingDescription> EventSourcingDescriptions { get; set; }
        public DbSet<UrlFriendly> UrlFriendlys { get; set; }
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<ContactUsInfo> ContactUsInfos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserMessageTransaction> UserMessageTransactions { get; set; }
    }
}