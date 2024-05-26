using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Application.Services
{
    public class ContactService
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
            var region = await _regionRepository.GetByDDDAsync(contact.DDD);
            if (region == null)
            {
                throw new Exception("Invalid DDD.");
            }

            // Additional validations can be added here
            await _contactRepository.AddAsync(contact);
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            var region = await _regionRepository.GetByDDDAsync(contact.DDD);
            if (region == null)
            {
                throw new Exception("Invalid DDD.");
            }

            // Additional validations can be added here
            await _contactRepository.UpdateAsync(contact);
        }

        public async Task DeleteContactAsync(int id)
        {
            await _contactRepository.DeleteAsync(id);
        }
    }
}
