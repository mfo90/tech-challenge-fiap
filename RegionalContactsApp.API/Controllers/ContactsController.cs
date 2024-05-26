using Microsoft.AspNetCore.Mvc;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Entities;

namespace RegionalContactsApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ContactService _contactService;
        private readonly RegionService _regionService;

        public ContactsController(ContactService contactService, RegionService regionService)
        {
            _contactService = contactService;
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Contact>> GetContacts()
        {
            return await _contactService.GetAllContactsAsync();
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
