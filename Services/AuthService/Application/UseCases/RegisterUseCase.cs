public class RegisterUseCase
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;

    public RegisterUseCase(IUserRepository repository, IPasswordHasher hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    public async Task Execute(RegisterRequest request)
    {
        var existing = await _repository.GetByEmailAsync(request.Email);
        if(existing != null)
            throw new Exception("Email already in use.");

        var hash = _hasher.Hash(request.Password);

        var user = new User(
            request.Name,
            request.Email,
            hash,
            request.Role
        );

        await _repository.AddAsync(user);
    }
}