namespace MyApp.Domain.Entities;

public sealed class Owner
{
    public string Id { get; private set; } = string.Empty;
    public int IdOwner { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    public Owner(int idOwner, string name, string email, string phone)
    {
        IdOwner = idOwner;
        Name = name;
        Email = email;
        Phone = phone;
    }
}
