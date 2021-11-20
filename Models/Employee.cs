using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("employee")]
    public record Employee
    {
        [Column("id")]
        public int Id { get; init; }

        [Column("last_name", TypeName = "nvarchar(50)")]
        public string LastName { get; init; }

        [Column("first_name", TypeName = "nvarchar(50)")]
        public string FirstName { get; init; }

        [Column("middle_name", TypeName = "nvarchar(50)")]
        public string MiddleName { get; init; }

        [Column("position_id")]
        public int PositionId { get; init; }

        public Position Position { get; init; }

        public List<Shift> Shifts { get; init; } = new List<Shift>();
    }
}