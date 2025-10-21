namespace LibHub.UserService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string HashedPassword { get; private set; } = string.Empty;
    public string Salt { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    private User() {}

    public static User Create(string name, string email, string role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Role = role
        };
    }

    public void SetPassword(string hashedPassword, string salt)
    {
        HashedPassword = hashedPassword;
        Salt = salt;
    }

    public bool VerifyPassword(string password)
    {
        return false;
    }
}
