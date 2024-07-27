using RegionalContactsApp.Domain.Entities;

namespace RegionalContactsApp.Domain.Interfaces
{
    public interface IContactService
    {
        Task AddContactAsync(Contact contact);
        Task DeleteContactAsync(int id);
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<Contact> GetContactByIdAsync(int id);
        Task UpdateContactAsync(Contact contact);
        Task<IEnumerable<Contact>> GetContactsByDDDAsync(string ddd);
        Task<Contact> GetContactByEmailAsync(string email);
    }
}