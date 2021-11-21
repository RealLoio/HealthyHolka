using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("employee")]
    public class Employee
    {
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("last_name", TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [Required]
        [Column("first_name", TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [Column("middle_name", TypeName = "nvarchar(50)")]
        public string MiddleName { get; set; }

        [Required]
        [Column("position_id")]
        public int PositionId { get; set; }

        public Position Position { get; set; }

        public List<Shift> Shifts { get; set; } = new List<Shift>();

        [Column("is_enabled")]
        public bool IsEnabled { get; set; }
    }
}