using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Dtos;
using MyApp.Application.Services;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("properties")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertiesService _service;
    public PropertiesController(IPropertiesService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _service.GetAllAsync();
        var dtos = properties.Select(p => new PropertiesDtos.PropertiesDto(p.Id, p.Name, p.Price, p.Address, p.Img));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var u = await _service.GetByIdAsync(id);
        return u == null ? NotFound() : Ok(new PropertiesDtos.PropertiesDto(u.Id, u.Name, u.Price, u.Address, u.Img));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertiesDtos.CreatePropertiesDto dto)
    {
        var u = await _service.CreateAsync(dto.IdOwner, dto.Name, dto.Price, dto.Address, dto.Img);
        return CreatedAtAction(nameof(Get), new { id = u.Id }, new PropertiesDtos.PropertiesDto(u.Id, u.Name, u.Price, u.Address, u.Img));
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