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

            await _contactRepository.AddAsync(contact);
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

            await _contactRepository.UpdateAsync(contact);
        }

        public async Task DeleteContactAsync(int id)
        {
            await _contactRepository.DeleteAsync(id);
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
    }
}
