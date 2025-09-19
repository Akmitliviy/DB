using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("Locations", Schema = "dbo")]
    public partial class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("location_id")]
        public int LocationId { get; set; }

        [Column("location_type")]
        [Required]
        public string LocationType { get; set; }

        [Column("address")]
        [Required]
        public string Address { get; set; }

        [Column("phone_number")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        public ICollection<BookAmountLocation> BookAmountLocations { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<WorkingHour> WorkingHours { get; set; }
    }
}