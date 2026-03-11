namespace StoreService.Domain.Entities;

public class Store
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ProfileImageUrl { get; private set; } = string.Empty;

    private Store() { }

    public Store(Guid userId, string name, string description, string profileImageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Store name cannot be empty.");

        UserId = userId;
        Name = name;
        Description = description;
        ProfileImageUrl = profileImageUrl;
    }

    public void UpdateProfile(string description, string profileImageUrl)
    {
        Description = description;
        ProfileImageUrl = profileImageUrl;
    }
}