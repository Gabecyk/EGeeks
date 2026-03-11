namespace StoreService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid StoreId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;

    private Product() { }

    public Product(Guid storeId, string name, string description, decimal price, int stockQuantity, string imageUrl)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        StoreId = storeId;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl;
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException("Not enough stock available.");

        StockQuantity -= quantity;
    }
}