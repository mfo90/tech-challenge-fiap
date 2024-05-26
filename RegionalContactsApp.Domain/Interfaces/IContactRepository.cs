using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionalContactsApp.Domain.Entities;

namespace RegionalContactsApp.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
        Task DeleteAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact> GetByIdAsync(int id);
        Task UpdateAsync(Contact contact);
    }
}
