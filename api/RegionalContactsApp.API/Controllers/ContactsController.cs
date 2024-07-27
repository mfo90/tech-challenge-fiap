using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegionalContactsApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IRegionService _regionService;

        public ContactsController(IContactService contactService, IRegionService regionService)
        {
            _contactService = contactService;
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Contact>> GetContacts([FromQuery] string ddd = null)
        {
            if (string.IsNullOrWhiteSpace(ddd))
            {
                return await _contactService.GetAllContactsAsync();
            }
            else
            {
                return await _contactService.GetContactsByDDDAsync(ddd);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return contact;
        }

        [HttpPost]
        public async Task<ActionResult> CreateContact(Contact contact)
        {
            try
            {
                await _contactService.AddContactAsync(contact);
                return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            try
            {
                await _contactService.UpdateContactAsync(contact);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            await _contactService.DeleteContactAsync(id);
            return NoContent();
        }
    }
}
