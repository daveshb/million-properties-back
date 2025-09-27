using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Controllers;
using MyApp.Application.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Tests.Controllers;

[TestFixture]
public class PropertiesControllerTests
{
    private Mock<IPropertiesService> _propertiesServiceMock;
    private Mock<IOwnerRepository> _ownerRepositoryMock;
    private Mock<IPropertyTraceRepository> _propertyTraceRepositoryMock;
    private PropertiesController _controller;

    [SetUp]
    public void Setup()
    {
        _propertiesServiceMock = new Mock<IPropertiesService>();
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _propertyTraceRepositoryMock = new Mock<IPropertyTraceRepository>();
        _controller = new PropertiesController(_propertiesServiceMock.Object, _ownerRepositoryMock.Object, _propertyTraceRepositoryMock.Object);
    }

    [Test]
    public async Task GetAll_WithoutFilters_ReturnsAllProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa 1", 100000, "Calle 1", "img1.jpg", 1, "CODE1", 2020),
            new(2, "Casa 2", 200000, "Calle 2", "img2.jpg", 2, "CODE2", 2021)
        };
        
        _propertiesServiceMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Test]
    public async Task GetAll_WithNameFilter_ReturnsFilteredProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa Test", 100000, "Calle 1", "img1.jpg", 1, "CODE1", 2020)
        };
        
        _propertiesServiceMock.Setup(x => x.GetByNameAsync("Casa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _controller.GetAll(name: "Casa");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetAll_WithPriceRangeFilter_ReturnsFilteredProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa 1", 150000, "Calle 1", "img1.jpg", 1, "CODE1", 2020)
        };
        
        _propertiesServiceMock.Setup(x => x.GetByPriceRangeAsync(100000, 200000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _controller.GetAll(minPrice: 100000, maxPrice: 200000);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Get_WithValidId_ReturnsPropertyWithDetails()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var property = new Properties(1, "Casa Test", 100000, "Calle Test", "img.jpg", 1, "CODE1", 2020);
        var owner = new Owner(1, "Owner Name", "owner@test.com", "123456789");
        var propertyTraces = new List<PropertyTrace>
        {
            new(1, DateTime.Now, "Sale 1", 100000, 10000)
        };

        _propertiesServiceMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);
        _ownerRepositoryMock.Setup(x => x.GetByIdOwnerAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(owner);
        _propertyTraceRepositoryMock.Setup(x => x.GetByIdPropertyAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(propertyTraces);

        // Act
        var result = await _controller.Get(propertyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Test]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var propertyId = "invalid-id";
        _propertiesServiceMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Properties?)null);

        // Act
        var result = await _controller.Get(propertyId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Create_WithValidData_ReturnsCreatedProperty()
    {
        // Arrange
        var createDto = new MyApp.Application.Dtos.PropertiesDtos.CreatePropertiesDto(
            1, "New Casa", 150000, "New Address", "new-img.jpg");
        
        var createdProperty = new Properties(1, "New Casa", 150000, "New Address", "new-img.jpg", 1, "NEWCODE", 2024);

        _propertiesServiceMock.Setup(x => x.CreateAsync(1, "New Casa", 150000, "New Address", "new-img.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProperty);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().NotBeNull();
    }

    [Test]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var updateDto = new MyApp.Application.Dtos.PropertiesDtos.UpdatePropertiesDto(
            "Updated Casa", 200000, "Updated Address", "updated-img.jpg");

        _propertiesServiceMock.Setup(x => x.UpdateAsync(propertyId, "Updated Casa", 200000, "Updated Address", "updated-img.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(propertyId, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var propertyId = "invalid-id";
        var updateDto = new MyApp.Application.Dtos.PropertiesDtos.UpdatePropertiesDto(
            "Updated Casa", 200000, "Updated Address", "updated-img.jpg");

        _propertiesServiceMock.Setup(x => x.UpdateAsync(propertyId, "Updated Casa", 200000, "Updated Address", "updated-img.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(propertyId, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        _propertiesServiceMock.Setup(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(propertyId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var propertyId = "invalid-id";
        _propertiesServiceMock.Setup(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(propertyId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
