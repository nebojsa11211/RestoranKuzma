using Microsoft.AspNetCore.Components;
using RestaurantSuite.Admin.Services;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Index
    {
        private List<RestaurantSuite.Application.DTOs.RestaurantDto>? restaurants;
        private List<TableDto>? tables;
        
        // Table statistics
        private int totalTables = 0;
        private int occupiedTables = 0;
        private int availableTables = 0;
        private int reservedTables = 0;
        private int occupancyPercentage = 0;

        [Inject]
        private ApiService ApiService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Load restaurant and table data
            try
            {
                restaurants = await ApiService.GetRestaurantsAsync();
                tables = await ApiService.GetTablesAsync();
                
                // Calculate table statistics
                CalculateTableStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                restaurants = new List<RestaurantSuite.Application.DTOs.RestaurantDto>();
                tables = new List<TableDto>();
            }
        }
        
        private void CalculateTableStatistics()
        {
            if (tables == null || tables.Count == 0)
            {
                totalTables = 0;
                occupiedTables = 0;
                availableTables = 0;
                reservedTables = 0;
                occupancyPercentage = 0;
                return;
            }
            
            totalTables = tables.Count;
            occupiedTables = tables.Count(t => t.Status == TableStatus.Occupied);
            availableTables = tables.Count(t => t.Status == TableStatus.Available);
            reservedTables = tables.Count(t => t.Status == TableStatus.Reserved);
            
            // Calculate occupancy percentage
            occupancyPercentage = totalTables > 0 ? (occupiedTables * 100) / totalTables : 0;
        }
    }
}
