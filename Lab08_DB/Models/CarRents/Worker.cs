using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Workers", Schema = "public")]
    public partial class Worker
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string OccupationalPosition { get; set; }

        [Required]
        public string OfficeName { get; set; }

        public Office Office { get; set; }

        public ICollection<Rent> Rents { get; set; }
    }
}