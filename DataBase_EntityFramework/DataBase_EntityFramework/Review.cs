using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Rating { get; set; } // Example: 1-5 scale

    [MaxLength(500)]
    public string? Comment { get; set; }

    [Required]
    public DateOnly ReviewDate { get; set; }

    // Foreign keys
    
    [MaxLength(100)]
    [Required]
    public string ClientEmail { get; set; }
    
    [MaxLength(20)]
    [Required]
    public string VehicleLicensePlate { get; set; }
    
    // Navigation properties
    public Client Client { get; set; } 
    public Vehicle Vehicle { get; set; }
}