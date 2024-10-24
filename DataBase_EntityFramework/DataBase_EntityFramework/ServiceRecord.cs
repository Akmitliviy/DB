using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class ServiceRecord
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public  DateOnly ServiceDate { get; set; }

    [MaxLength(500)]
    [Required]
    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal ServiceCost { get; set; }

    // Foreign keys
    
    [MaxLength(20)]
    [Required]
    public string VehicleLicencePlate { get; set; }
    
    // Navigation property
    public Vehicle Vehicle { get; set; }
}