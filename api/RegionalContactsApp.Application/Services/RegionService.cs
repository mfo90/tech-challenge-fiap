using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RegionalContactsApp.Application.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IContactRepository _contactRepository;

        public RegionService(IRegionRepository regionRepository, IContactRepository contactRepository)
        {
            _regionRepository = regionRepository;
            _contactRepository = contactRepository;
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            return await _regionRepository.GetAllAsync();
        }

        public async Task<Region> GetRegionByDDDAsync(string ddd)
        {
            return await _regionRepository.GetByDDDAsync(ddd);
        }

        public async Task AddRegionAsync(Region region)
        {
            ValidateRegion(region);
            await _regionRepository.AddAsync(region);
        }

        public async Task UpdateAsync(Region region)
        {
            ValidateRegion(region);
            await _regionRepository.UpdateAsync(region);
        }

        public async Task DeleteAsync(string ddd)
        {
            var contacts = await _contactRepository.GetContactsByDDDAsync(ddd);
            if (contacts.Any())
            {
                throw new ValidationException("Cannot delete region with contacts.");
            }

            await _regionRepository.DeleteAsync(ddd);
        }

        private void ValidateRegion(Region region)
        {
            if (string.IsNullOrWhiteSpace(region.DDD))
            {
                throw new ValidationException("DDD is required.");
            }
            if (region.DDD.Length > 2)
            {
                throw new ValidationException("DDD cannot exceed 2 characters.");
            }
            if (string.IsNullOrWhiteSpace(region.Name))
            {
                throw new ValidationException("Name is required.");
            }
            if (region.Name.Length > 100)
            {
                throw new ValidationException("Name cannot exceed 100 characters.");
            }
        }
    }
}
