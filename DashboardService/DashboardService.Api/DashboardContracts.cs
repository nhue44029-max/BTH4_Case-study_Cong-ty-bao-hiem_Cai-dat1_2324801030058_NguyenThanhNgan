namespace DashboardService.Api.Queries
{

public abstract class SalesQuery
{
    public DateTime SalesDateFrom { get; set; }
    public DateTime SalesDateTo { get; set; }
}

public sealed class GetTotalSalesQuery : SalesQuery
{
    public string? ProductCode { get; set; }
}

public sealed class GetAgentsSalesQuery : SalesQuery { }

public sealed class GetSalesTrendsQuery : SalesQuery
{
    public Dtos.TimeUnit Unit { get; set; }
}

public sealed class GetTotalSalesResult
{
    public Dtos.SalesTotalDto Total { get; set; } = new();
    public IDictionary<string, Dtos.SalesTotalDto> PerProductTotal { get; set; } = new Dictionary<string, Dtos.SalesTotalDto>();
}

public sealed class GetAgentsSalesResult
{
    public IDictionary<string, Dtos.SalesTotalDto> PerAgentTotal { get; set; } = new Dictionary<string, Dtos.SalesTotalDto>();
}

public sealed class GetSalesTrendsResult
{
    public IList<Dtos.PeriodSalesDto> PeriodsSales { get; set; } = new List<Dtos.PeriodSalesDto>();
}
}

namespace DashboardService.Api.Queries.Dtos
{

public enum TimeUnit { Day, Week, Month, Year }

public sealed class SalesTotalDto
{
    public decimal PremiumAmount { get; set; }
    public long PoliciesCount { get; set; }
}

public sealed class PeriodSalesDto
{
    public string Period { get; set; } = string.Empty;
    public SalesTotalDto Sales { get; set; } = new();
}
}
