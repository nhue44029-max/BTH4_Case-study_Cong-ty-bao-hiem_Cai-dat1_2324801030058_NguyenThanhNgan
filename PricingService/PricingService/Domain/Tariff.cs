namespace PricingService.Domain;

public sealed class Tariff(string code)
{
    public string Code { get; } = code;
    public BasePremiumRuleSet BasePremiumRules { get; } = new();
    public MarkupRuleSet DiscountMarkupRules { get; } = new();

    public PriceResult CalculatePrice(Calculation calculation)
    {
        if (calculation.ProductCode != Code)
            throw new ArgumentException("The calculation uses a different product.", nameof(calculation));

        var covers = new Dictionary<string, CoverPrice>();
        foreach (var cover in calculation.SelectedCovers.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var matching = BasePremiumRules.Rules
                .Where(rule => string.Equals(rule.CoverCode, cover, StringComparison.OrdinalIgnoreCase))
                .Where(rule => rule.Condition is null || RuleExpression.Evaluate(rule.Condition, calculation.Answers) is true)
                .ToList();
            if (matching.Count != 1)
                throw new InvalidOperationException($"Expected one pricing rule for cover {cover}; found {matching.Count}.");

            var basePrice = Convert.ToDecimal(RuleExpression.Evaluate(matching[0].PriceExpression, calculation.Answers));
            var markup = DiscountMarkupRules.Rules
                .Where(rule => RuleExpression.Evaluate(rule.Condition, calculation.Answers) is true)
                .Aggregate(1m, (current, rule) => current * rule.Multiplier);
            covers.Add(cover, new CoverPrice(basePrice * markup));
        }
        return new PriceResult { Covers = covers, TotalPremium = covers.Values.Sum(value => value.Price) };
    }
}

public sealed record BasePremiumRule(string CoverCode, string? Condition, string PriceExpression);

public sealed class BasePremiumRuleSet
{
    public List<BasePremiumRule> Rules { get; } = new();
    public void AddBasePriceRule(string coverCode, string? condition, string priceExpression) =>
        Rules.Add(new BasePremiumRule(coverCode, condition, priceExpression));
}

public sealed record MarkupRule(string Condition, decimal Multiplier);

public sealed class MarkupRuleSet
{
    public List<MarkupRule> Rules { get; } = new();
    public void AddPercentMarkup(string condition, decimal multiplier) => Rules.Add(new MarkupRule(condition, multiplier));
}
