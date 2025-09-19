using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab08DB.Models.CarRents
{
    [Table("Offices", Schema = "public")]
    public partial class Office
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

        public ICollection<Worker> Workers { get; set; }
    }
}