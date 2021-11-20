using HealthyHolka.Models;

using Microsoft.EntityFrameworkCore;

namespace HealthyHolka.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>().HasData(
                new Position() { Id = 1, Name = "Менеджер" },
                new Position() { Id = 2, Name = "Инженер" },
                new Position() { Id = 3, Name = "Тестировщик свечей" });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
    }
}