using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Models;

namespace RestaurantSuite.Waiter.Services;

public class InvoicesApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public InvoicesApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task EnsureAuthenticatedAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<InvoicesResponse> GetInvoicesAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync("api/invoices");

            if (response.IsSuccessStatusCode)
            {
                var invoices = await response.Content.ReadFromJsonAsync<List<Invoice>>();
                return new InvoicesResponse
                {
                    Success = true,
                    Invoices = invoices ?? new List<Invoice>()
                };
            }

            return new InvoicesResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch invoices: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new InvoicesResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<InvoiceResponse> GetInvoiceByIdAsync(Guid invoiceId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync($"api/invoices/{invoiceId}");

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<Invoice>();
                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }

            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch invoice: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<InvoiceResponse> GetInvoiceByOrderIdAsync(Guid orderId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync($"api/invoices/order/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<Invoice>();
                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }

            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch invoice for order: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/invoices", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var idObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                if (idObj != null && idObj.TryGetValue("id", out var idStr) && Guid.TryParse(idStr, out var id))
                {
                    // Fetch the created invoice
                    return await GetInvoiceByIdAsync(id);
                }
            }

            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = $"Failed to create invoice: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new InvoiceResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<string> GetInvoiceHtmlUrlAsync(Guid invoiceId)
    {
        return $"{_httpClient.BaseAddress}api/invoices/{invoiceId}/html";
    }

    public async Task<string> GetInvoicePdfUrlAsync(Guid invoiceId)
    {
        return $"{_httpClient.BaseAddress}api/invoices/{invoiceId}/pdf";
    }
}
