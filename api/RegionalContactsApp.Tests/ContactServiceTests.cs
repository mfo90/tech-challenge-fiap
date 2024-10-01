using Microsoft.Extensions.Configuration;
using Moq;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RegionalContactsApp.Tests
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _mockContactRepository;
        private readonly Mock<IRegionRepository> _mockRegionRepository;
        private readonly ContactService _contactService;
        private readonly Mock<IConfiguration> _configuration;
        private readonly byte[] _key;

        public ContactServiceTests()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _mockRegionRepository = new Mock<IRegionRepository>();
            _contactService = new ContactService(_mockContactRepository.Object, _mockRegionRepository.Object, _configuration.Object);
        }

        [Fact]
        public async Task AddContact_ShouldAddContactAsync()
        {
            // Arrange
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(contact.DDD)).ReturnsAsync(new Region());
            _mockContactRepository.Setup(r => r.GetContactByEmailAsync(contact.Email)).ReturnsAsync((Contact)null);

            // Act
            await _contactService.AddContactAsync(contact);

            // Assert
            _mockContactRepository.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
        }

        [Fact]
        public async Task AddContact_ShouldThrowValidationException_WhenEmailIsDuplicated()
        {
            // Arrange
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(contact.DDD)).ReturnsAsync(new Region());
            _mockContactRepository.Setup(r => r.GetContactByEmailAsync(contact.Email)).ReturnsAsync(contact);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _contactService.AddContactAsync(contact));
        } 

        [Fact]
        public async Task AddContact_ShouldThrowException_WhenDDDIsInvalid()
        {
            // Arrange
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(contact.DDD)).ReturnsAsync((Region)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _contactService.AddContactAsync(contact));
        }

        [Fact]
        public async Task AddContact_ShouldThrowValidationException_WhenEmailIsInvalid()
        {
            // Arrange
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "invalid-email", DDD = "11" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _contactService.AddContactAsync(contact));
        }

        [Fact]
        public async Task UpdateContact_ShouldUpdateContactAsync()
        {
            // Arrange
            var contact = new Contact { Id = 1, Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(contact.DDD)).ReturnsAsync(new Region());
            _mockContactRepository.Setup(r => r.GetContactByEmailAsync(contact.Email)).ReturnsAsync((Contact)null);

            // Act
            await _contactService.UpdateContactAsync(contact);

            // Assert
            _mockContactRepository.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Once);
        }

        [Fact]
        public async Task UpdateContact_ShouldThrowValidationException_WhenEmailIsDuplicated()
        {
            // Arrange
            var contact = new Contact { Id = 1, Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            var existingContact = new Contact { Id = 2, Name = "Jane Doe", Phone = "987654321", Email = "john@example.com", DDD = "11" };
            _mockRegionRepository.Setup(r => r.GetByDDDAsync(contact.DDD)).ReturnsAsync(new Region());
            _mockContactRepository.Setup(r => r.GetContactByEmailAsync(contact.Email)).ReturnsAsync(existingContact);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _contactService.UpdateContactAsync(contact));
        }

        [Fact]
        public async Task DeleteContact_ShouldDeleteContactAsync()
        {
            // Arrange
            var contactId = 1;
            _mockContactRepository.Setup(r => r.DeleteAsync(contactId)).Returns(Task.CompletedTask);

            // Act
            await _contactService.DeleteContactAsync(contactId);

            // Assert
            _mockContactRepository.Verify(r => r.DeleteAsync(contactId), Times.Once);
        }

        [Fact]
        public async Task GetContactById_ShouldReturnContact_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var expectedContact = new Contact { Id = contactId, Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };
            _mockContactRepository.Setup(r => r.GetByIdAsync(contactId)).ReturnsAsync(expectedContact);

            // Act
            var actualContact = await _contactService.GetContactByIdAsync(contactId);

            // Assert
            Assert.Equal(expectedContact, actualContact);
        }

        [Fact]
        public async Task GetAllContacts_ShouldReturnAllContacts()
        {
            // Arrange
            var expectedContacts = new List<Contact>
            {
                new Contact { Id = 1, Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" },
                new Contact { Id = 2, Name = "Jane Doe", Phone = "987654321", Email = "jane@example.com", DDD = "12" }
            };
            _mockContactRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedContacts);

            // Act
            var actualContacts = await _contactService.GetAllContactsAsync();

            // Assert
            Assert.Equal(expectedContacts, actualContacts);
        }
    }
}
