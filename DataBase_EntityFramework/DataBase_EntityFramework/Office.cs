using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;


public class Office
{

    [Key]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(100)]
    public string Address { get; set; }

    [MaxLength(15)]
    [Required]
    public string PhoneNumber { get; set; }

    // Navigation properties
    public ICollection<Worker> Workers { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; }
}