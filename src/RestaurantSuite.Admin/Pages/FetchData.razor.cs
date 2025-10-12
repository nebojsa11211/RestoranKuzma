using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Data;

namespace RestaurantSuite.Admin.Pages
{
    public partial class FetchData
    {
        private WeatherForecast[]? forecasts;

        [Inject]
        private WeatherForecastService ForecastService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
        }
    }
}
