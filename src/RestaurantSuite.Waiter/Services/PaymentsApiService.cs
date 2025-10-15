using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Models;

namespace RestaurantSuite.Waiter.Services;

public class PaymentsApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public PaymentsApiService(HttpClient httpClient, AuthService authService)
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

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/payments", request);

            if (response.IsSuccessStatusCode)
            {
                var payment = await response.Content.ReadFromJsonAsync<Payment>();
                return new PaymentResponse
                {
                    Success = true,
                    Payment = payment
                };
            }

            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = $"Failed to create payment: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PaymentListResponse> ProcessSplitPaymentAsync(SplitPaymentRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/payments/split", request);

            if (response.IsSuccessStatusCode)
            {
                var payments = await response.Content.ReadFromJsonAsync<List<Payment>>();
                return new PaymentListResponse
                {
                    Success = true,
                    Payments = payments ?? new List<Payment>()
                };
            }

            return new PaymentListResponse
            {
                Success = false,
                ErrorMessage = $"Failed to process split payment: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentListResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
