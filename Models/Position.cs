using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyHolka.Models
{
    [Table("position")]
    public record Position
    { 
        [Column("id")]
        public int Id { get; init; }

        [Column("name", TypeName = "nvarchar(20)")]
        public string Name { get; init; }
    }
}