using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("InsurancePolicies", Schema = "public")]
    public partial class InsurancePolicy
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string PolicyNumber { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public string VehicleLicensePlate { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}