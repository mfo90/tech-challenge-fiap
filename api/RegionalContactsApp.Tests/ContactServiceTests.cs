using Moq;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace RegionalContactsApp.Tests
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _mockContactRepository;
        private readonly Mock<IRegionRepository> _mockRegionRepository;
        private readonly ContactService _contactService;

        public ContactServiceTests()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _mockRegionRepository = new Mock<IRegionRepository>();
            _contactService = new ContactService(_mockContactRepository.Object, _mockRegionRepository.Object);
        }

        [Fact]
        public async Task AddContact_ShouldAddContactAsync()
        {
            // Arrange
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(It.IsAny<string>())).ReturnsAsync(new Region());

            // Act
            await _contactService.AddContactAsync(contact);

            // Assert
            _mockContactRepository.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
        }
    }
}
