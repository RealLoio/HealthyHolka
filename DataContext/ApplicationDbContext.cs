using HealthyHolka.Models;

using Bogus;
using System;
using System.Collections.Generic;
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
            GenerateModel(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void GenerateModel(ModelBuilder modelBuilder)
        {
            Randomizer.Seed = new Random(1337);

            int numberOfEmployees = 10;
            int numberOfShifts = 50;

            List<Position> positions = new List<Position>() {
                new Position() { Id = 1, Name = "Менеджер" },
                new Position() { Id = 2, Name = "Инженер" },
                new Position() { Id = 3, Name = "Тестировщик свечей", RequiredWorkHours = new TimeSpan(12, 0, 0) } };
            List<Employee> employees = new List<Employee>();
            List<Shift> shifts = new List<Shift>();

            modelBuilder.Entity<Position>().HasData(positions);

            Faker<Employee> mockEmployee = new Faker<Employee>("ru")
                .RuleFor(e => e.LastName, e => e.Name.LastName())
                .RuleFor(e => e.FirstName, e => e.Name.FirstName())
                .RuleFor(e => e.MiddleName, e => $"{e.Name.FirstName()}ович")
                .RuleFor(e => e.PositionId, e => e.Random.Number(1, 3))
                .RuleFor(e => e.IsDeleted, e => e.Random.Bool(.17f));

            for (int i = 1; i <= numberOfEmployees; i++)
            {
                Employee employeeToAdd = new Employee { Id = i };
                mockEmployee.Populate(employeeToAdd);
                employees.Add(employeeToAdd);
            }

            modelBuilder.Entity<Employee>().HasData(employees);

            Faker<Shift> mockShift = new Faker<Shift>()
                .RuleFor(s => s.Start, s => s.Date.Between(
                    DateTime.Today.Date.Add(new TimeSpan(7, 27, 13)),
                    DateTime.Today.Add(new TimeSpan(10, 15, 40))))
                .RuleFor(s => s.End, s => s.Date.Between(
                    DateTime.Today.Date.Add(new TimeSpan(17, 27, 13)),
                    DateTime.Today.Add(new TimeSpan(23, 59, 59))));

            for (int i = 1; i <= numberOfShifts; i++)
            {
                Shift shiftToAdd = new Shift { Id = i };
                mockShift.Populate(shiftToAdd);
                shiftToAdd.EmployeeId = new Faker().Random.Int(1, numberOfEmployees);
                shiftToAdd.HoursWorked = (int)((DateTime)shiftToAdd.End).Subtract(shiftToAdd.Start).TotalHours;

                Employee employee = employees[shiftToAdd.EmployeeId - 1];
                Position position = positions[employee.PositionId - 1];

                DateTime requiredStartTime = shiftToAdd.Start.Add(position.StartingHour);
                DateTime requiredEndTime = ((DateTime)shiftToAdd.End)
                    .Add(shiftToAdd.Start.TimeOfDay)
                    .Add(position.RequiredWorkHours);
                if (shiftToAdd.Start.CompareTo(requiredStartTime) > 0)
                {
                    shiftToAdd.TimesViolated++;
                }
                if (((DateTime)shiftToAdd.End).CompareTo(requiredEndTime) < 0)
                {
                    shiftToAdd.TimesViolated++;
                }

                shifts.Add(shiftToAdd);
            }

            modelBuilder.Entity<Shift>().HasData(shifts);
        }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
    }
}