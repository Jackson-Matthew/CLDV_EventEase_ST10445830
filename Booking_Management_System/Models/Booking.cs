using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Management_system.Models
{
    [Table("BOOKINGS")]
    public class Booking
    {
        [Key]
        public int BOOKING_ID { get; set; }

        [Required]
        [Display(Name = "Booking date")]
        [DataType(DataType.DateTime)]
        public DateTime BOOKING_DATE { get; set; }

        [Required]
        [Display(Name = "Event Id")]
        public int EVENT_ID { get; set; }

        [Required]
        [Display(Name = "Venue Id")]
        public int VENUE_ID { get; set; }

        [ForeignKey(nameof(EVENT_ID))]
        public Event? Event { get; set; }

        [ForeignKey(nameof(VENUE_ID))]
        public Venue? Venue { get; set; }
    }
}
