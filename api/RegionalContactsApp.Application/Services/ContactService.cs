using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RegionalContactsApp.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IRegionRepository _regionRepository;

        public ContactService(IContactRepository contactRepository, IRegionRepository regionRepository)
        {
            _contactRepository = contactRepository;
            _regionRepository = regionRepository;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            return await _contactRepository.GetByIdAsync(id);
        }

        public async Task AddContactAsync(Contact contact)
        {
            ValidateContact(contact);
            var region = await _regionRepository.GetByDDDAsync(contact.DDD);
            if (region == null)
            {
                throw new Exception("Invalid DDD.");
            }

            var existingContact = await _contactRepository.GetContactByEmailAsync(contact.Email);
            if (existingContact != null)
            {
                throw new ValidationException("Email already in use.");
            }

            // Criação da mensagem com a operação "Create"
            var contactMessage = new ContactMessage
            {
                Operation = "Create",  // Define a operação como "Create"
                Contact = contact
            };
            SendMessageToQueue(contactMessage);

            //await _contactRepository.AddAsync(contact);
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            ValidateContact(contact);
            var region = await _regionRepository.GetByDDDAsync(contact.DDD);
            if (region == null)
            {
                throw new Exception("Invalid DDD.");
            }

            var existingContact = await _contactRepository.GetContactByEmailAsync(contact.Email);
            if (existingContact != null && existingContact.Id != contact.Id)
            {
                throw new ValidationException("Email already in use.");
            }

            // Criação da mensagem com a operação "Create"
            var contactMessage = new ContactMessage
            {
                Operation = "Update",  // Define a operação como "Update"
                Contact = contact
            };

            SendMessageToQueue(contactMessage);

            //await _contactRepository.UpdateAsync(contact);
        }

        public async Task DeleteContactAsync(int id)
        {
            // Criação da mensagem com a operação "Create"
            var contactMessage = new ContactMessage
            {
                Operation = "Delete",  // Define a operação como "Delete"
                Contact.id = id
            };

            SendMessageToQueue(contactMessage);


            //await _contactRepository.DeleteAsync(id);
        }

        private void ValidateContact(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
            {
                throw new ValidationException("Name is required.");
            }
            if (contact.Name.Length > 100)
            {
                throw new ValidationException("Name cannot exceed 100 characters.");
            }
            if (!IsValidEmail(contact.Email))
            {
                throw new ValidationException("Invalid email format.");
            }
            if (contact.Email.Length > 100)
            {
                throw new ValidationException("Email cannot exceed 100 characters.");
            }
            if (contact.Phone.Length > 15)
            {
                throw new ValidationException("Phone cannot exceed 15 characters.");
            }
            if (contact.DDD.Length > 2)
            {
                throw new ValidationException("DDD cannot exceed 2 characters.");
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Contact>> GetContactsByDDDAsync(string ddd)
        {
            return await _contactRepository.GetContactsByDDDAsync(ddd); ;
        }

        public async Task<Contact> GetContactByEmailAsync(string email)
        {
            return await _contactRepository.GetContactByEmailAsync(email);
        }
        public void SendMessageToQueue(ContactMessage contactMessage)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "ContactRegisteredQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Serialize the object to JSON
                string jsonMessage = JsonConvert.SerializeObject(contactMessage);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: "ContactRegisteredQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
