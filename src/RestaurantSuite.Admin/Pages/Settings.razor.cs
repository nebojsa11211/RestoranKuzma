using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Settings
    {
        [Inject]
        private ApiService ApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Settings data would be loaded here
            await Task.CompletedTask;
        }
    }
}
