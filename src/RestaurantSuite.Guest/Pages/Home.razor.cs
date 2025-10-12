using Microsoft.AspNetCore.Components;

namespace RestaurantSuite.Guest.Pages
{
    public partial class Home
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Navigation Methods
        private void NavigateToMenu()
        {
            NavigationManager.NavigateTo("/menu");
        }

        private void NavigateToReserveTable()
        {
            NavigationManager.NavigateTo("/reserve-table");
        }

        private void NavigateToOrder()
        {
            NavigationManager.NavigateTo("/order");
        }

        private void CallService()
        {
            // TODO: Implement service call functionality
            Console.WriteLine("Service requested - Waiter will be notified");
        }
    }
}
