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
    public async Task<IActionResult> GetAll([FromQuery] string? name = null, [FromQuery] string? address = null, [FromQuery] double? minPrice = null, [FromQuery] double? maxPrice = null, [FromQuery] int page = 1)
    {
        const int pageSize = 9;
        
       
        if (page < 1) page = 1;

        (IEnumerable<Properties> items, int totalCount) result;

        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(address) && !minPrice.HasValue && !maxPrice.HasValue)
        {
            // No filters - get all paginated
            result = await _service.GetAllPaginatedAsync(page, pageSize);
        }
        else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(address) && minPrice.HasValue && maxPrice.HasValue)
        {
            // All filters
            result = await _service.GetByAllFiltersPaginatedAsync(name, address, minPrice, maxPrice, page, pageSize);
        }
        else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(address))
        {
            // Name and address filters
            result = await _service.GetByNameAndAddressPaginatedAsync(name, address, page, pageSize);
        }
        else if (!string.IsNullOrEmpty(name) && (minPrice.HasValue || maxPrice.HasValue))
        {
            // Name and price filters
            result = await _service.GetByNameAndPriceRangePaginatedAsync(name, minPrice, maxPrice, page, pageSize);
        }
        else if (!string.IsNullOrEmpty(address) && (minPrice.HasValue || maxPrice.HasValue))
        {
            // Address and price filters
            result = await _service.GetByAddressAndPriceRangePaginatedAsync(address, minPrice, maxPrice, page, pageSize);
        }
        else if (!string.IsNullOrEmpty(name))
        {
            // Only name filter
            result = await _service.GetByNamePaginatedAsync(name, page, pageSize);
        }
        else if (!string.IsNullOrEmpty(address))
        {
            // Only address filter
            result = await _service.GetByAddressPaginatedAsync(address, page, pageSize);
        }
        else if (minPrice.HasValue || maxPrice.HasValue)
        {
            // Only price filters
            result = await _service.GetByPriceRangePaginatedAsync(minPrice, maxPrice, page, pageSize);
        }
        else
        {
            // Fallback to all paginated
            result = await _service.GetAllPaginatedAsync(page, pageSize);
        }

        var dtos = result.items.Select(p => new PropertiesDtos.PropertiesDto(p.Id, p.Name, p.Price, p.Address, p.Img, p.IdProperty, p.CodeInternal, p.Year, p.IdOwner));
        
        var totalPages = (int)Math.Ceiling((double)result.totalCount / pageSize);
        
        var paginatedResponse = new PropertiesDtos.PaginatedPropertiesDto(
            dtos, 
            result.totalCount, 
            page, 
            pageSize, 
            totalPages);
        
        return Ok(paginatedResponse);
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