using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Invoices", Schema = "public")]
    public partial class Invoice
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateOnly PayTerm { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        public Guid RentId { get; set; }

        public Rent Rent { get; set; }
    }
}