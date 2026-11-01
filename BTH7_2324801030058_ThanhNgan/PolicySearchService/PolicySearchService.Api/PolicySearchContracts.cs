namespace PolicySearchService.Api.Queries
{

public sealed class FindPolicyResult
{
    public List<Dtos.PolicyDto> Policies { get; set; } = new();
}
}

namespace PolicySearchService.Api.Queries.Dtos
{

public sealed class PolicyDto
{
    public string PolicyNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public DateTime PolicyStartDate { get; set; }
    public DateTime PolicyEndDate { get; set; }
    public string PolicyHolder { get; set; } = string.Empty;
    public decimal PremiumAmount { get; set; }
}
}
