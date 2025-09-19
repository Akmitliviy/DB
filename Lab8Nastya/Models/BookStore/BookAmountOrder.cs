using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("BookAmountOrder", Schema = "dbo")]
    public partial class BookAmountOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("record_id")]
        public int RecordId { get; set; }

        [Column("order_id")]
        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Column("book_id")]
        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("sold_by_price")]
        public decimal? SoldByPrice { get; set; }
    }
}