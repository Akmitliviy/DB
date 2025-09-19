using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("Books", Schema = "dbo")]
    public partial class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("title")]
        [Required]
        public string Title { get; set; }

        [Column("author_id")]
        [Required]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        [Column("category_id")]
        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Column("publication_year")]
        [Required]
        public int PublicationYear { get; set; }

        [Column("publisher_id")]
        [Required]
        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("price")]
        [Required]
        public decimal Price { get; set; }

        [Column("discount_percentage")]
        public decimal? DiscountPercentage { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("final_price")]
        public decimal? FinalPrice { get; set; }

        public ICollection<BookAmountLocation> BookAmountLocations { get; set; }

        public ICollection<BookAmountOrder> BookAmountOrders { get; set; }
    }
}