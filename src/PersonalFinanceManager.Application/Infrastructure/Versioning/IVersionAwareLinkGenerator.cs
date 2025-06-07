using Carter;

namespace PersonalFinanceManager.Application.Infrastructure.Versioning;

public interface IVersionAwareLinkGenerator
{
    string? GetEndpointPath<TEndpoint>(object? values = null)
        where TEndpoint : ICarterModule;
}
