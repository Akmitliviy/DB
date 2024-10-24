using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class Rent
{
    
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal Cost { get; set; }

    [MaxLength(20)]
    [Required]
    public string Status { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    // Foreign keys
    [MaxLength(20)]
    [Required]
    public string VehicleLicensePlate { get; set; }
    
    [MaxLength(100)]
    [Required]
    public string ClientEmail { get; set; }
    
    [Required]
    public Guid WorkerId { get; set; }

    // Navigation properties
    public Vehicle Vehicle { get; set; }
    public Client Client { get; set; }
    public Worker Worker { get; set; }
    public Invoice Invoice { get; set; }
    public ICollection<DamageReport>? DamageReports { get; set; }
}