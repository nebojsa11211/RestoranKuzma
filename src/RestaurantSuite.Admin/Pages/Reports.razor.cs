using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Reports
    {
        [Inject]
        private OrdersApiService OrdersApiService { get; set; } = default!;

        [Inject]
        private InventoryApiService InventoryApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Reports data would be loaded here
            await Task.CompletedTask;
        }
    }
}
