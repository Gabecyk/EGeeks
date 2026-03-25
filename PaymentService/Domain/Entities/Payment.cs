namespace PaymentService.Domain.Entities;

public class Payment
{
    public Guid id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Amount { get; private set; }
    public string FakeBoletoCode { get; private set; }
    public string Status { get; private set; }

    private Payment() { }

    public Payment(Guid orderId, Guid customerId, decimal amount, string fakeBoletoCode)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment value are invalid");

        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        FakeBoletoCode = fakeBoletoCode;
        Status = "Pending";
    }

    public void Confirm()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Payment can't be confirmed");

        Status = "Confirmed";
    }

    public void Cancel()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Payment can't be cancelled");

        Status = "Cancelled";
    }
}