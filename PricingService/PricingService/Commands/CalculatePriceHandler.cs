using FluentValidation;
using MediatR;
using PricingService.Api.Commands;
using PricingService.Domain;

namespace PricingService.Commands;

public sealed class CalculatePriceHandler(IDataStore dataStore) : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
{
    public async Task<CalculatePriceResult> Handle(CalculatePriceCommand command, CancellationToken cancellationToken)
    {
        await new CalculatePriceCommandValidator().ValidateAndThrowAsync(command, cancellationToken);
        var tariff = await dataStore.Tariffs[command.ProductCode];
        if (tariff is null) throw new KeyNotFoundException($"Unknown product code {command.ProductCode}.");

        var answers = command.Answers.ToDictionary(answer => answer.QuestionCode, answer => answer.GetAnswer());
        // The farm form omits flood risk. Apply the surcharge only when the caller supplies it.
        if (string.Equals(tariff.Code, "FAI", StringComparison.OrdinalIgnoreCase))
            answers.TryAdd("FLOOD", "NO");

        var calculation = new Calculation(command.ProductCode, command.PolicyFrom, command.PolicyTo,
            command.SelectedCovers, answers);
        var price = tariff.CalculatePrice(calculation);
        return new CalculatePriceResult
        {
            TotalPrice = price.TotalPremium,
            CoverPrices = price.Covers.ToDictionary(item => item.Key, item => item.Value.Price)
        };
    }
}
