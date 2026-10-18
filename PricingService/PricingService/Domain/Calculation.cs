namespace PricingService.Domain;

public sealed class Calculation(
    string productCode,
    DateTimeOffset policyFrom,
    DateTimeOffset policyTo,
    IReadOnlyCollection<string> selectedCovers,
    IReadOnlyDictionary<string, object> answers)
{
    public string ProductCode { get; } = productCode;
    public DateTimeOffset PolicyFrom { get; } = policyFrom;
    public DateTimeOffset PolicyTo { get; } = policyTo;
    public IReadOnlyCollection<string> SelectedCovers { get; } = selectedCovers;
    public IReadOnlyDictionary<string, object> Answers { get; } = answers;
}

public sealed record CoverPrice(decimal Price);

public sealed class PriceResult
{
    public decimal TotalPremium { get; init; }
    public Dictionary<string, CoverPrice> Covers { get; init; } = new();
}
