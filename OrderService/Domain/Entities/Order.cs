namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set;}
    public List<OrderItem> Items { get; private set;} = new List<OrderItem>();
    public decimal TotalAmount { get; private set;}
    public string Status { get; private set;}
    public DateTime CreatedAt { get; private set;}

    // Construtor sem parâmetros para EF Core
    public Order() { }

    public Order(Guid customerId, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Items = items;
        TotalAmount = items.Sum(i => i.TotalPrice);
        CreatedAt = DateTime.UtcNow;
        Status = "CREATED";
    }
    
    public void MarkAsPaid() => Status = "PAID";
    public void MarkAsCancelled() => Status = "CANCELLED";
}