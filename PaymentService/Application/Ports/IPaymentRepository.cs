using PaymentService.Domain.Entities;

namespace PaymentService.Application.Ports;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task<Payment?> GetByIdAsync(Guid paymentId);
    Task UpdateAsync(Payment payment);
}