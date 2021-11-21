using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("shift")]
    public class Shift
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("started_on")]
        public DateTimeOffset Start { get; set; }

        [Column("ended_on")]
        public DateTimeOffset? End { get; set; }

        [Column("hours_worked")]
        public int HoursWorked { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}