using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class InsurancePolicy
{
    
    [Key]
    public int ID { get; set; }

    public string PolicyNumber { get; set; }

    public string Provider { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    // Foreign Key - Vehicle
    public string VehicleLicensePlate { get; set; }
    public Vehicle Vehicle { get; set; } // Navigation property
}