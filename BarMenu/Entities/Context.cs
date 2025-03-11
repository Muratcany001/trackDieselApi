
using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;

namespace BarMenu.Entities
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}
