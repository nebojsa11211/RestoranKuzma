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

        // Component State - Tables
        private List<TableDto>? tables;
        private bool isLoading = true;
        private string? errorMessage;

        // Component State - View Management
        private string activeView = "tables";
        private bool isQuickReservation = false;
        private TableDto? selectedTable = null;

        // Component State - Table Filtering
        private DateTime filterDate = DateTime.Today;
        private string filterTime = string.Empty;
        private List<TableDto>? availableTablesForDateTime;

        // Component State - Reservation
        private TableReservationRequest reservationRequest = new();
        private bool isSubmitting = false;
        private bool showSuccessModal = false;

        // Available times for reservations
        private List<string> availableTimes = new()
        {
            "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM", "1:00 PM", "1:30 PM",
            "2:00 PM", "2:30 PM", "3:00 PM", "3:30 PM", "4:00 PM", "4:30 PM",
            "5:00 PM", "5:30 PM", "6:00 PM", "6:30 PM", "7:00 PM", "7:30 PM",
            "8:00 PM", "8:30 PM", "9:00 PM", "9:30 PM"
        };

        // Computed Properties
        private int occupiedTablesCount => tables?.Count(t => t.Status == "Occupied") ?? 0;
        private int availableTablesCount => tables?.Count(t => t.Status == "Available") ?? 0;
        private int reservedTablesCount => tables?.Count(t => t.Status == "Reserved") ?? 0;

        // Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadTables();

            // Check if navigated with table parameter
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var tableParam = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("table");
            if (!string.IsNullOrEmpty(tableParam))
            {
                // Find the table and pre-select it
                var table = tables?.FirstOrDefault(t => t.TableNumber == tableParam);
                if (table != null && table.Status == "Available")
                {
                    selectedTable = table;
                    StartQuickReservation();
                }
            }
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

        private async Task UpdateTableAvailability()
        {
            if (!string.IsNullOrEmpty(filterTime) && filterDate != default)
            {
                try
                {
                    // TODO: Call API to get available tables for specific date/time
                    // For now, filter based on current status
                    availableTablesForDateTime = tables?.Where(t => t.Status == "Available").ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating table availability: {ex.Message}");
                }
            }
        }

        // View Management Methods
        private void SetActiveView(string view)
        {
            activeView = view;
            if (view == "tables")
            {
                isQuickReservation = false;
                selectedTable = null;
            }
        }

        private string GetViewButtonClass(string view)
        {
            return activeView == view ? "active" : "";
        }

        // Table Selection Methods
        private void HandleTableSelection(TableDto table)
        {
            if (table.Status == "Available")
            {
                selectedTable = table;
            }
        }

        private void CancelTableSelection()
        {
            selectedTable = null;
        }

        private void StartQuickReservation()
        {
            if (selectedTable != null)
            {
                isQuickReservation = true;
                activeView = "reservation";
                // Pre-fill reservation with selected table capacity
                if (selectedTable.Capacity < reservationRequest.NumberOfGuests)
                {
                    reservationRequest.NumberOfGuests = selectedTable.Capacity;
                }
            }
        }

        private void CancelQuickReservation()
        {
            isQuickReservation = false;
            activeView = "tables";
            selectedTable = null;
        }

        private void SelectTableForReservation(TableDto table)
        {
            selectedTable = table;
            // Adjust guest count if needed
            if (table.Capacity < reservationRequest.NumberOfGuests)
            {
                reservationRequest.NumberOfGuests = table.Capacity;
            }
        }

        // Reservation Form Methods
        private async Task OnDateChanged(ChangeEventArgs e)
        {
            await UpdateTableAvailability();
        }

        private async Task OnTimeChanged(ChangeEventArgs e)
        {
            await UpdateTableAvailability();
        }

        private async Task HandleReservation()
        {
            try
            {
                isSubmitting = true;

                // Create reservation object with table information if selected
                var reservation = new
                {
                    reservationRequest.GuestName,
                    reservationRequest.Email,
                    reservationRequest.PhoneNumber,
                    reservationRequest.ReservationDate,
                    reservationRequest.ReservationTime,
                    reservationRequest.NumberOfGuests,
                    reservationRequest.SpecialRequests,
                    TableId = selectedTable?.Id,
                    TableNumber = selectedTable?.TableNumber
                };

                // TODO: Implement actual API call to create reservation
                // var response = await Http.PostAsJsonAsync("api/reservations", reservation);
                // if (response.IsSuccessStatusCode)
                // {
                //     showSuccessModal = true;
                //     await LoadTables(); // Refresh table status
                // }

                // For now, simulate API call delay
                await Task.Delay(1500);

                Console.WriteLine($"Reservation created for {reservationRequest.GuestName}");
                if (selectedTable != null)
                {
                    Console.WriteLine($"Table {selectedTable.TableNumber} reserved");
                }

                showSuccessModal = true;
                isQuickReservation = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating reservation: {ex.Message}");
                errorMessage = "Failed to create reservation. Please try again.";
            }
            finally
            {
                isSubmitting = false;
            }
        }

        private void CloseSuccessModal()
        {
            showSuccessModal = false;
            // Reset form and state
            reservationRequest = new TableReservationRequest();
            selectedTable = null;
            activeView = "tables";
            isQuickReservation = false;
            // Reload tables to show updated status
            _ = LoadTables();
        }

        // Style Helper Methods
        private string GetTableStatusClass(TableDto table)
        {
            return table.Status.ToLower();
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
                HandleTableSelection(table);
            }
        }

        private string GetModalDisplayClass()
        {
            return showSuccessModal ? "fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" : "hidden";
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