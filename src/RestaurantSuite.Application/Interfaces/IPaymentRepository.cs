using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    void Update(Payment payment);
}
