using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class Rent
{
    
    [Key]
    public int ID { get; set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [MaxLength(20)]
    public string Status { get; set; }

    // Foreign keys
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    public Worker Worker { get; set; }

    // Navigation property
    public ICollection<Payment> Payments { get; set; }
}