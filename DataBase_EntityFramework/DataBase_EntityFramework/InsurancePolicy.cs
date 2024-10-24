using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class InsurancePolicy
{
    
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(20)]
    [Required]
    public string PolicyNumber { get; set; }
    
    [MaxLength(50)]
    [Required]
    public string Provider { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal Cost { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    // Foreign keys
    [MaxLength(20)]
    [Required]
    public string VehicleLicensePlate { get; set; }
    
    // Navigation properties
    public Vehicle Vehicle { get; set; }
}