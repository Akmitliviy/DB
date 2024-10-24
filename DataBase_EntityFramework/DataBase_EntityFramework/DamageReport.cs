using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class DamageReport
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(500)]
    [Required]
    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal RepairCost { get; set; }

    [Required]
    public DateOnly ReportDate { get; set; }

    // Foreign keys
    public Guid RentId { get; set; }
    
    // Navigation properties
    public Rent Rent { get; set; } 
}