namespace PersonalFinanceManager.API.Endpoints.TransactionCategory.Queries;

[UsedImplicitly]
public class GetTransactionCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/transactionCategories",
                ([AsParameters] GetTransactionCategoriesQuery query, IMediator mediator) =>
                    mediator.Send(query)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(GetTransactionCategories))
            .WithTags(nameof(TransactionCategory))
            .WithDescription("Get transaction categories")
            .WithSummary("Get transaction categories")
            .Produces<IEnumerable<TransactionCategoryDto>>();
    }

    public record GetTransactionCategoriesQuery : IRequest<IResult>;

    [UsedImplicitly]
    public class GetTransactionCategoriesQueryHandler(ITransactionCategoryService service)
        : IRequestHandler<GetTransactionCategoriesQuery, IResult>
    {
        public async Task<IResult> Handle(
            GetTransactionCategoriesQuery request,
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
