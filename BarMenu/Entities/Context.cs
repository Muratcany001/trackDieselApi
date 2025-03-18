
using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Entities
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}
