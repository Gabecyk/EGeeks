public class GetProfileUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public GetProfileUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> Execute(Guid userId)
    {
        Customer customer;
        try
        {
            customer = await _customerRepository.GetProfileAsync(userId);
            if (customer == null)
                throw new ConflictException("Customer profile not found.");

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while retrieving the customer profile.", ex);
        }

        return customer;
    }
}