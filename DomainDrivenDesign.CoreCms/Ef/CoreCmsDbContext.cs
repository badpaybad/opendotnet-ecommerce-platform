using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements;
using MySql.Data.Entity;

namespace DomainDrivenDesign.CoreCms.Ef
{
    public class CoreCmsDbContext : CoreDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<HomePageSection> HomePageSections { get; set; }
    }
}
