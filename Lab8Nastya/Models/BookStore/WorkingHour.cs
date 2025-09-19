using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8Nastya.Models.BookStore
{
    [Table("WorkingHours", Schema = "dbo")]
    public partial class WorkingHour
    {
        [Key]
        [Column("location_id")]
        [Required]
        public int LocationId { get; set; }

        public Location Location { get; set; }

        [Column("weekdays_start")]
        [Required]
        public TimeOnly WeekdaysStart { get; set; }

        [Column("weekdays_end")]
        [Required]
        public TimeOnly WeekdaysEnd { get; set; }

        [Column("saturday_start")]
        public TimeOnly? SaturdayStart { get; set; }

        [Column("saturday_end")]
        public TimeOnly? SaturdayEnd { get; set; }

        [Column("sunday_start")]
        public TimeOnly? SundayStart { get; set; }

        [Column("sunday_end")]
        public TimeOnly? SundayEnd { get; set; }
    }
}