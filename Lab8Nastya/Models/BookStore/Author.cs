using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("Authors", Schema = "dbo")]
    public partial class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("author_id")]
        public int AuthorId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Timestamp]
        [Column("row_version")]
        public byte[] RowVersion { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}