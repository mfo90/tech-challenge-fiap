using Microsoft.AspNetCore.Mvc;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;

namespace RegionalContactsApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionsController : ControllerBase
    {
        private readonly RegionService _regionService;

        public RegionsController(RegionService regionService)
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
        public async Task<ActionResult> CreateRegion(Region region)
        {
            await _regionService.AddRegionAsync(region);
            return CreatedAtAction(nameof(GetRegion), new { ddd = region.DDD }, region);
        }
    }
}
