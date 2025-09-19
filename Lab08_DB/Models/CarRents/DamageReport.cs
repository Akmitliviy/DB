using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("DamageReports", Schema = "public")]
    public partial class DamageReport
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal RepairCost { get; set; }

        [Required]
        public DateOnly ReportDate { get; set; }

        [Required]
        public Guid RentId { get; set; }

        public Rent Rent { get; set; }
    }
}