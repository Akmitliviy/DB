using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("ServiceRecords", Schema = "public")]
    public partial class ServiceRecord
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateOnly ServiceDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal ServiceCost { get; set; }

        [Required]
        public string VehicleLicencePlate { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}