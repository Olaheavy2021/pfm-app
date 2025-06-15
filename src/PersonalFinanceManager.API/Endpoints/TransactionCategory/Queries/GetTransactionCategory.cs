namespace PersonalFinanceManager.API.Endpoints.TransactionCategory.Queries;

[UsedImplicitly]
public class GetTransactionCategory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/transactionCategories/{id}",
                ([AsParameters] Guid id, IMediator mediator, CancellationToken ct) =>
                    mediator.Send(new GetTransactionCategoryQuery(id), ct)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(GetTransactionCategory))
            .WithTags(nameof(TransactionCategory))
            .WithDescription("Returns a transaction category by its unique identifier.")
            .WithSummary("Retrieve a transaction category")
            .Produces<TransactionCategoryDto>();
    }

    public record GetTransactionCategoryQuery(Guid Id) : IRequest<IResult>;

    [UsedImplicitly]
    public class GetTransactionCategoryQueryHandler(ITransactionCategoryService service)
        : IRequestHandler<GetTransactionCategoryQuery, IResult>
    {
        public async Task<IResult> Handle(
            GetTransactionCategoryQuery request,
            CancellationToken cancellationToken
        )
        {
            var result = await service.GetByIdAsync(request.Id, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        }
    }
}
