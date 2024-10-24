using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class Vehicle
{
    [Key]
    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; }

    [MaxLength(50)]
    [Required]
    public string Model { get; set; }

    [Required]
    public int Year { get; set; }

    [MaxLength(20)]
    [Required]
    public string FuelType { get; set; }

    [Required]
    public int Mileage { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal CostPerDay { get; set; }

    // Foreign keys
    [MaxLength(50)]
    [Required]
    public string OfficeName { get; set; }
    
    // Navigation properties
    public Office Office { get; set; }
    public ICollection<ServiceRecord>? ServiceRecords { get; set; }
    public ICollection<InsurancePolicy>? InsurancePolicies { get; set; }
    public ICollection<Rent>? Rents { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}