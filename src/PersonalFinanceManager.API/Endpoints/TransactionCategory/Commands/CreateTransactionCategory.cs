using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManager.API.Endpoints.TransactionCategory.Queries;
using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.API.Endpoints.TransactionCategory.Commands;

[UsedImplicitly]
public class CreateTransactionCategory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/transactionCategories",
                ([FromBody] CreateTransactionCategoryCommand command, IMediator mediator) =>
                    mediator.Send(command)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(CreateTransactionCategory))
            .WithTags(nameof(TransactionCategory))
            .WithDescription("Create a transaction category")
            .WithSummary("Create a transaction category")
            .Produces<TransactionCategoryDto>()
            .ProducesProblem(500)
            .RequireAuthorization();
    }

    [UsedImplicitly]
    public record CreateTransactionCategoryCommand(
        string Name,
        string Description,
        EntityEnum.Status Status = EntityEnum.Status.Enabled
    ) : IRequest<IResult>;

    public class CreateTransactionCategoryCommandValidator
        : AbstractValidator<CreateTransactionCategoryCommand>
    {
        public CreateTransactionCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The transaction category name must not be empty.")
                .MaximumLength(30)
                .WithMessage("The transaction category name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("The transaction description must not be empty.")
                .MaximumLength(200)
                .WithMessage(
                    "The transaction category description must not exceed 500 characters."
                );
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("The transaction category status must be a valid enum value.");
        }
    }

    public class CreateTransactionCategoryCommandHandler(
        ITransactionCategoryService transactionCategoryService
    ) : IRequestHandler<CreateTransactionCategoryCommand, IResult>
    {
        public async Task<IResult> Handle(
            CreateTransactionCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await transactionCategoryService.CreateAsync(
                new UpsertTransactionCategoryDto(request.Name, request.Description, request.Status),
                cancellationToken
            );

            return result.IsSuccess
                ? Results.CreatedAtRoute(
                    nameof(GetTransactionCategory),
                    new { id = result.Value.Id },
                    result.Value
                )
                : Results.Problem(
                    result.Errors.FirstOrDefault()?.Message
                        ?? "An error occurred while creating the transaction category.",
                    statusCode: 500
                );
        }
    }
}
