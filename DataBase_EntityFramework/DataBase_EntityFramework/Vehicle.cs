using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class Vehicle
{
    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; }

    [MaxLength(50)]
    public string Model { get; set; }

    public int Year { get; set; }

    [MaxLength(20)]
    public string FuelType { get; set; }

    public int Mileage { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPerDay { get; set; }
    
    public Rent Rent { get; set; }

    // Foreign key
    public int OfficeId { get; set; }
    public Office Office { get; set; }
    public ICollection<ServiceRecord>? ServiceRecords { get; set; }
}