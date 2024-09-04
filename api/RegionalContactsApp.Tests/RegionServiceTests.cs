using Moq;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RegionalContactsApp.Tests
{
    public class RegionServiceTests
    {
        private readonly Mock<IRegionRepository> _mockRegionRepository;
        private readonly Mock<IContactRepository> _mockContactRepository;
        private readonly RegionService _regionService;

        public RegionServiceTests()
        {
            _mockRegionRepository = new Mock<IRegionRepository>();
            _mockContactRepository = new Mock<IContactRepository>();
            _regionService = new RegionService(_mockRegionRepository.Object, _mockContactRepository.Object);
        }

        [Fact]
        public async Task AddRegion_ShouldAddRegionAsync()
        {
            // Arrange
            var region = new Region { DDD = "11", Name = "São Paulo" };

            // Act
            await _regionService.AddRegionAsync(region);

            // Assert
            _mockRegionRepository.Verify(r => r.AddAsync(It.IsAny<Region>()), Times.Once);
        }

        [Fact]
        public async Task AddRegion_ShouldThrowValidationException_WhenDDDIsEmpty()
        {
            // Arrange
            var region = new Region { DDD = "", Name = "São Paulo" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _regionService.AddRegionAsync(region));
        }

        [Fact]
        public async Task AddRegion_ShouldThrowValidationException_WhenDDDExceeds2Characters()
        {
            // Arrange
            var region = new Region { DDD = "111", Name = "São Paulo" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _regionService.AddRegionAsync(region));
        }

        [Fact]
        public async Task AddRegion_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var region = new Region { DDD = "11", Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _regionService.AddRegionAsync(region));
        }

        [Fact]
        public async Task AddRegion_ShouldThrowValidationException_WhenNameExceeds100Characters()
        {
            // Arrange
            var region = new Region { DDD = "11", Name = new string('a', 101) };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _regionService.AddRegionAsync(region));
        }

        [Fact]
        public async Task DeleteRegion_ShouldThrowValidationException_WhenRegionHasContacts()
        {
            // Arrange
            var ddd = "11";
            _mockContactRepository.Setup(r => r.GetContactsByDDDAsync(ddd)).ReturnsAsync(new List<Contact> { new Contact() });

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _regionService.DeleteAsync(ddd));
        }

        [Fact]
        public async Task DeleteRegion_ShouldDeleteRegion_WhenNoContacts()
        {
            // Arrange
            var ddd = "11";
            _mockContactRepository.Setup(r => r.GetContactsByDDDAsync(ddd)).ReturnsAsync(new List<Contact>());

            // Act
            await _regionService.DeleteAsync(ddd);

            // Assert
            _mockRegionRepository.Verify(r => r.DeleteAsync(ddd), Times.Once);
        }
    }
}
