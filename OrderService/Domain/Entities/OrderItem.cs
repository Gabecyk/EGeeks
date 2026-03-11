namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set;}
    public int Quantity { get; private set;}
    public decimal UnitPrice { get; private set;}
    public decimal TotalPrice => Quantity * UnitPrice;

    // Construtor sem parâmetros para EF Core
    public OrderItem() { }

    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}