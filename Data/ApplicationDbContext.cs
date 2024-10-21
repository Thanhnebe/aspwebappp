using Microsoft.EntityFrameworkCore;
using webappmvcasp.Models;

namespace MyMvcApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
 public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public DbSet<User> Users { get; set; }
    }
}
