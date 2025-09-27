using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Dtos;
using MyApp.Application.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyApp.Tests.Integration;

[TestFixture]
public class PropertiesControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace real services with mocks for integration testing
                    services.AddScoped<IPropertiesService, MockPropertiesService>();
                    services.AddScoped<IOwnerRepository, MockOwnerRepository>();
                    services.AddScoped<IPropertyTraceRepository, MockPropertyTraceRepository>();
                });
            });

        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task GetAll_ReturnsOkWithProperties()
    {
        // Act
        var response = await _client.GetAsync("/properties");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetAll_WithNameFilter_ReturnsFilteredProperties()
    {
        // Act
        var response = await _client.GetAsync("/properties?name=Test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetAll_WithPriceRange_ReturnsFilteredProperties()
    {
        // Act
        var response = await _client.GetAsync("/properties?minPrice=100000&maxPrice=500000");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Get_WithValidId_ReturnsPropertyWithDetails()
    {
        // Act
        var response = await _client.GetAsync("/properties/507f1f77bcf86cd799439011");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new PropertiesDtos.CreatePropertiesDto(
            1, "Test Casa", 150000, "Test Address", "test-img.jpg");

        // Act
        var response = await _client.PostAsJsonAsync("/properties", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new PropertiesDtos.UpdatePropertiesDto(
            "Updated Casa", 200000, "Updated Address", "updated-img.jpg");

        // Act
        var response = await _client.PutAsJsonAsync("/properties/507f1f77bcf86cd799439011", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/properties/507f1f77bcf86cd799439011");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Swagger_IsAccessible()
    {
        // Act
        var response = await _client.GetAsync("/swagger");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

// Mock implementations for integration testing
public class MockPropertiesService : IPropertiesService
{
    private readonly List<Properties> _properties = new()
    {
        new(1, "Test Casa 1", 100000, "Test Calle 1", "test-img1.jpg", 1, "TEST1", 2020),
        new(2, "Test Casa 2", 200000, "Test Calle 2", "test-img2.jpg", 2, "TEST2", 2021),
        new(3, "Test Casa 3", 150000, "Test Calle 3", "test-img3.jpg", 3, "TEST3", 2019),
        new(4, "Test Casa 4", 250000, "Test Calle 4", "test-img4.jpg", 4, "TEST4", 2022),
        new(5, "Test Casa 5", 180000, "Test Calle 5", "test-img5.jpg", 5, "TEST5", 2021),
        new(6, "Test Casa 6", 120000, "Test Calle 6", "test-img6.jpg", 6, "TEST6", 2020),
        new(7, "Test Casa 7", 300000, "Test Calle 7", "test-img7.jpg", 7, "TEST7", 2023),
        new(8, "Test Casa 8", 160000, "Test Calle 8", "test-img8.jpg", 8, "TEST8", 2021),
        new(9, "Test Casa 9", 220000, "Test Calle 9", "test-img9.jpg", 9, "TEST9", 2022),
        new(10, "Test Casa 10", 190000, "Test Calle 10", "test-img10.jpg", 10, "TEST10", 2020),
        new(11, "Test Casa 11", 280000, "Test Calle 11", "test-img11.jpg", 11, "TEST11", 2023)
    };

    public Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<Properties>>(_properties);
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetAllPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var skip = (pageNumber - 1) * pageSize;
        var items = _properties.Skip(skip).Take(pageSize);
        return Task.FromResult((items, _properties.Count));
    }

    public Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var properties = _properties.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult<IEnumerable<Properties>>(properties);
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNamePaginatedAsync(string name, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default)
    {
        var properties = _properties.Where(p => p.Address.Contains(address, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult<IEnumerable<Properties>>(properties);
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressPaginatedAsync(string address, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => p.Address.Contains(address, StringComparison.OrdinalIgnoreCase)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default)
    {
        var properties = _properties.Where(p => 
            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price <= maxPrice.Value)).ToList();
        return Task.FromResult<IEnumerable<Properties>>(properties);
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByPriceRangePaginatedAsync(double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => 
            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price <= maxPrice.Value)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndAddressPaginatedAsync(string name, string address, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => 
            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) &&
            p.Address.Contains(address, StringComparison.OrdinalIgnoreCase)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndPriceRangePaginatedAsync(string name, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => 
            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) &&
            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price <= maxPrice.Value)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressAndPriceRangePaginatedAsync(string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => 
            p.Address.Contains(address, StringComparison.OrdinalIgnoreCase) &&
            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price <= maxPrice.Value)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAllFiltersPaginatedAsync(string name, string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filtered = _properties.Where(p => 
            (string.IsNullOrEmpty(name) || p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(address) || p.Address.Contains(address, StringComparison.OrdinalIgnoreCase)) &&
            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || p.Price <= maxPrice.Value)).ToList();
        var skip = (pageNumber - 1) * pageSize;
        var items = filtered.Skip(skip).Take(pageSize);
        return Task.FromResult((items, filtered.Count));
    }

    public Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var property = new Properties(1, "Test Casa", 100000, "Test Calle", "test-img.jpg", 1, "TEST1", 2020);
        typeof(Properties).GetProperty("Id")!.SetValue(property, id);
        return Task.FromResult<Properties?>(property);
    }

    public async Task<Properties> CreateAsync(int idOwner, string name, double price, string address, string img, CancellationToken ct = default)
    {
        var property = new Properties(idOwner, name, price, address, img, 1, "NEWCODE", 2024);
        typeof(Properties).GetProperty("Id")!.SetValue(property, "new-id");
        return property;
    }

    public Task<bool> UpdateAsync(string id, string name, double price, string address, string img, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}

public class MockOwnerRepository : IOwnerRepository
{
    public Task<Owner?> GetByIdOwnerAsync(int idOwner, CancellationToken ct = default)
    {
        var owner = new Owner(idOwner, "Test Owner", "test@owner.com", "123456789");
        typeof(Owner).GetProperty("Id")!.SetValue(owner, "owner-id");
        return Task.FromResult<Owner?>(owner);
    }
}

public class MockPropertyTraceRepository : IPropertyTraceRepository
{
    public Task<IEnumerable<PropertyTrace>> GetByIdPropertyAsync(int idProperty, CancellationToken ct = default)
    {
        var traces = new List<PropertyTrace>
        {
            new(idProperty, DateTime.Now, "Test Sale", 100000, 10000)
        };
        return Task.FromResult<IEnumerable<PropertyTrace>>(traces);
    }
}
