using Microsoft.AspNetCore.SignalR.Client;
using RestaurantSuite.Waiter.Models;
using System.Diagnostics;

namespace RestaurantSuite.Waiter.Services;

public class SignalRService : IAsyncDisposable
{
    private readonly AuthService _authService;
    private readonly string _hubUrl;
    private HubConnection? _hubConnection;
    private bool _isConnected = false;
    private readonly int _maxReconnectAttempts = 5;
    private int _reconnectAttempts = 0;

    // Events for order updates
    public event Action<Order>? OrderCreated;
    public event Action<Guid, OrderStatus, DateTime>? OrderStatusChanged;
    public event Action<Guid, Guid, string>? OrderLineStatusChanged;
    public event Action<Guid, string>? OrderCancelled;
    public event Action<Guid, decimal>? PaymentCompleted;
    public event Action? ConnectionStateChanged;

    public bool IsConnected => _isConnected;

    public SignalRService(AuthService authService, string apiBaseUrl)
    {
        _authService = authService;
        _hubUrl = $"{apiBaseUrl.TrimEnd('/')}/hubs/orders";
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null)
        {
            Console.WriteLine("SignalR: Connection already exists");
            return;
        }

        try
        {
            var token = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("SignalR: No access token available");
                return;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.AccessTokenProvider = async () => await _authService.GetAccessTokenAsync();
                })
                .WithAutomaticReconnect(new[]
                {
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .Build();

            // Register event handlers
            RegisterHandlers();

            // Connection state events
            _hubConnection.Closed += OnConnectionClosed;
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;

            await _hubConnection.StartAsync();
            _isConnected = true;
            _reconnectAttempts = 0;
            Console.WriteLine("SignalR: Connected successfully");
            ConnectionStateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR: Failed to connect: {ex.Message}");
            await HandleConnectionFailure();
        }
    }

    private void RegisterHandlers()
    {
        if (_hubConnection == null) return;

        // Order created event
        _hubConnection.On<Order>("OrderCreated", (order) =>
        {
            Console.WriteLine($"SignalR: Order created - {order.Id}");
            OrderCreated?.Invoke(order);
        });

        // Order status changed event
        _hubConnection.On<Guid, string, DateTime, string>("OrderStatusChanged", (orderId, newStatus, timestamp, updatedBy) =>
        {
            Console.WriteLine($"SignalR: Order {orderId} status changed to {newStatus}");
            if (Enum.TryParse<OrderStatus>(newStatus, out var status))
            {
                OrderStatusChanged?.Invoke(orderId, status, timestamp);
            }
        });

        // Order line status changed event
        _hubConnection.On<Guid, Guid, string, DateTime>("OrderLineStatusChanged", (orderId, orderLineId, newStatus, timestamp) =>
        {
            Console.WriteLine($"SignalR: Order line {orderLineId} status changed to {newStatus}");
            OrderLineStatusChanged?.Invoke(orderId, orderLineId, newStatus);
        });

        // Order cancelled event
        _hubConnection.On<Guid, string, DateTime>("OrderCancelled", (orderId, reason, timestamp) =>
        {
            Console.WriteLine($"SignalR: Order {orderId} cancelled - {reason}");
            OrderCancelled?.Invoke(orderId, reason);
        });

        // Payment completed event
        _hubConnection.On<Guid, decimal, string, DateTime>("PaymentCompleted", (orderId, amount, method, timestamp) =>
        {
            Console.WriteLine($"SignalR: Payment completed for order {orderId} - ${amount}");
            PaymentCompleted?.Invoke(orderId, amount);
        });
    }

    private Task OnConnectionClosed(Exception? exception)
    {
        _isConnected = false;
        Console.WriteLine($"SignalR: Connection closed - {exception?.Message}");
        ConnectionStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    private Task OnReconnecting(Exception? exception)
    {
        _isConnected = false;
        _reconnectAttempts++;
        Console.WriteLine($"SignalR: Reconnecting (attempt {_reconnectAttempts}) - {exception?.Message}");
        ConnectionStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        _isConnected = true;
        _reconnectAttempts = 0;
        Console.WriteLine($"SignalR: Reconnected - ConnectionId: {connectionId}");
        ConnectionStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    private async Task HandleConnectionFailure()
    {
        if (_reconnectAttempts < _maxReconnectAttempts)
        {
            _reconnectAttempts++;
            var delay = TimeSpan.FromSeconds(Math.Pow(2, _reconnectAttempts));
            Console.WriteLine($"SignalR: Retrying connection in {delay.TotalSeconds} seconds (attempt {_reconnectAttempts}/{_maxReconnectAttempts})");

            await Task.Delay(delay);
            await StartAsync();
        }
        else
        {
            Console.WriteLine("SignalR: Max reconnection attempts reached. Giving up.");
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.StopAsync();
                _isConnected = false;
                Console.WriteLine("SignalR: Disconnected");
                ConnectionStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Error stopping connection: {ex.Message}");
            }
        }
    }

    public async Task JoinWaiterGroup(Guid waiterId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("JoinWaiterGroup", waiterId.ToString());
                Console.WriteLine($"SignalR: Joined waiter group - {waiterId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Failed to join waiter group: {ex.Message}");
            }
        }
    }

    public async Task JoinTableGroup(Guid tableId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("JoinTableGroup", tableId.ToString());
                Console.WriteLine($"SignalR: Joined table group - {tableId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Failed to join table group: {ex.Message}");
            }
        }
    }

    public async Task LeaveTableGroup(Guid tableId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("LeaveTableGroup", tableId.ToString());
                Console.WriteLine($"SignalR: Left table group - {tableId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Failed to leave table group: {ex.Message}");
            }
        }
    }

    public async Task SendOrderUpdate(Guid orderId, OrderStatus newStatus)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync("UpdateOrderStatus", orderId, newStatus.ToString());
                Console.WriteLine($"SignalR: Sent order update - {orderId} -> {newStatus}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR: Failed to send order update: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}
