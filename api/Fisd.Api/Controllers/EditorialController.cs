using System.Text.Json;
using Fisd.Application.Models.Editorial;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Fisd.Api.Controllers;
[ApiController]
[Route("api/editorial")]
public class EditorialController(FisdDbContext db) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<EditorialContent?>> Get()
    {
        var json = await db.SiteSettings.Select(x => x.EditorialJson).FirstOrDefaultAsync();
        return Ok(json == null ? null : JsonSerializer.Deserialize<EditorialContent>(json));
    }
    [HttpPut, Authorize(Roles = "Editor,SuperAdmin,PlatformAdmin")]
    [RequestSizeLimit(1000000)]
    public async Task<ActionResult<EditorialContent>> Save(EditorialContent content)
    {
        var row = await db.SiteSettings.FirstOrDefaultAsync();
        if (row == null) { row = new SiteSettingsEntity { Id = Guid.NewGuid(), SlideDurationSeconds = 5 }; db.SiteSettings.Add(row); }
        row.EditorialJson = JsonSerializer.Serialize(content);
        await db.SaveChangesAsync();
        return Ok(content);
    }
}
