using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("BookAmountLocation", Schema = "dbo")]
    public partial class BookAmountLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("record_id")]
        public int RecordId { get; set; }

        [Column("location_id")]
        [Required]
        public int LocationId { get; set; }

        public Location Location { get; set; }

        [Column("book_id")]
        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}