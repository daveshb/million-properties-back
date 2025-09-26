namespace MyApp.Application.Dtos;

public class PropertiesDtos
{
    public record CreatePropertiesDto(int IdOwner, string Name, double Price, string Address, string Img);
    public record UpdatePropertiesDto(string Name, double Price, string Address, string Img);
    public record PropertiesDto(string Id, string Name, double Price, string Address, string Img, int IdProperty, string CodeInternal, int Year, int IdOwner);
    public record OwnerDto(string Id, int IdOwner, string Name, string Email, string Phone);
    public record PropertyTraceDto(string Id, int IdProperty, DateTime DateSale, string Name, double Value, double Tax);
    public record PropertiesWithDetailsDto(string Id, string Name, double Price, string Address, string Img, int IdProperty, string CodeInternal, int Year, int IdOwner, OwnerDto? Owner, IEnumerable<PropertyTraceDto> PropertyTraces);
    
}