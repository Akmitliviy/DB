using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;


public class Client
{

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [MaxLength(15)]
    public string PhoneNumber { get; set; }

    [Key]
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string DriverLicense { get; set; }

    public DateOnly BirthDate { get; set; }

    // Navigation property
    public ICollection<Rent>? Rents { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}