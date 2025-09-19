using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Reviews", Schema = "public")]
    public partial class Review
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateOnly ReviewDate { get; set; }

        [Required]
        public string ClientEmail { get; set; }

        public Client Client { get; set; }

        [Required]
        public string VehicleLicensePlate { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}