public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    Task<Customer> GetProfileAsync(Guid userId);
}