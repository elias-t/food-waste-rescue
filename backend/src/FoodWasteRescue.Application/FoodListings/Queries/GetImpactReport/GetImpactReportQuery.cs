using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetImpactReport;

public record GetImpactReportQuery : IRequest<Result<ImpactReportDto>>;
