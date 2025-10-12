using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Orders
    {
        [Inject]
        private ApiService ApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Orders data would be loaded here
            await Task.CompletedTask;
        }
    }
}
