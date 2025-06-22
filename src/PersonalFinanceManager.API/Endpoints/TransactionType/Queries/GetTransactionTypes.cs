namespace PersonalFinanceManager.API.Endpoints.TransactionType.Queries;

[UsedImplicitly]
public class GetTransactionTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/transactionTypes",
                ([AsParameters] GetTransactionTypesQuery query, IMediator mediator) =>
                    mediator.Send(query)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(GetTransactionTypes))
            .WithTags(nameof(TransactionType))
            .WithDescription("Get transaction types")
            .WithSummary("Retrieve all transaction types")
            .Produces<IEnumerable<TransactionTypeDto>>();
    }

    public record GetTransactionTypesQuery : IRequest<IResult>;

    [UsedImplicitly]
    public class GetTransactionTypesQueryHandler(ITransactionTypeService service)
        : IRequestHandler<GetTransactionTypesQuery, IResult>
    {
        public async Task<IResult> Handle(
            GetTransactionTypesQuery request,
            CancellationToken cancellationToken
        )
        {
            var result = await service.GetAllAsync(cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Errors.FirstOrDefault()!.Message, statusCode: 500);
        }
    }
}
