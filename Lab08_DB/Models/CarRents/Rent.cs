using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Rents", Schema = "public")]
    public partial class Rent
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        [Required]
        public string VehicleLicensePlate { get; set; }

        public Vehicle Vehicle { get; set; }

        [Required]
        public string ClientEmail { get; set; }

        public Client Client { get; set; }

        [Required]
        public Guid WorkerId { get; set; }

        public Worker Worker { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<DamageReport> DamageReports { get; set; }
    }
}