using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Management_system.Models
{
    public class Event
    {
        [Key]
        public int EVENT_ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Event Name")]
        public string EVENT_NAME { get; set; }

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.DateTime)]
        public DateTime EVENT_DATE { get; set; }

        [Display(Name = "Description")]
        public string? DESCRIPTION { get; set; }

        [Required]
        [ForeignKey("Venue")]
        [Display(Name = "Venue Id")]
        public int VENUE_ID { get; set; }

        public virtual Venue Venue { get; set; }  // Navigation property
    }
}
