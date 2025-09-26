namespace MyApp.Domain.Entities;

public sealed class PropertyTrace
{
    public string Id { get; private set; } = string.Empty;
    public int IdProperty { get; private set; } = default!;
    public DateTime DateSale { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public double Value { get; private set; } = default!;
    public double Tax { get; private set; } = default!;

    public PropertyTrace(int idProperty, DateTime dateSale, string name, double value, double tax)
    {
        IdProperty = idProperty;
        DateSale = dateSale;
        Name = name;
        Value = value;
        Tax = tax;
    }
}
