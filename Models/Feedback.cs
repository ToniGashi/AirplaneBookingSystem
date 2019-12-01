using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirplaneBookingSystem.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public string Email { get; set; }

        [Required]
        [DisplayName("Title: ")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Comment: ")]
        public string Body { get; set; }
    }
}
