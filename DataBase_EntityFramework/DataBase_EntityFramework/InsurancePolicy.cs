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

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    // Foreign Key - Rent
    public int RentId { get; set; }
    public Rent Rent { get; set; } // Navigation property
}