using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;


public class Client
{
    [Key]
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(50)]
    [Required]
    public string FirstName { get; set; }

    [MaxLength(50)]
    [Required]
    public string LastName { get; set; }

    [MaxLength(15)]
    [Required]
    public string PhoneNumber { get; set; }

    [MaxLength(20)]
    [Required]
    public string DriverLicense { get; set; }

    [Required]
    public DateOnly BirthDate { get; set; }

    // Navigation properties
    public ICollection<Rent>? Rents { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}