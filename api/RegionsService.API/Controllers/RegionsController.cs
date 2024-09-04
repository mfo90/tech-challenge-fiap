using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegionsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Region>> GetRegions()
        {
            return await _regionService.GetAllRegionsAsync();
        }

        [HttpGet("{ddd}")]
        public async Task<ActionResult<Region>> GetRegion(string ddd)
        {
            var region = await _regionService.GetRegionByDDDAsync(ddd);
            if (region == null)
            {
                return NotFound();
            }
            return region;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRegion([FromBody] Region region)
        {
            await _regionService.AddRegionAsync(region);
            return CreatedAtAction(nameof(GetRegion), new { ddd = region.DDD }, region);
        }

        [HttpPut("{id}")]
        public async Task<bool> UpdateAsync(Region region)
        {
            await _regionService.UpdateAsync(region);
            return true;
        }

        [HttpDelete("{ddd}")]
        public async Task<IActionResult> DeleteContact(string ddd)
        {
            await _regionService.DeleteAsync(ddd);
            return NoContent();
        }
    }
}
