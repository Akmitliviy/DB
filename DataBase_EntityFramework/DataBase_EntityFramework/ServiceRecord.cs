using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class ServiceRecord
{
    [Key]
    public int ID { get; set; }

    public DateOnly ServiceDate { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ServiceCost { get; set; }

    // Foreign Key - Auto
    public string VehicleLicencePlate { get; set; }
    public Vehicle Vehicle { get; set; } // Navigation property
}