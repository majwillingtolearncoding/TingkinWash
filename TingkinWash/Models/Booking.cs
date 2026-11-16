
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TingkinWash.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string ServiceName { get; set; } = "";

        // Admin will enter the weight
        [Range(0.01, 1000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Weight { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }

        [Required]
        public TimeSpan PickupTime { get; set; }

        [Required]
        public string PickupAddress { get; set; } = "";

        // Admin will set the price
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime BookingDate { get; set; }
            = DateTime.Now;
    }
}
