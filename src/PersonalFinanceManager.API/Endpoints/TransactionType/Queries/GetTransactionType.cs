using PersonalFinanceManager.API.Endpoints.TransactionCategory.Queries;

namespace PersonalFinanceManager.API.Endpoints.TransactionType.Queries;

[UsedImplicitly]
public class GetTransactionType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/transactionTypes/{Id}",
                (
                    [AsParameters] GetTransactionTypeQuery query,
                    IMediator mediator,
                    CancellationToken ct
                ) => mediator.Send(query, ct)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(GetTransactionType))
            .WithTags(nameof(TransactionType))
            .WithDescription("Returns a transaction type by its unique identifier.")
            .WithSummary("Retrieve a transaction type")
            .Produces<TransactionTypeDto>();
    }

    [UsedImplicitly]
    public record GetTransactionTypeQuery(Guid Id) : IRequest<IResult>;

    [UsedImplicitly]
    public class GetTransactionTypeQueryValidator : AbstractValidator<GetTransactionTypeQuery>
    {
        public GetTransactionTypeQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("The transaction type ID must not be empty.");
        }
    }

    [UsedImplicitly]
    public class GetTransactionTypeQueryHandler(ITransactionTypeService service)
        : IRequestHandler<GetTransactionTypeQuery, IResult>
    {
        public async Task<IResult> Handle(
            GetTransactionTypeQuery request,
            CancellationToken cancellationToken
        )
        {
            var result = await service.GetByIdAsync(request.Id, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
        }
    }
}
