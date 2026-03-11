public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Execute(Guid userId, string name, string email, string address)
    {
        var existingCustomer = await _customerRepository.GetProfileAsync(userId);
        if (existingCustomer != null)
            throw new ConflictException("Customer profile already exists."); 

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            throw new ValidationException("Invalid or missing user claims."); 

        if (string.IsNullOrWhiteSpace(address))
            throw new ValidationException("Address is required.");

        var customer = new Customer(userId, name, email, address);
        await _customerRepository.AddAsync(customer);
    }
}