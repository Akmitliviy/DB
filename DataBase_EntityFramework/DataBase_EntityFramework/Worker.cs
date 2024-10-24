using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;

public class Worker
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(50)]
    [Required]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [MaxLength(15)]
    [Required]
    public string PhoneNumber { get; set; }

    [MaxLength(50)]
    [Required]
    public string OccupationalPosition { get; set; }
    
    // Foreign keys
    [MaxLength(50)]
    [Required]
    public string OfficeName { get; set; }
    
    // Navigation properties
    public Office Office { get; set; }
    public ICollection<Rent>? Rents { get; set; }
}