using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase_EntityFramework;


public class Invoice
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateOnly PayTerm { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal TotalCost { get; set; }

    [MaxLength(30)]
    [Required]
    public string PaymentType { get; set; }

    // Foreign keys
    public Guid RentId { get; set; }
    
    // Navigation properties
    public Rent Rent { get; set; }
}