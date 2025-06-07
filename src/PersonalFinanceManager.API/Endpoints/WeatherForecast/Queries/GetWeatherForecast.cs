using Asp.Versioning;
using Carter;
using JetBrains.Annotations;
using MediatR;
using PersonalFinanceManager.API.Endpoints.WeatherForecast.Model;

namespace PersonalFinanceManager.API.Endpoints.WeatherForecast.Queries;

[UsedImplicitly]
public class GetWeatherForecasts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/WeatherForecasts",
                ([AsParameters] GetWeatherForecastsQuery query, IMediator mediator) =>
                    mediator.Send(query)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(GetWeatherForecasts))
            .WithTags(nameof(WeatherForecast))
            .WithDescription("Get weather forecasts")
            .WithSummary("Get weather forecasts")
            .RequireAuthorization()
            .Produces<List<WeatherForecastResponse>>();
    }

    public record GetWeatherForecastsQuery : IRequest<IResult>;

    [UsedImplicitly]
    public class GetWeatherForecastsQueryHandler
        : IRequestHandler<GetWeatherForecastsQuery, IResult>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching",
        };

        public Task<IResult> Handle(
            GetWeatherForecastsQuery request,
            CancellationToken cancellationToken
        )
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecastResponse(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
                .ToArray();
            return Task.FromResult(Results.Ok(forecast));
        }
    }
}
