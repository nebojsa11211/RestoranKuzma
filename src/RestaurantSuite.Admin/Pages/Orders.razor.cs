using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Orders
    {
        [Inject]
        private OrdersApiService ApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Orders data would be loaded here
            await Task.CompletedTask;
        }
    }
}
