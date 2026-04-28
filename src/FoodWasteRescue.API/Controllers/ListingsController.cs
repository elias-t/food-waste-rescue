using FoodWasteRescue.Application.FoodListings.Commands.CancelListing;
using FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;
using FoodWasteRescue.Application.FoodListings.Commands.CreateFoodListing;
using FoodWasteRescue.Application.FoodListings.Queries.GetListingDetail;
using FoodWasteRescue.Application.FoodListings.Queries.GetNearbyListings;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteRescue.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController(ISender sender) : ControllerBase
{
    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetNearbyListingsQuery(latitude, longitude, radiusKm), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetListingDetailQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> Create([FromBody] CreateListingRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateFoodListingCommand(
                request.Title,
                request.Description,
                request.Category,
                request.QuantityDescription,
                request.ExpiresAt,
                request.Latitude,
                request.Longitude,
                request.Address),
            ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/claim")]
    [Authorize(Roles = "Claimer")]
    public async Task<IActionResult> Claim(Guid id, [FromBody] ClaimListingRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ClaimFoodListingCommand(id, request.Notes), ct);
        return result.IsSuccess ? Ok(new { claimId = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new CancelListingCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record CreateListingRequest(
    string Title,
    string? Description,
    FoodCategory Category,
    string QuantityDescription,
    DateTime ExpiresAt,
    double? Latitude,
    double? Longitude,
    string? Address);

public record ClaimListingRequest(string? Notes);
