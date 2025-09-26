using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Dtos;
using MyApp.Application.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("properties")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertiesService _service;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPropertyTraceRepository _propertyTraceRepository;
    
    public PropertiesController(IPropertiesService service, IOwnerRepository ownerRepository, IPropertyTraceRepository propertyTraceRepository)
    {
        _service = service;
        _ownerRepository = ownerRepository;
        _propertyTraceRepository = propertyTraceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name = null, [FromQuery] string? address = null, [FromQuery] double? minPrice = null, [FromQuery] double? maxPrice = null)
    {
        IEnumerable<Properties> properties;

        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(address) && (minPrice.HasValue || maxPrice.HasValue))
        {
            properties = await _service.GetByPriceRangeAsync(minPrice, maxPrice);
        }
        else if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(address))
        {
            properties = (name, address) switch
            {
                (not null, not null) => (await _service.GetByNameAsync(name))
                    .Where(p => p.Address.Contains(address, StringComparison.OrdinalIgnoreCase)),
                (not null, null) => await _service.GetByNameAsync(name),
                (null, not null) => await _service.GetByAddressAsync(address),
                _ => await _service.GetAllAsync()
            };

            if (minPrice.HasValue || maxPrice.HasValue)
            {
                properties = properties.Where(p => 
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value));
            }
        }
        else
        {
            properties = await _service.GetAllAsync();
        }

        var dtos = properties.Select(p => new PropertiesDtos.PropertiesDto(p.Id, p.Name, p.Price, p.Address, p.Img, p.IdProperty, p.CodeInternal, p.Year, p.IdOwner));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var property = await _service.GetByIdAsync(id);
        if (property == null) return NotFound();

        // Consultar datos del owner
        var owner = await _ownerRepository.GetByIdOwnerAsync(property.IdOwner);
        var ownerDto = owner != null ? new PropertiesDtos.OwnerDto(owner.Id, owner.IdOwner, owner.Name, owner.Email, owner.Phone) : null;

        // Consultar datos del property trace
        var propertyTraces = await _propertyTraceRepository.GetByIdPropertyAsync(property.IdProperty);
        var propertyTraceDtos = propertyTraces.Select(pt => new PropertiesDtos.PropertyTraceDto(pt.Id, pt.IdProperty, pt.DateSale, pt.Name, pt.Value, pt.Tax));

        var result = new PropertiesDtos.PropertiesWithDetailsDto(
            property.Id, 
            property.Name, 
            property.Price, 
            property.Address, 
            property.Img, 
            property.IdProperty, 
            property.CodeInternal, 
            property.Year, 
            property.IdOwner, 
            ownerDto, 
            propertyTraceDtos);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertiesDtos.CreatePropertiesDto dto)
    {
        var u = await _service.CreateAsync(dto.IdOwner, dto.Name, dto.Price, dto.Address, dto.Img);
        return CreatedAtAction(nameof(Get), new { id = u.Id }, new PropertiesDtos.PropertiesDto(u.Id, u.Name, u.Price, u.Address, u.Img, u.IdProperty, u.CodeInternal, u.Year, u.IdOwner));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] PropertiesDtos.UpdatePropertiesDto dto)
    {
        var ok = await _service.UpdateAsync(id, dto.Name, dto.Price, dto.Address, dto.Img);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}