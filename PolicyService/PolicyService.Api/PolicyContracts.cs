namespace PolicyService.Api.Commands.Dtos
{

public enum QuestionType { Numeric, Choice }

public abstract class QuestionAnswer
{
    public string QuestionCode { get; set; } = string.Empty;
    public abstract QuestionType QuestionType { get; }
}

public sealed class ChoiceQuestionAnswer : QuestionAnswer
{
    public override QuestionType QuestionType => QuestionType.Choice;
    public string Answer { get; set; } = string.Empty;
}

public sealed class NumericQuestionAnswer : QuestionAnswer
{
    public override QuestionType QuestionType => QuestionType.Numeric;
    public decimal Answer { get; set; }
}

public sealed class PersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
}

public sealed class AddressDto
{
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
}
}

namespace PolicyService.Api.Commands
{

using PolicyService.Api.Commands.Dtos;

public sealed class CreateOfferCommand
{
    public string ProductCode { get; set; } = string.Empty;
    public DateTime PolicyFrom { get; set; }
    public DateTime PolicyTo { get; set; }
    public IList<string> SelectedCovers { get; set; } = new List<string>();
    public IList<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
}

public sealed class CreateOfferResult
{
    public string OfferNumber { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

public sealed class CreatePolicyCommand
{
    public string OfferNumber { get; set; } = string.Empty;
    public PersonDto PolicyHolder { get; set; } = new();
    public AddressDto PolicyHolderAddress { get; set; } = new();
}

public sealed class CreatePolicyResult
{
    public string PolicyNumber { get; set; } = string.Empty;
}
}

namespace PolicyService.Api.Queries
{

public sealed class GetPolicyDetailsQueryResult
{
    public PolicyDetailsDto Policy { get; set; } = new();
}

public sealed class PolicyDetailsDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string PolicyHolder { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public IList<string> Covers { get; set; } = new List<string>();
}
}
