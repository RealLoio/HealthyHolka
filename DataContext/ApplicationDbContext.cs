using HealthyHolka.Models;

using System;
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
                new Position() { Id = 3, Name = "Тестировщик свечей", RequiredWorkHours = new TimeSpan(12, 0, 0) });

            // TODO use generator later instead of this
            modelBuilder.Entity<Employee>().HasData(
                new Employee() { Id = 1, LastName = "Тестовый", FirstName = "Тест", MiddleName = "Тестович", PositionId = 1 },
                new Employee() { Id = 2, LastName = "Тестовая", FirstName = "Теста", MiddleName = "Тестовна", PositionId = 3 });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
    }
}