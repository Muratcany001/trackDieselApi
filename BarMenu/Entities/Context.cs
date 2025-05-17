using BarMenu.Entities.AppEntities;
using Microsoft.EntityFrameworkCore;
    

namespace BarMenu.Entities
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<CarPart> CarParts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Car ↔ Issue ilişkisi
            modelBuilder.Entity<Car>()
                .HasMany(c => c.ErrorHistory)
                .WithOne(i => i.Car)
                .HasForeignKey(i => i.CarId);

            modelBuilder.Entity<CarPart>()
        .HasOne(cp => cp.Car)
        .WithMany(c => c.CarParts)
        .HasForeignKey(cp => cp.CarId);

            modelBuilder.Entity<CarPart>()
                .HasOne(cp => cp.Part)
                .WithMany(p => p.CarParts)
                .HasForeignKey(cp => cp.PartId);
        }
    }
}