namespace MyApp.Application.Dtos;

public class PropertiesDtos
{
    public record CreatePropertiesDto(int IdOwner, string Name, double Price, string Address, string Img);
    public record UpdatePropertiesDto(string Name, double Price, string Address, string Img);
    public record PropertiesDto(string Id, string Name, double Price, string Address, string Img, int IdProperty, string CodeInternal, int Year, int IdOwner);
    
}