using FoodWasteRescue.Application.FoodListings.Queries.GetMyListings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteRescue.API.Controllers;

[ApiController]
[Route("api/my-listings")]
[Authorize(Roles = "Donor")]
public class MyListingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetMyListingsQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
