using System.ComponentModel.DataAnnotations;

namespace RestaurantSuite.Guest.Models
{
    public class TableReservationRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reservation date is required")]
        public DateTime ReservationDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Reservation time is required")]
        public string ReservationTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int NumberOfGuests { get; set; } = 2;

        public string? SpecialRequests { get; set; }
    }
}
