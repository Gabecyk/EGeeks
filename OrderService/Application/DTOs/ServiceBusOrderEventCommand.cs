using OrderService.Application.Events;

namespace OrderService.Application.DTOs;

public record ServiceBusOrderEventCommand(string EventType, OrderStatus? OrderStatus);
