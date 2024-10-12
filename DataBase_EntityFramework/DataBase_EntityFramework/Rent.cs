using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class Rent
{
    
    [Key]
    public int ID { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [MaxLength(20)]
    public string Status { get; set; }

    // Foreign keys
    
    public string VehicleLicensePlate { get; set; }
    public Vehicle Vehicle { get; set; }

    public string ClientEmail { get; set; }
    public Client Client { get; set; }
    
    //Foreign key
    public int WorkerId { get; set; }
    public Worker Worker { get; set; }

    // Navigation property
    public Invoice Invoice { get; set; }
    public ICollection<DamageReport>? DamageReports { get; set; }
}