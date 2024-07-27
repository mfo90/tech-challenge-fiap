using RegionalContactsApp.Domain.Entities;

namespace RegionalContactsApp.Domain.Interfaces
{
    public interface IRegionService
    {
        Task AddRegionAsync(Region region);
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        Task<Region> GetRegionByDDDAsync(string ddd);
        Task UpdateAsync(Region region);
        Task DeleteAsync(string ddd);
    }
}