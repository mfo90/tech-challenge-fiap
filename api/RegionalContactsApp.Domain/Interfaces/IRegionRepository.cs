using RegionalContactsApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Domain.Interfaces
{
    public interface IRegionRepository
    {
        Task AddAsync(Region region);
        Task<IEnumerable<Region>> GetAllAsync();
        Task<Region> GetByDDDAsync(string ddd);
        Task UpdateAsync(Region region);
        Task DeleteAsync(string ddd);
        Task<IEnumerable<Contact>> GetContactsByDDDAsync(string ddd);
    }
}
