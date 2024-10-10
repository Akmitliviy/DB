using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;

public class Worker
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

    [MaxLength(50)]
    public string OccupationalPosition { get; set; }

    // Foreign keys
    public int? RentId { get; set; }
    public Rent Rent { get; set; }

    public int OfficeId { get; set; }
    public Office Office { get; set; }
}