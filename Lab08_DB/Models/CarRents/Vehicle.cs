using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Vehicles", Schema = "public")]
    public partial class Vehicle
    {
        [Key]
        [Required]
        public string LicensePlate { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string FuelType { get; set; }

        [Required]
        public int Mileage { get; set; }

        [Required]
        public decimal CostPerDay { get; set; }

        [Required]
        public string OfficeName { get; set; }

        public Office Office { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<InsurancePolicy> InsurancePolicies { get; set; }

        public ICollection<Rent> Rents { get; set; }

        public ICollection<ServiceRecord> ServiceRecords { get; set; }
    }
}