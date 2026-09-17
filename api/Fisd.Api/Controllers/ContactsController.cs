using Fisd.Application.Models.receive.Contact;
using Fisd.Application.Models.result.Contact;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsService _contactsService;

        public ContactsController(IContactsService contactsService)
        {
            _contactsService = contactsService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ContactResultModel>>> GetVisible() => Ok(await _contactsService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ContactResultModel>>> GetAll() => Ok(await _contactsService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ContactResultModel>> Create([FromBody] SaveContactModel model) =>
            Ok(await _contactsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ContactResultModel>> Update(Guid id, [FromBody] SaveContactModel model)
        {
            var updated = await _contactsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _contactsService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
