using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;

public class ServiceRecord
{
    [Key]
    public int ID { get; set; }

    public DateTime ServiceDate { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ServiceCost { get; set; }

    // Foreign Key - Auto
    public int AutoId { get; set; }
    public Vehicle Vehicle { get; set; } // Navigation property
}