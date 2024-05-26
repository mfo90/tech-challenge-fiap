using Microsoft.AspNetCore.Mvc;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegionalContactsApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult> CreateRegion(Region region)
        {
            await _regionService.AddRegionAsync(region);
            return CreatedAtAction(nameof(GetRegion), new { ddd = region.DDD }, region);
        }
    }
}
