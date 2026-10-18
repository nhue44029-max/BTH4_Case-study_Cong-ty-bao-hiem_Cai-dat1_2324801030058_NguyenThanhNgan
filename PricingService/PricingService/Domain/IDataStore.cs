namespace PricingService.Domain;

public interface IDataStore
{
    ITariffRepository Tariffs { get; }
}

public interface ITariffRepository
{
    Task<Tariff?> this[string productCode] { get; }
}

public sealed class InMemoryTariffDataStore : IDataStore, ITariffRepository
{
    private readonly Dictionary<string, Tariff> tariffs = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryTariffDataStore()
    {
        var travel = new Tariff("TRI");
        travel.BasePremiumRules.AddBasePriceRule("C1", null, "NUM_OF_ADULTS * (DESTINATION == \"EUR\" ? 26M : 34M)");
        travel.BasePremiumRules.AddBasePriceRule("C2", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 26M");
        travel.BasePremiumRules.AddBasePriceRule("C3", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 10M");
        travel.DiscountMarkupRules.AddPercentMarkup("DESTINATION == \"WORLD\"", 1.5M);
        tariffs.Add(travel.Code, travel);

        var house = new Tariff("HSI");
        house.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"APT\"", "AREA * 1.25M");
        house.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"HOUSE\"", "AREA * 1.50M");
        house.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"APT\"", "AREA * 0.25M");
        house.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"HOUSE\"", "AREA * 0.45M");
        house.BasePremiumRules.AddBasePriceRule("C3", null, "30M");
        house.BasePremiumRules.AddBasePriceRule("C4", null, "50M");
        house.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.50M);
        house.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 1", 1.25M);
        tariffs.Add(house.Code, house);

        var farm = new Tariff("FAI");
        farm.BasePremiumRules.AddBasePriceRule("C1", null, "10M");
        farm.BasePremiumRules.AddBasePriceRule("C2", null, "20M");
        farm.BasePremiumRules.AddBasePriceRule("C3", null, "30M");
        farm.BasePremiumRules.AddBasePriceRule("C4", null, "40M");
        farm.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.50M);
        farm.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 2M);
        tariffs.Add(farm.Code, farm);

        var car = new Tariff("CAR");
        car.BasePremiumRules.AddBasePriceRule("C1", null, "100M");
        car.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 1.50M);
        tariffs.Add(car.Code, car);
    }

    public ITariffRepository Tariffs => this;
    public Task<Tariff?> this[string productCode] =>
        Task.FromResult(tariffs.GetValueOrDefault(productCode));
}
