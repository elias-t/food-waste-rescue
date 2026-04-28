using FoodWasteRescue.Application.FoodListings.Queries.GetImpactReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteRescue.API.Controllers;

[ApiController]
[Route("api/impact")]
[AllowAnonymous]
public class ImpactController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetImpactReportQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
