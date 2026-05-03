namespace FoodWasteRescue.Application.FoodListings.Queries.GetImpactReport;

public class ImpactReportDto
{
    public int TotalListings { get; set; }
    public int TotalClaimed { get; set; }
    public int TotalExpired { get; set; }
    public string? MostActiveDonor { get; set; }
    public Dictionary<string, int> FoodSavedByCategory { get; set; } = [];
}
