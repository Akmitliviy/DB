using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Clients", Schema = "public")]
    public partial class Client
    {
        [Key]
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string DriverLicense { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }

        [Timestamp]
        [Column("RowVersion")]
        public byte[] RowVersion { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<Rent> Rents { get; set; }
    }
}