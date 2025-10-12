using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantSuite.Guest.Models;
using System.Net.Http.Json;

namespace RestaurantSuite.Guest.Pages
{
    public partial class ReserveTable
    {
        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Component State
        private TableReservationRequest reservationRequest = new();
        private bool isSubmitting = false;
        private bool showSuccessModal = false;

        // Available times for reservations
        private List<string> availableTimes = new()
        {
            "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM", "1:00 PM", "1:30 PM",
            "2:00 PM", "2:30 PM", "3:00 PM", "3:30 PM", "4:00 PM", "4:30 PM",
            "5:00 PM", "5:30 PM", "6:00 PM", "6:30 PM", "7:00 PM", "7:30 PM",
            "8:00 PM", "8:30 PM", "9:00 PM", "9:30 PM"
        };

        // Event Handlers
        private async Task HandleReservation()
        {
            try
            {
                isSubmitting = true;
                
                // TODO: Implement actual API call to create reservation
                // For now, simulate API call delay
                await Task.Delay(1500);
                
                // Simulate successful reservation
                Console.WriteLine($"Reservation created for {reservationRequest.GuestName}");
                
                showSuccessModal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating reservation: {ex.Message}");
                // TODO: Show error message to user
            }
            finally
            {
                isSubmitting = false;
            }
        }

        private void CloseSuccessModal()
        {
            showSuccessModal = false;
            // Reset form
            reservationRequest = new TableReservationRequest();
        }

        // Helper Methods
        private string GetModalDisplayClass()
        {
            return showSuccessModal ? "fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" : "hidden";
        }
    }
}
