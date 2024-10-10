using System.ComponentModel.DataAnnotations;

namespace DataBase_EntityFramework;

public class Review
{
    [Key]
    public int ID { get; set; }

    public int Rating { get; set; } // Example: 1-5 scale

    [MaxLength(500)]
    public string Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    // Foreign Key - Client (the client who wrote the review)
    public int ClientId { get; set; }
    public Client Client { get; set; } // Navigation property

    // Foreign Key - Rent (the rental the review is associated with)
    public int RentId { get; set; }
    public Rent Rent { get; set; } // Navigation property
}