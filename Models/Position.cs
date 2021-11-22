using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("position")]
    public class Position
    { 
        [Column("id")]
        public int Id { get; set; }

        [Column("name", TypeName = "nvarchar(20)")]
        public string Name { get; set; }

        [Column("starting_hour")]
        public TimeSpan StartingHour { get; set; } = new TimeSpan(9, 0, 0);

        [Column("required_work_hours")]
        public TimeSpan RequiredWorkHours { get; set; } = new TimeSpan(9, 0, 0);
    }
}