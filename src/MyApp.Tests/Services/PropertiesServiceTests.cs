using MyApp.Application.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Tests.Services;

[TestFixture]
public class PropertiesServiceTests
{
    private Mock<IPropertiesRepository> _repositoryMock;
    private PropertiesService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IPropertiesRepository>();
        _service = new PropertiesService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa 1", 100000, "Calle 1", "img1.jpg", 1, "CODE1", 2020),
            new(2, "Casa 2", 200000, "Calle 2", "img2.jpg", 2, "CODE2", 2021)
        };

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa 1");
        result.Should().Contain(p => p.Name == "Casa 2");
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsProperty()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var property = new Properties(1, "Casa Test", 100000, "Calle Test", "img.jpg", 1, "CODE1", 2020);

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _service.GetByIdAsync(propertyId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Casa Test");
        result.Price.Should().Be(100000);
    }

    [Test]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var propertyId = "invalid-id";

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Properties?)null);

        // Act
        var result = await _service.GetByIdAsync(propertyId);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByNameAsync_ReturnsFilteredProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa Test", 100000, "Calle 1", "img1.jpg", 1, "CODE1", 2020)
        };

        _repositoryMock.Setup(x => x.GetByNameAsync("Casa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _service.GetByNameAsync("Casa");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Casa");
    }

    [Test]
    public async Task GetByAddressAsync_ReturnsFilteredProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa 1", 100000, "Calle Test", "img1.jpg", 1, "CODE1", 2020)
        };

        _repositoryMock.Setup(x => x.GetByAddressAsync("Calle", It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _service.GetByAddressAsync("Calle");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Address.Should().Contain("Calle");
    }

    [Test]
    public async Task GetByPriceRangeAsync_ReturnsFilteredProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            new(1, "Casa 1", 150000, "Calle 1", "img1.jpg", 1, "CODE1", 2020)
        };

        _repositoryMock.Setup(x => x.GetByPriceRangeAsync(100000, 200000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _service.GetByPriceRangeAsync(100000, 200000);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Price.Should().BeInRange(100000, 200000);
    }

    [Test]
    public async Task CreateAsync_WithValidData_ReturnsCreatedProperty()
    {
        // Arrange
        var property = new Properties(1, "New Casa", 150000, "New Address", "new-img.jpg", 1, "NEWCODE", 2024);

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Properties>(), It.IsAny<CancellationToken>()))
            .Callback<Properties, CancellationToken>((p, ct) => 
            {
                typeof(Properties).GetProperty("Id")!.SetValue(p, "new-id");
            });

        // Act
        var result = await _service.CreateAsync(1, "New Casa", 150000, "New Address", "new-img.jpg");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Casa");
        result.Price.Should().Be(150000);
        result.Address.Should().Be("New Address");
        result.Img.Should().Be("new-img.jpg");
        result.IdOwner.Should().Be(1);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Properties>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var existingProperty = new Properties(1, "Casa Original", 100000, "Calle Original", "img.jpg", 1, "CODE1", 2020);

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Properties>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(propertyId, "Casa Actualizada", 200000, "Calle Actualizada", "img-actualizada.jpg");

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Properties>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var propertyId = "invalid-id";

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Properties?)null);

        // Act
        var result = await _service.UpdateAsync(propertyId, "Casa Actualizada", 200000, "Calle Actualizada", "img-actualizada.jpg");

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Properties>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var existingProperty = new Properties(1, "Casa Test", 100000, "Calle Test", "img.jpg", 1, "CODE1", 2020);

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _repositoryMock.Setup(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(propertyId);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var propertyId = "invalid-id";

        _repositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Properties?)null);

        // Act
        var result = await _service.DeleteAsync(propertyId);

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()), Times.Never);
    }
}
