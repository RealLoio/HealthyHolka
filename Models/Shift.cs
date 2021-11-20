using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("shift")]
    public record Shift
    {
        [Column("id")]
        public int Id { get; init; }

        [Column("started_on")]
        public DateTimeOffset Start { get; init; }

        [Column("ended_on")]
        public DateTimeOffset? End { get; init; }

        [Column("hours_worked")]
        public int HoursWorked { get; init; }

        [Column("employee_id")]
        public int EmployeeId { get; init; }

        public Employee Employee { get; init; }
    }
}