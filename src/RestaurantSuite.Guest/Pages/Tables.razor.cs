using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantSuite.Guest.Models;
using System.Net.Http.Json;

namespace RestaurantSuite.Guest.Pages
{
    public partial class Tables
    {
        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Component State
        private List<TableDto>? tables;
        private bool isLoading = true;
        private string? errorMessage;

        // Computed Properties
        private int occupiedTablesCount => tables?.Count(t => t.Status == "Occupied") ?? 0;
        private int availableTablesCount => tables?.Count(t => t.Status == "Available") ?? 0;
        private int reservedTablesCount => tables?.Count(t => t.Status == "Reserved") ?? 0;

        // Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadTables();
        }

        // Data Loading Methods
        private async Task LoadTables()
        {
            try
            {
                isLoading = true;
                // Call the API to get real table data
                tables = await Http.GetFromJsonAsync<List<TableDto>>("api/tables");
                errorMessage = null;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to load tables: " + ex.Message;
                tables = new List<TableDto>();
            }
            finally
            {
                isLoading = false;
            }
        }

        // Event Handlers
        private void NavigateToReserveTable()
        {
            NavigationManager.NavigateTo("/reserve-table");
        }

        private string GetTableColorClass(TableDto table)
        {
            return table.Status switch
            {
                "Available" => "bg-green-100 border-green-300 hover:bg-green-200",
                "Occupied" => "bg-red-100 border-red-300 hover:bg-red-200",
                "Reserved" => "bg-yellow-100 border-yellow-300 hover:bg-yellow-200",
                _ => "bg-gray-100 border-gray-300"
            };
        }

        private string GetTableTextColorClass(TableDto table)
        {
            return table.Status switch
            {
                "Available" => "text-green-800",
                "Occupied" => "text-red-800",
                "Reserved" => "text-yellow-800",
                _ => "text-gray-800"
            };
        }

        private string GetTableCursorClass(TableDto table)
        {
            return table.Status == "Available" ? "cursor-pointer" : "cursor-not-allowed";
        }

        private void HandleTableClick(TableDto table)
        {
            if (table.Status == "Available")
            {
                NavigationManager.NavigateTo($"/reserve-table?table={table.TableNumber}");
            }
        }

        // Helper Classes
        public class TableDto
        {
            public Guid Id { get; set; }
            public string TableNumber { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
