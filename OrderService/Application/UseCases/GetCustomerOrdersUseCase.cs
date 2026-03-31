using OrderService.Application.Ports;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases;

public class GetCustomerOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrdersUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Order>> Execute(Guid customerId)
    {
        List<Order> orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return orders;
    }
}