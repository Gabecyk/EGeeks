using StoreService.Application.Events;

namespace StoreService.Application.DTOs;

public record StockConfirmedDto(
    string EventType, 
    StockConfirmedEvent EventData
);