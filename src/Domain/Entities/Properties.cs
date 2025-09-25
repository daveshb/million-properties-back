namespace MyApp.Domain.Entities;

public sealed class Properties
{
    public string Id { get; private set; } = string.Empty;   // MongoDB ObjectId
    public int IdOwner { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public double Price { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string Img { get; private set; } = default!;

    public Properties(int idOwner, string name, double price, string address, string img)
    {
        IdOwner = idOwner;
        Name = name;
        Price = price;
        Address = address;
        Img = img;
    }

    public void Update(string name, double price, string address, string img)
    {
        Name = name;
        Price = price;
        Address = address;
        Img = img;
    }
}
