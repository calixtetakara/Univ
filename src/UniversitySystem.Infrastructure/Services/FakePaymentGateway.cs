namespace UniversitySystem.Infrastructure.Services;

public interface IPaymentGatewayClient
{
    Task<string> CreatePaymentAsync(string reference, decimal montant, string moyen);
}

public class FakePaymentGateway : IPaymentGatewayClient
{
    public Task<string> CreatePaymentAsync(string reference, decimal montant, string moyen)
    {
        return Task.FromResult($"/Student/Payment/FakeGateway?ref={reference}");
    }
}

public class RealPaymentGateway : IPaymentGatewayClient
{
    public Task<string> CreatePaymentAsync(string reference, decimal montant, string moyen)
    {
        throw new NotImplementedException("Prestataire réel non configuré.");
    }
}