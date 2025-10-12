using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantSuite.Admin.Services;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Admin.Pages
{
    public partial class Tables
    {
        private List<TableDto> tables = new();
        private bool isLoading = true;
        private string? errorMessage;

        [Inject]
        private ApiService ApiService { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadTables();
        }

        private async Task LoadTables()
        {
            try
            {
                isLoading = true;
                tables = await ApiService.GetTablesAsync();
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

        private async Task ShowReserveTableModal()
        {
            // For now, show a simple alert. In a full implementation, this would open a modal
            // to select a table and enter reservation details
            await JSRuntime.InvokeVoidAsync("alert", "Table reservation feature coming soon! Click on an available table to reserve it.");
        }

        private async Task ReserveTable(Guid tableId)
        {
            try
            {
                await ApiService.ReserveTableAsync(tableId);
                await LoadTables(); // Refresh the table list
                await JSRuntime.InvokeVoidAsync("alert", "Table reserved successfully!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Failed to reserve table: {ex.Message}");
            }
        }

        private async Task OccupyTable(Guid tableId)
        {
            try
            {
                await ApiService.OccupyTableAsync(tableId);
                await LoadTables(); // Refresh the table list
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Failed to occupy table: {ex.Message}");
            }
        }

        private async Task MakeTableAvailable(Guid tableId)
        {
            try
            {
                await ApiService.MakeTableAvailableAsync(tableId);
                await LoadTables(); // Refresh the table list
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Failed to make table available: {ex.Message}");
            }
        }

        private string GetTableColorClass(TableDto table)
        {
            return table.Status switch
            {
                TableStatus.Available => "table-available",
                TableStatus.Occupied => "table-occupied",
                TableStatus.Reserved => "table-reserved",
                _ => "table-maintenance"
            };
        }

        private string GetTableTextColorClass(TableDto table)
        {
            return table.Status switch
            {
                TableStatus.Available => "text-green-800",
                TableStatus.Occupied => "text-red-800",
                TableStatus.Reserved => "text-yellow-800",
                _ => "text-gray-800"
            };
        }

        private string GetTableCursorClass(TableDto table)
        {
            return table.Status == TableStatus.Available ? "cursor-pointer" : "cursor-not-allowed";
        }

        private async Task HandleTableClick(TableDto table)
        {
            if (table.Status == TableStatus.Available)
            {
                var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Reserve {table.TableNumber} for a guest?");
                if (confirmed)
                {
                    await ReserveTable(table.Id);
                }
            }
            else if (table.Status == TableStatus.Reserved)
            {
                var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Mark {table.TableNumber} as occupied?");
                if (confirmed)
                {
                    await OccupyTable(table.Id);
                }
            }
            else if (table.Status == TableStatus.Occupied)
            {
                var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Make {table.TableNumber} available again?");
                if (confirmed)
                {
                    await MakeTableAvailable(table.Id);
                }
            }
        }
    }
}
