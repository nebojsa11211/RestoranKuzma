using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Reports
    {
        [Inject]
        private ApiService ApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Reports data would be loaded here
            await Task.CompletedTask;
        }
    }
}
