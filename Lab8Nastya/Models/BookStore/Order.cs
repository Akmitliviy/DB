using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("Orders", Schema = "dbo")]
    public partial class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("client_id")]
        [Required]
        public int ClientId { get; set; }

        public Client Client { get; set; }

        [Column("location_id")]
        public int? LocationId { get; set; }

        public Location Location { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("delivery_address")]
        [Required]
        public string DeliveryAddress { get; set; }

        [Column("receipt_number")]
        [Required]
        public string ReceiptNumber { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("receiving_date")]
        public DateTime? ReceivingDate { get; set; }

        public ICollection<BookAmountOrder> BookAmountOrders { get; set; }
    }
}