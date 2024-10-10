using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;


public class Client
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [MaxLength(15)]
    public string PhoneNumber { get; set; }

    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string DriverLicense { get; set; }

    public DateTimeOffset BirthDate { get; set; }

    // Navigation property
    public ICollection<Rent> Rents { get; set; }
}