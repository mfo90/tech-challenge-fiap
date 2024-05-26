using Moq;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Tests
{
    public class ContactServiceTests
    {
        [Fact]
        public async Task AddContact_ShouldAddContactAsync()
        {
            // Arrange
            var mockRepo = new Mock<IContactRepository>();
            var service = new ContactService(mockRepo.Object);
            var contact = new Contact { Name = "John Doe", Phone = "123456789", Email = "john@example.com", DDD = "11" };

            // Act
            await service.AddContactAsync(contact);

            // Assert
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
        }
    }
}
