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
    }
}