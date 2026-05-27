using StoreService.Application.DTOs;

namespace StoreService.Application.Ports;

public interface IStockConfirmed
{
    Task HandleStockConfirmedAsync(StockConfirmedDto stockConfirmedDto);
}