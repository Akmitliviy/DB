using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class DamageReport
{
    [Key]
    public int ID { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RepairCost { get; set; }

    public DateOnly ReportDate { get; set; }

    // Foreign Key - Rent (since damage reports are usually tied to a rental)
    public int RentId { get; set; }
    public Rent Rent { get; set; } // Navigation property
}