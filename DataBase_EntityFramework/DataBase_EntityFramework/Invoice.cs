using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;


public class Invoice
{
    [Key]
    public int ID { get; set; }

    public DateOnly PayTerm { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; }

    [MaxLength(30)]
    public string PaymentType { get; set; }

    // Foreign key
    public int RentId { get; set; }
    public Rent Rent { get; set; }
}