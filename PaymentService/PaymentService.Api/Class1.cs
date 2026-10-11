namespace PaymentService.Api;

public interface IPaymentServiceContract
{
    string ServiceName { get; }
}

public sealed record PaymentServiceStatusDto(string ServiceName, string Status);
