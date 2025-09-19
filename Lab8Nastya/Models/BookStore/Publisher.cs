using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("Publishers", Schema = "dbo")]
    public partial class Publisher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("publisher_id")]
        public int PublisherId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("phone_number")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column("email")]
        [Required]
        public string Email { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}