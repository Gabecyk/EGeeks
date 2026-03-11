public class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }

    private Customer() { }

    public Customer(Guid userId, string name, string email, string address)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Address = address;
    }

    public void UpdateAddress(string address)
    {
        Address = address;
    }
}