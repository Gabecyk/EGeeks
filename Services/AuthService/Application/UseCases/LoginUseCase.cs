public class LoginUseCase
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtGenerator _jwt;

    public LoginUseCase(IUserRepository repository, IPasswordHasher hasher, IJwtGenerator jwt)
    {
        _repository = repository;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<string> Execute(LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        return _jwt.Generate(user);
    }
}